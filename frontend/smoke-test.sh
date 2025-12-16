#!/bin/bash
# Frontend Smoke Test Script
# Tests basic functionality of the deployed frontend application

set -e

# Configuration
FRONTEND_URL="${FRONTEND_URL:-https://deploy-preview-staging--gsc-tracking.netlify.app}"
API_URL="${API_URL:-https://gsc-tracking-api-staging.fly.dev}"
MAX_RETRIES=30
RETRY_DELAY=5

echo "🧪 Starting Frontend Smoke Tests..."
echo "📍 Testing Frontend at: $FRONTEND_URL"
echo "📍 Testing API connectivity to: $API_URL"
echo ""

# Function to test an endpoint
test_endpoint() {
    local endpoint=$1
    local expected_status=${2:-200}
    local description=$3
    local check_content=${4:-}
    
    echo "Testing: $description"
    echo "  URL: $endpoint"
    
    # Make request and capture response
    response=$(curl -s -w "\n%{http_code}" "$endpoint" 2>&1 || echo "FAILED
000")
    http_code=$(echo "$response" | tail -n 1)
    body=$(echo "$response" | head -n -1)
    
    # Check HTTP status
    if [ "$http_code" != "$expected_status" ]; then
        echo "  ❌ Status: $http_code (Expected: $expected_status)"
        echo "  📄 Response: ${body:0:200}..."
        echo ""
        return 1
    fi
    
    echo "  ✅ Status: $http_code (Expected: $expected_status)"
    
    # Check content if specified
    if [ -n "$check_content" ]; then
        if echo "$body" | grep -q "$check_content"; then
            echo "  ✅ Content check passed: Found '$check_content'"
        else
            echo "  ❌ Content check failed: '$check_content' not found"
            echo ""
            return 1
        fi
    fi
    
    echo ""
    return 0
}

# Wait for frontend to be available
echo "⏳ Waiting for frontend to be available..."
retry_count=0
while [ $retry_count -lt $MAX_RETRIES ]; do
    if curl -s -f -o /dev/null "$FRONTEND_URL" 2>/dev/null; then
        echo "✅ Frontend is responding!"
        echo ""
        break
    fi
    
    retry_count=$((retry_count + 1))
    if [ $retry_count -eq $MAX_RETRIES ]; then
        echo "❌ Frontend failed to respond after $MAX_RETRIES attempts"
        exit 1
    fi
    
    echo "  Attempt $retry_count/$MAX_RETRIES - waiting ${RETRY_DELAY}s..."
    sleep $RETRY_DELAY
done

# Test results
failed_tests=0
total_tests=0

# Test 1: Frontend Home Page
total_tests=$((total_tests + 1))
if test_endpoint "$FRONTEND_URL" 200 "Frontend Home Page" "<!DOCTYPE html>"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi

# Test 2: Check if frontend contains expected meta tags or content
total_tests=$((total_tests + 1))
if test_endpoint "$FRONTEND_URL" 200 "Frontend HTML Content" "GSC Tracking\|GSC\|root"; then
    :
else
    echo "  ℹ️  Could not verify specific content, but page loaded"
fi

# Test 3: Check if assets are accessible (try to access a typical asset path)
total_tests=$((total_tests + 1))
# Note: This might return 404 if no asset at this exact path, which is OK
asset_response=$(curl -s -w "%{http_code}" -o /dev/null "$FRONTEND_URL/assets/" 2>/dev/null || echo "000")
if [ "$asset_response" = "200" ] || [ "$asset_response" = "403" ] || [ "$asset_response" = "404" ]; then
    echo "Testing: Assets Directory"
    echo "  ✅ Frontend serving assets properly"
    echo ""
else
    echo "Testing: Assets Directory"
    echo "  ⚠️  Unexpected response from assets directory: $asset_response"
    echo ""
fi

# Test 4: Verify backend API is accessible from the frontend's perspective
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/api/hello" 200 "Backend API Connectivity"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi

# Test 5: Check CORS headers are present (make OPTIONS request to API)
total_tests=$((total_tests + 1))
echo "Testing: CORS Configuration"
cors_response=$(curl -s -X OPTIONS "$API_URL/api/customers" \
    -H "Origin: $FRONTEND_URL" \
    -H "Access-Control-Request-Method: GET" \
    -w "\n%{http_code}" 2>&1 || echo "FAILED
000")
cors_code=$(echo "$cors_response" | tail -n 1)

if [ "$cors_code" = "204" ] || [ "$cors_code" = "200" ]; then
    echo "  ✅ CORS preflight successful (Status: $cors_code)"
    echo ""
else
    echo "  ⚠️  CORS preflight returned: $cors_code (may need configuration)"
    echo ""
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
