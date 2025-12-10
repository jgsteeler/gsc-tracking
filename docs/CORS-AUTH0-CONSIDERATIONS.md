# CORS and Auth0 Wildcard Considerations for Deploy Previews

**Version:** 1.0  
**Last Updated:** 2025-12-10  
**Audience:** Developers implementing Netlify deploy previews with Auth0

---

## Overview

When using Netlify deploy previews as staging environments, you may encounter challenges with CORS (Cross-Origin Resource Sharing) and Auth0 configuration due to dynamic preview URLs. This document addresses these concerns based on real-world experience.

---

## The Wildcard Problem

### What's the Issue?

Deploy preview URLs follow this pattern:
```
https://deploy-preview-{PR-NUMBER}--{SITE-NAME}.netlify.app
```

**Examples:**
- `https://deploy-preview-42--gsc-tracking-ui.netlify.app`
- `https://deploy-preview-43--gsc-tracking-ui.netlify.app`
- `https://deploy-preview-123--gsc-tracking-ui.netlify.app`

Each PR gets a **unique number**, making it impossible to pre-register all URLs. This creates problems for:

1. **Backend CORS Configuration** - Need to allow requests from unknown preview URLs
2. **Auth0 Callback URLs** - Must register URLs before authentication works
3. **Auth0 Allowed Origins** - Must whitelist origins for cross-origin authentication

### Why Wildcards Seem Like the Solution

The intuitive solution is to use wildcards:
```
https://deploy-preview-*--gsc-tracking-ui.netlify.app  ❌ Often doesn't work
https://*.gsc-tracking-ui.netlify.app                  ❌ May not work as expected
```

**But this causes problems:**
- Auth0 has **limited wildcard support** (restrictions on subdomain wildcards)
- Backend CORS implementations vary in wildcard support
- Security risks with overly permissive wildcards
- Maintenance challenges (wildcards can be too broad)

---

## Solution Strategies

### Strategy 1: Explicit URL Registration (Most Secure)

**Approach:** Manually register each deploy preview URL as needed.

**For CORS (Backend):**
```csharp
// backend/GscTracking.Api/Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            // Local development
            "http://localhost:5173",
            
            // Production
            "https://gsc-tracking-ui.netlify.app",
            
            // Staging (shared)
            "https://staging--gsc-tracking-ui.netlify.app",
            
            // Add specific deploy preview URLs as needed
            "https://deploy-preview-42--gsc-tracking-ui.netlify.app",
            "https://deploy-preview-43--gsc-tracking-ui.netlify.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

**For Auth0:**

In Auth0 Dashboard → Applications → Your App → Settings:

1. **Allowed Callback URLs:**
   ```
   http://localhost:5173/callback,
   https://gsc-tracking-ui.netlify.app/callback,
   https://staging--gsc-tracking-ui.netlify.app/callback,
   https://deploy-preview-42--gsc-tracking-ui.netlify.app/callback
   ```

2. **Allowed Web Origins:**
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app,
   https://deploy-preview-42--gsc-tracking-ui.netlify.app
   ```

**Pros:**
- ✅ Most secure (explicit allow list)
- ✅ Works reliably
- ✅ No wildcard issues

**Cons:**
- ❌ Manual work for each PR
- ❌ Need to update backend/Auth0 for each preview
- ❌ Not scalable for many concurrent PRs

**When to Use:**
- Early development with few PRs
- High security requirements
- Auth0 won't allow wildcards

---

### Strategy 2: Dedicated Staging Environment (Recommended)

**Approach:** Use a single, persistent staging URL instead of preview URLs for Auth0 testing.

**Setup:**

1. **Create a dedicated staging deploy:**
   - Use a long-lived branch: `staging`
   - Or use branch deploys: `https://staging--gsc-tracking-ui.netlify.app`

2. **Configure Auth0 for staging only:**
   ```
   Allowed Callback URLs:
   - http://localhost:5173/callback
   - https://gsc-tracking-ui.netlify.app/callback
   - https://staging--gsc-tracking-ui.netlify.app/callback
   
   Allowed Web Origins:
   - http://localhost:5173
   - https://gsc-tracking-ui.netlify.app
   - https://staging--gsc-tracking-ui.netlify.app
   ```

3. **Use deploy previews for non-Auth0 testing:**
   - Preview URLs test UI/UX changes
   - Preview URLs test API calls (without authentication)
   - Staging environment tests full authentication flow

**Pros:**
- ✅ Stable URL for Auth0 configuration
- ✅ No wildcard needed
- ✅ Deploy previews still useful for most testing
- ✅ One-time Auth0 setup

**Cons:**
- ❌ Can't test Auth0 flows in deploy previews
- ❌ Need to merge to staging branch for Auth0 testing

**When to Use:**
- When Auth0 is implemented ✅ (Recommended)
- Multiple team members testing concurrently
- Wildcards don't work in your environment

---

### Strategy 3: Dynamic CORS with Pattern Matching (Advanced)

**Approach:** Implement custom CORS logic to match Netlify preview patterns.

**Backend Implementation:**
```csharp
// backend/GscTracking.Api/Program.cs
using System.Text.RegularExpressions;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                // Allow localhost
                if (origin.StartsWith("http://localhost")) return true;
                
                // Allow production
                if (origin == "https://gsc-tracking-ui.netlify.app") return true;
                
                // Allow staging
                if (origin == "https://staging--gsc-tracking-ui.netlify.app") return true;
                
                // Allow Netlify deploy previews with pattern matching
                var netlifyPreviewPattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";
                if (Regex.IsMatch(origin, netlifyPreviewPattern)) return true;
                
                return false;
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

**Pros:**
- ✅ Automatically allows all deploy preview URLs
- ✅ No manual URL registration
- ✅ Scales to unlimited PRs
- ✅ Secure (pattern-based, not open wildcard)

**Cons:**
- ❌ More complex implementation
- ❌ Doesn't solve Auth0 wildcard issue
- ❌ Requires backend code changes

**When to Use:**
- Backend CORS is the bottleneck ✅ (Recommended)
- Many concurrent PRs
- Team comfortable with regex patterns

---

### Strategy 4: Netlify Identity or JWT (Auth0 Alternative)

**Approach:** Use Netlify's built-in identity service for preview environments.

**Setup:**

1. Use Auth0 for production
2. Use Netlify Identity for deploy previews/staging
3. Configure different auth providers per environment

**In frontend code:**
```typescript
// src/config/auth.ts
const getAuthConfig = () => {
  const hostname = window.location.hostname;
  
  // Production: Use Auth0
  if (hostname === 'gsc-tracking-ui.netlify.app') {
    return {
      provider: 'auth0',
      domain: process.env.VITE_AUTH0_DOMAIN,
      clientId: process.env.VITE_AUTH0_CLIENT_ID
    };
  }
  
  // Staging/Previews: Use Netlify Identity
  if (hostname.includes('netlify.app')) {
    return {
      provider: 'netlify-identity',
      siteUrl: window.location.origin
    };
  }
  
  // Development: Use Auth0 dev tenant
  return {
    provider: 'auth0',
    domain: process.env.VITE_AUTH0_DEV_DOMAIN,
    clientId: process.env.VITE_AUTH0_DEV_CLIENT_ID
  };
};
```

**Pros:**
- ✅ No Auth0 wildcard issues
- ✅ Works with all preview URLs
- ✅ Netlify Identity included free

**Cons:**
- ❌ Different auth in different environments
- ❌ May not catch Auth0-specific bugs
- ❌ Additional complexity

**When to Use:**
- Auth0 wildcards are a blocker
- Preview testing is critical
- Comfortable managing multiple auth providers

---

## Recommended Approach for GSC Tracking

### Phase 1: Pre-Auth0 (Current)

**CORS Strategy:** Dynamic pattern matching (Strategy 3)

```csharp
// Use regex pattern matching for CORS
policy.SetIsOriginAllowed(origin =>
{
    if (origin.StartsWith("http://localhost")) return true;
    if (origin == "https://gsc-tracking-ui.netlify.app") return true;
    if (origin == "https://staging--gsc-tracking-ui.netlify.app") return true;
    
    var netlifyPreviewPattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";
    return Regex.IsMatch(origin, netlifyPreviewPattern);
});
```

**Pros:**
- ✅ Works for all deploy previews
- ✅ No manual URL management
- ✅ Secure pattern-based matching

### Phase 2: After Auth0 Implementation

**Auth0 Strategy:** Dedicated staging environment (Strategy 2)

1. **Create staging branch deploy:**
   - URL: `https://staging--gsc-tracking-ui.netlify.app`
   - Persistent, doesn't change

2. **Configure Auth0:**
   - Production: `https://gsc-tracking-ui.netlify.app`
   - Staging: `https://staging--gsc-tracking-ui.netlify.app`
   - Local: `http://localhost:5173`

3. **Use deploy previews for non-auth testing:**
   - UI changes
   - Non-authenticated API calls
   - General functionality

4. **Use staging for auth testing:**
   - Login/logout flows
   - Protected routes
   - Token refresh
   - User management

**Pros:**
- ✅ Best of both worlds
- ✅ Deploy previews work for most testing
- ✅ Stable URL for Auth0
- ✅ No wildcard issues

---

## Implementation Plan

### Step 1: Update Backend CORS (Do Now)

Add pattern matching to backend CORS:

```bash
# Edit backend/GscTracking.Api/Program.cs
# Add regex-based origin validation
# Test with current deploy previews
```

### Step 2: Create Staging Branch (Do Before Auth0)

```bash
# Create staging branch
git checkout -b staging
git push origin staging

# Configure in Netlify:
# Site settings → Continuous Deployment → Deploy contexts
# Enable "Branch deploys" → Select "staging" branch
```

### Step 3: Configure Auth0 (When Implementing Auth0)

**In Auth0 Dashboard:**

1. Applications → GSC Tracking → Settings
2. Add allowed URLs (NO wildcards):
   ```
   http://localhost:5173/callback,
   https://gsc-tracking-ui.netlify.app/callback,
   https://staging--gsc-tracking-ui.netlify.app/callback
   ```
3. Add web origins:
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

### Step 4: Document Workflow

**For team:**
- Deploy previews: Test UI/non-auth features
- Staging: Test authentication flows
- Production: Final validation

---

## Testing Checklist

Before implementing Auth0:

- [ ] **Test CORS with deploy preview**
  - [ ] Open deploy preview URL
  - [ ] Open browser console
  - [ ] Make API call (e.g., fetch customers)
  - [ ] Verify no CORS errors

- [ ] **Test with multiple previews**
  - [ ] Open 2-3 PRs
  - [ ] Verify each preview works
  - [ ] Check API calls succeed in all

- [ ] **Verify pattern matching**
  - [ ] Check backend logs
  - [ ] Confirm preview URLs are allowed
  - [ ] Confirm other URLs are blocked

After implementing Auth0:

- [ ] **Test staging authentication**
  - [ ] Visit staging URL
  - [ ] Login with Auth0
  - [ ] Access protected routes
  - [ ] Verify token refresh

- [ ] **Verify deploy preview limitations**
  - [ ] Open deploy preview
  - [ ] Attempt Auth0 login
  - [ ] Expect failure (URL not registered)
  - [ ] Document for team

- [ ] **Test production authentication**
  - [ ] Login on production
  - [ ] Verify all auth flows
  - [ ] Check token handling

---

## Troubleshooting

### CORS Error in Deploy Preview

**Error:** "CORS policy: No 'Access-Control-Allow-Origin' header"

**Solutions:**

1. **Check backend logs** - Verify origin is being checked
2. **Test regex pattern** - Ensure it matches preview URL format
3. **Verify backend deployment** - Ensure latest code with pattern matching is deployed
4. **Check URL format** - Ensure it matches expected pattern

**Quick test:**
```bash
# Test from command line
curl -H "Origin: https://deploy-preview-42--gsc-tracking-ui.netlify.app" \
     -H "Access-Control-Request-Method: GET" \
     -X OPTIONS \
     https://gsc-tracking-api.fly.dev/api/customers
```

### Auth0 Login Fails in Preview

**Error:** "The redirect URI is not in the allowed list"

**Expected:** This is normal if URL is not registered in Auth0

**Solutions:**

1. **Use staging environment** - `https://staging--gsc-tracking-ui.netlify.app`
2. **Add specific preview URL** - Register in Auth0 for this PR
3. **Accept limitation** - Document that auth testing requires staging

### Backend Pattern Not Matching

**Error:** CORS still failing despite regex pattern

**Debug:**

1. **Add logging:**
   ```csharp
   policy.SetIsOriginAllowed(origin =>
   {
       Console.WriteLine($"CORS Check: {origin}");
       // ... pattern matching logic
   });
   ```

2. **Check actual origin** - Browser dev tools → Network → Request headers
3. **Test regex online** - https://regex101.com/
4. **Verify deployment** - Ensure backend has latest code

---

## Security Considerations

### Don't Use Overly Broad Wildcards

❌ **Bad:**
```csharp
// Allows ANY netlify.app subdomain
policy.WithOrigins("https://*.netlify.app")
```

❌ **Bad:**
```csharp
// Allows ANY domain
policy.AllowAnyOrigin()
```

✅ **Good:**
```csharp
// Specific pattern for your site only
var pattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";
```

### Validate Origin Format

Always validate origin format:
- Check protocol (https only in production)
- Check domain matches your site
- Check pattern is specific (not just `*.netlify.app`)
- Log rejected origins for monitoring

### Monitor CORS Requests

Set up monitoring:
- Log all CORS validation attempts
- Alert on repeated failures
- Review logs for suspicious origins
- Update patterns if legitimate requests blocked

---

## Additional Resources

- **MDN CORS Guide:** https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
- **Auth0 Wildcard Docs:** https://auth0.com/docs/get-started/applications/application-settings#urls
- **Netlify Deploy Contexts:** https://docs.netlify.com/site-deploys/overview/#deploy-contexts
- **.NET CORS Docs:** https://learn.microsoft.com/en-us/aspnet/core/security/cors

---

## Summary

**Current State (No Auth0):**
- ✅ Use dynamic CORS pattern matching
- ✅ Deploy previews work for all testing
- ✅ No manual URL management

**Future State (With Auth0):**
- ✅ Keep CORS pattern matching
- ✅ Add dedicated staging environment
- ✅ Use staging for auth testing
- ✅ Use previews for non-auth testing

**Key Takeaway:** Don't try to make wildcards work with Auth0. Use a dedicated staging environment instead.

---

**Questions?** Open an issue or consult Auth0 support for specific wildcard limitations.
