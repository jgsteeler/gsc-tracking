# Smoke Testing Guide

This document describes the automated smoke tests for the GSC Tracking application and how to use them.

## Overview

Smoke tests are automated tests that verify the basic functionality of deployed applications. They run automatically as part of the CI/CD pipeline after each deployment to ensure the application is running correctly.

## Backend Smoke Tests

### Location
`backend/smoke-test.sh`

### What It Tests

1. **API Availability** - Waits for the API to respond (with retries)
2. **Health Check Endpoint** - Tests `/api/hello` returns 200 OK
3. **Swagger UI** - Verifies Swagger documentation is accessible (in staging)
4. **Customers API** - Tests `/api/customers` endpoint functionality
5. **Jobs API** - Tests `/api/jobs` endpoint functionality

### Usage

#### In CI/CD (Automatic)
The smoke tests run automatically after each deployment in the GitHub Actions workflow.

#### Manual Execution
```bash
# Test staging environment
cd backend
API_URL="https://gsc-tracking-api-staging.fly.dev" ./smoke-test.sh

# Test production environment
API_URL="https://gsc-tracking-api.fly.dev" ./smoke-test.sh

# Test local environment
API_URL="http://localhost:8080" ./smoke-test.sh
```

### Configuration

- **API_URL**: The base URL of the API to test (required)
- **MAX_RETRIES**: Maximum number of retries waiting for API (default: 30)
- **RETRY_DELAY**: Delay between retries in seconds (default: 5)

### Exit Codes

- `0` - All tests passed
- `1` - One or more tests failed

## Frontend Smoke Tests

### Location
`frontend/smoke-test.sh`

### What It Tests

1. **Frontend Availability** - Waits for the frontend to respond (with retries)
2. **Home Page** - Verifies the home page loads successfully
3. **HTML Content** - Checks for expected content in the HTML
4. **Assets** - Verifies assets are being served correctly
5. **Backend Connectivity** - Tests that the frontend can reach the backend API
6. **CORS Configuration** - Verifies CORS is configured correctly

### Usage

#### In CI/CD (Automatic)
Frontend smoke tests can be added to Netlify deployment workflow.

#### Manual Execution
```bash
# Test staging environment
cd frontend
FRONTEND_URL="https://deploy-preview-staging--gsc-tracking.netlify.app" \
API_URL="https://gsc-tracking-api-staging.fly.dev" \
./smoke-test.sh

# Test production environment
FRONTEND_URL="https://gsc-tracking.netlify.app" \
API_URL="https://gsc-tracking-api.fly.dev" \
./smoke-test.sh

# Test local environment
FRONTEND_URL="http://localhost:5173" \
API_URL="http://localhost:8080" \
./smoke-test.sh
```

### Configuration

- **FRONTEND_URL**: The URL of the frontend to test (required)
- **API_URL**: The URL of the backend API (required)
- **MAX_RETRIES**: Maximum number of retries waiting for frontend (default: 30)
- **RETRY_DELAY**: Delay between retries in seconds (default: 5)

### Exit Codes

- `0` - All tests passed
- `1` - One or more tests failed

## Integration with CI/CD

### Current Integration

Backend smoke tests are integrated into the deployment workflow:

```yaml
# .github/workflows/deploy-flyio.yml
- name: Run Backend Smoke Tests
  run: |
    chmod +x ./backend/smoke-test.sh
    API_URL="https://gsc-tracking-api-staging.fly.dev" ./backend/smoke-test.sh
```

### Benefits

1. **Early Detection** - Catches deployment issues immediately
2. **Automated Validation** - No manual testing required after deployment
3. **Confidence** - Ensures basic functionality before marking deployment as successful
4. **Fast Feedback** - Tests complete in under 1 minute typically

## Extending Smoke Tests

### Adding New Tests to Backend

Edit `backend/smoke-test.sh` and add new test cases:

```bash
# Test X: New Feature Endpoint
total_tests=$((total_tests + 1))
if test_endpoint "$API_URL/api/new-feature" 200 "New Feature Endpoint"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi
```

### Adding New Tests to Frontend

Edit `frontend/smoke-test.sh` and add new test cases:

```bash
# Test X: New Page
total_tests=$((total_tests + 1))
if test_endpoint "$FRONTEND_URL/new-page" 200 "New Page"; then
    :
else
    failed_tests=$((failed_tests + 1))
fi
```

## Troubleshooting

### Tests Failing Locally

1. **Check API/Frontend is Running**
   ```bash
   curl -I https://gsc-tracking-api-staging.fly.dev/api/hello
   ```

2. **Verify Environment Variables**
   ```bash
   echo $API_URL
   echo $FRONTEND_URL
   ```

3. **Run with Verbose Output**
   - Check the test output for specific failures
   - Each test shows its HTTP response code and body

### Tests Failing in CI/CD

1. **Check Deployment Logs** - Verify the deployment completed successfully
2. **Check Application Logs** - Look for errors in the application logs
3. **Increase Wait Time** - The deployment might need more time to stabilize
4. **Check Database Migrations** - Ensure migrations completed successfully

### Common Issues

**Issue: API not responding**
- Solution: Increase `MAX_RETRIES` or `RETRY_DELAY`
- The Fly.io instances may need time to wake up from sleep

**Issue: CORS errors**
- Solution: Check CORS configuration in backend `Program.cs`
- Verify frontend URL is whitelisted

**Issue: 401/403 errors**
- Solution: Check authentication/authorization configuration
- Verify API keys or tokens are configured correctly

## Best Practices

1. **Keep Tests Simple** - Smoke tests should be fast and focused
2. **Test Critical Paths** - Focus on essential functionality
3. **Avoid External Dependencies** - Don't rely on third-party services if possible
4. **Make Tests Idempotent** - Tests should be repeatable without side effects
5. **Clear Error Messages** - Make failures easy to diagnose

## Related Documentation

- [CI/CD Pipeline Documentation](../.github/workflows/README.md)
- [Deployment Guide](../DOCKER.md)
- [Backend README](../backend/README.md)
- [Frontend README](../frontend/README.md)
