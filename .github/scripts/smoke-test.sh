#!/bin/bash
# Backend Smoke Test Script
# Tests basic functionality of the deployed backend API

set -e

# Configuration
API_URL="${API_URL:-https://gsc-tracking-api-staging.fly.dev}"
MAX_RETRIES=30
RETRY_DELAY=5

echo "🧪 Starting Backend Smoke Tests..."
echo "📍 Testing API at: $API_URL"
echo ""

# Function to test an endpoint
test_endpoint() {
    local endpoint=$1
    local expected_status=${2:-200}
    local description=$3
    
    echo "Testing: $description"
    echo "  Endpoint: $endpoint"
    
    # Make request and capture response
    response=$(curl -s -w "\n%{http_code}" "$endpoint" 2>&1 || echo "FAILED
000")
    http_code=$(echo "$response" | tail -n 1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "$expected_status" ]; then
        echo "  ✅ Status: $http_code (Expected: $expected_status)"
        if [ -n "$body" ]; then
            echo "  📄 Response: ${body:0:100}..."
        fi
        echo ""
        return 0
    else
        echo "  ❌ Status: $http_code (Expected: $expected_status)"
        echo "  📄 Response: $body"
        echo ""
        return 1
    fi
}

# Wait for API to be available
echo "⏳ Waiting for API to be available..."
retry_count=0
while [ $retry_count -lt $MAX_RETRIES ]; do
    if curl -s -f -o /dev/null "$API_URL/api/hello" 2>/dev/null; then
        echo "✅ API is responding!"
        echo ""
        break
    fi
    
    retry_count=$((retry_count + 1))
    if [ $retry_count -eq $MAX_RETRIES ]; then
        echo "❌ API failed to respond after $MAX_RETRIES attempts"
        exit 1
    fi
    
    echo "  Attempt $retry_count/$MAX_RETRIES - waiting ${RETRY_DELAY}s..."
    sleep $RETRY_DELAY
done

# Test results
failed_tests=0
total_tests=0

# Test 1: Health Check Endpoint
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/api/hello" 200 "Health Check Endpoint"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi

# Test 2: API Root (should redirect or return 404)
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/api" 404 "API Root (expecting 404)"; then
    :
else
    # Accept other codes as valid
    echo "  ℹ️  Non-404 response acceptable for API root"
fi

# Test 3: Swagger endpoint (should be available in staging)
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/swagger/index.html" 200 "Swagger UI"; then
    :
else
    echo "  ⚠️  Swagger may not be enabled in this environment"
fi

# Test 4: Customers endpoint (basic API functionality) should return 401
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/api/customers" 401 "Customers API Endpoint"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi



# Summary
echo "================================================"
echo "📊 Smoke Test Summary"
echo "================================================"
echo "Total Tests: $total_tests"
echo "Passed: $((total_tests - failed_tests))"
echo "Failed: $failed_tests"
echo ""

if [ $failed_tests -eq 0 ]; then
    echo "✅ All critical smoke tests passed!"
    exit 0
else
    echo "❌ Some smoke tests failed!"
    exit 1
fi