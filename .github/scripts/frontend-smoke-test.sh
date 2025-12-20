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