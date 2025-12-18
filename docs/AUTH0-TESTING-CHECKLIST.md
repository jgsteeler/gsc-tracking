# Auth0 Testing Checklist

This checklist helps verify that Auth0 authentication is working correctly after configuration.

## Prerequisites

Before testing, ensure:

- [ ] Auth0 tenant is created
- [ ] Auth0 API is created with audience configured
- [ ] Auth0 Application (SPA) is created
- [ ] Callback URLs, Logout URLs, and Web Origins are configured in Auth0
- [ ] Backend Auth0 environment variables are set (Domain, Audience)
- [ ] Frontend Auth0 environment variables are set (Domain, Client ID, Audience)

## Backend Testing

### 1. Verify Backend Starts Without Errors

```bash
cd backend/GscTracking.Api
dotnet run
```

Expected: Application starts successfully. Check console output:

- ✅ No Auth0-related errors
- ✅ If Auth0 is configured, no "Auth0 is not configured" warning
- ✅ If Auth0 is not configured, see "WARNING: Auth0 is not configured. Authentication will be disabled."

### 2. Test Swagger Documentation

1. Navigate to: `http://localhost:5091/swagger` (or your backend URL)
2. Expected:
   - ✅ Swagger UI loads successfully
   - ✅ If Auth0 is configured, see "Authorize" button in top-right
   - ✅ OAuth2 authentication options available

### 3. Test Public Endpoints (No Auth Required)

```bash
# Test hello endpoint
curl http://localhost:5091/api/hello

# Test customers endpoint (if not protected)
curl http://localhost:5091/api/customers
```

Expected:

- ✅ Endpoints return data successfully
- ✅ No 401 Unauthorized errors (if endpoints are not protected yet)

### 4. Test Protected Endpoints (After Adding [Authorize])

After adding `[Authorize]` to a controller:

```bash
# Without token - should fail
curl http://localhost:5091/api/customers

# Expected: 401 Unauthorized
```

## Frontend Testing

### 1. Verify Frontend Starts Without Errors

```bash
cd frontend
npm run dev
```

Expected:

- ✅ Application starts on `http://localhost:5173`
- ✅ No Auth0-related errors in browser console
- ✅ If Auth0 is not configured, app works normally (no auth UI)

### 2. Test Login Flow

1. Open `http://localhost:5173` in browser
2. Check sidebar or navigation:
   - ✅ "Log In" button is visible (if Auth0 is configured)
   - ✅ No "Log In" button if Auth0 is not configured
3. Click "Log In" button
4. Expected:
   - ✅ Redirected to Auth0 Universal Login page
   - ✅ Auth0 domain matches your tenant
   - ✅ Application name shown on login page
5. Enter credentials or sign up
6. Expected:
   - ✅ Redirected back to application
   - ✅ User profile picture appears in sidebar
   - ✅ User name/email appears in sidebar
   - ✅ "Log Out" button appears
   - ✅ "Log In" button disappears

### 3. Test Authenticated State

After logging in:

1. Check browser console:
   - ✅ No Auth0-related errors
   - ✅ Access token is present (check Auth0 SDK state)
2. Check Local Storage:
   - Open Developer Tools → Application → Local Storage
   - ✅ Auth0 keys present (e.g., `@@auth0spajs@@::...`)
3. Navigate between pages:
   - ✅ User remains logged in
   - ✅ Profile info persists

### 4. Test Logout Flow

1. Click "Log Out" button
2. Expected:
   - ✅ User is logged out
   - ✅ Profile info disappears
   - ✅ "Log In" button reappears
   - ✅ "Log Out" button disappears
   - ✅ Local Storage Auth0 keys are cleared

### 5. Test Protected Routes (When Implemented)

If using `ProtectedRoute` component:

1. Log out
2. Try to access a protected route directly
3. Expected:
   - ✅ Redirected to login
   - ✅ After login, redirected back to the intended page

### 6. Test API Calls with Authentication

When using `useAccessToken` hook:

1. Log in
2. Make an API call to a protected endpoint
3. Check Network tab in Developer Tools:
   - ✅ Request includes `Authorization: Bearer <token>` header
   - ✅ Request succeeds (200 OK)
4. Log out
5. Make the same API call:
   - ✅ No Authorization header (or request fails with 401)

## Environment-Specific Testing

### Local Development

- [ ] Backend running on `http://localhost:5091`
- [ ] Frontend running on `http://localhost:5173`
- [ ] Auth0 callback URL includes `http://localhost:5173`
- [ ] Login/logout works correctly

### Staging (Netlify)

- [ ] Backend deployed to `https://gsc-tracking-api-staging.fly.dev`
- [ ] Frontend deployed to `https://staging--gsc-tracking-ui.netlify.app`
- [ ] Auth0 callback URL includes staging URL
- [ ] Auth0 environment variables set in Netlify (for staging context)
- [ ] Login/logout works correctly on staging

### Production (Netlify)

- [ ] Backend deployed to `https://gsc-tracking-api.fly.dev`
- [ ] Frontend deployed to `https://gsc-tracking-ui.netlify.app`
- [ ] Auth0 callback URL includes production URL
- [ ] Auth0 environment variables set in Netlify (for production context)
- [ ] Login/logout works correctly on production

## Common Issues and Solutions

### Issue: "Invalid state" error on callback

**Cause:** Redirect URI mismatch or cookie issues

**Solution:**

1. Verify redirect URI in Auth0 matches exactly (including protocol and port)
2. Clear browser cookies and Local Storage
3. Try login again

### Issue: 401 Unauthorized on API calls

**Cause:** Token not being sent or invalid

**Solution:**

1. Check if user is logged in (token should be present)
2. Verify `Authorization` header is being sent
3. Check backend logs for JWT validation errors
4. Verify Auth0 audience matches between frontend and backend

### Issue: CORS errors

**Cause:** Origin not allowed by backend or Auth0

**Solution:**

1. Verify frontend origin is in Auth0's Allowed Web Origins
2. Verify backend CORS policy includes frontend origin
3. Check both backend and Auth0 dashboard settings

### Issue: "Login required" on deploy previews

**Cause:** Deploy preview URLs not registered in Auth0

**Solution:**

- This is expected behavior
- Use staging branch for Auth0 testing
- Deploy previews are for non-authenticated testing only

### Issue: User profile not showing

**Cause:** Scopes not requested or granted

**Solution:**

1. Verify `openid`, `profile`, `email` scopes in Auth0Provider config
2. Check Auth0 application settings for enabled scopes
3. Re-login to grant new scopes

## Security Verification

### Backend Security

- [ ] Auth0 domain and audience are set via environment variables (not hardcoded)
- [ ] JWT tokens are validated on protected endpoints
- [ ] Sensitive endpoints have `[Authorize]` attribute
- [ ] HTTPS is used in production

### Frontend Security

- [ ] Auth0 credentials stored in environment variables (not in code)
- [ ] No tokens stored in LocalStorage manually (Auth0 SDK handles this)
- [ ] Logout clears all authentication state
- [ ] HTTPS is used in production

### Auth0 Dashboard Security

- [ ] Callback URLs are exact matches (no wildcards in production)
- [ ] Only necessary origins are allowed
- [ ] Application type is set to "Single Page Application"
- [ ] Token expiration is configured appropriately
- [ ] Consider enabling MFA for production

## Performance Verification

- [ ] Login redirect is fast (< 2 seconds)
- [ ] Token refresh happens silently (no page reload)
- [ ] API calls with tokens don't significantly increase latency
- [ ] Auth0 Universal Login page loads quickly

## Accessibility Verification

- [ ] Login/Logout buttons are keyboard accessible
- [ ] Screen readers can announce login state
- [ ] Auth0 Universal Login page is accessible (Auth0's responsibility)

## Final Checklist

Before considering Auth0 setup complete:

- [ ] All backend tests pass
- [ ] All frontend tests pass (when written)
- [ ] Login flow tested on all environments (local, staging, production)
- [ ] Logout flow tested on all environments
- [ ] Protected endpoints return 401 when not authenticated
- [ ] Protected endpoints work when authenticated
- [ ] Documentation is up to date
- [ ] Team members can log in successfully
- [ ] No console errors related to Auth0
- [ ] Security best practices followed

## Support

If issues persist:

1. Check [docs/AUTH0-SETUP.md](./AUTH0-SETUP.md) for detailed setup instructions
2. Review [Auth0 Documentation](https://auth0.com/docs)
3. Check Auth0 Dashboard Logs: Monitoring → Logs
4. Verify environment variables are set correctly

---

**Last Updated**: 2025-12-16
