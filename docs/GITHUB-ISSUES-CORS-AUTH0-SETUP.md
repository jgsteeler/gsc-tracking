# GitHub Issues for CORS and Auth0 Setup

Copy and paste these into GitHub Issues to track the implementation.

---

## Issue 1: Implement Pre-Auth0 CORS Configuration with Pattern Matching

**Title:** `[INFRA] Implement pre-Auth0 CORS configuration with pattern matching`

**Labels:** `infrastructure`, `backend`, `deployment`

**Description:**

### Description

Implement CORS configuration in the backend API to support Netlify deploy previews before Auth0 is implemented. Use pattern-based origin validation instead of wildcards to allow all deploy preview URLs dynamically.

### Objectives

- Configure CORS to allow production, staging, and all deploy preview URLs
- Use regex pattern matching for deploy preview validation
- Support configurable localhost ports via app settings
- Ensure security with specific pattern matching (no broad wildcards)

### Acceptance Criteria

- [ ] CORS policy implemented in `backend/GscTracking.Api/Program.cs`
- [ ] Pattern matching validates deploy preview URLs: `https://deploy-preview-\d+--gsc-tracking-ui\.netlify\.app`
- [ ] Production URL allowed: `https://gsc-tracking-ui.netlify.app`
- [ ] Staging URL allowed: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] Localhost ports configurable via comma-separated app setting (e.g., `"5173,5174,3000"`)
- [ ] CORS policy allows credentials, any method, any header
- [ ] Tested with deploy preview URLs from actual PRs
- [ ] Documentation updated in README or deployment docs

### Technical Details

**Configuration approach:**

```csharp
// In appsettings.json or appsettings.Development.json
{
  "AllowedLocalPorts": "5173,5174,3000"  // Comma-separated list of allowed localhost ports
}

// In Program.cs
using System.Text.RegularExpressions;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedLocalPorts = builder.Configuration["AllowedLocalPorts"]?.Split(',') ?? new[] { "5173" };
        
        policy.SetIsOriginAllowed(origin =>
        {
            // Allow configured localhost ports
            foreach (var port in allowedLocalPorts)
            {
                if (origin == $"http://localhost:{port.Trim()}")
                    return true;
            }
            
            // Allow production
            if (origin == "https://gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow staging
            if (origin == "https://staging--gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow Netlify deploy previews with pattern matching
            var netlifyPreviewPattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";
            if (Regex.IsMatch(origin, netlifyPreviewPattern))
                return true;
            
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Apply CORS
app.UseCors("AllowFrontend");
```

**Configuration in appsettings:**

```json
{
  "AllowedLocalPorts": "5173,5174,3000",
  "Logging": {
    // ... existing config
  }
}
```

### Category

Authentication/Security

### Priority

High

### Dependencies

- Backend deployment to Fly.io (already complete)
- Netlify frontend deployment configuration (already complete)

### Additional Context

- See `docs/CORS-AUTH0-CONSIDERATIONS.md` for detailed strategy
- This is Phase 1 (pre-Auth0) implementation
- Pattern matching ensures security without requiring manual URL registration for each PR
- Localhost port configuration allows different developers to use different ports without code changes

### Testing Plan

1. Deploy backend with new CORS configuration
2. Test from localhost on configured port (e.g., 5173)
3. Test from localhost on non-configured port (should fail)
4. Create a test PR to generate deploy preview
5. Test API calls from deploy preview URL (should succeed)
6. Test from production URL (should succeed)
7. Test from staging URL (should succeed)
8. Verify CORS errors for non-allowed origins

---

## Issue 2: Implement Post-Auth0 CORS and Auth0 Configuration

**Title:** `[INFRA] Implement post-Auth0 CORS and Auth0 configuration with dedicated staging`

**Labels:** `infrastructure`, `backend`, `authentication`, `deployment`

**Description:**

### Description

Implement Auth0 authentication configuration and update CORS policy to work with Auth0. Create dedicated staging environment for Auth0 testing (since wildcards don't work reliably with Auth0).

### Objectives

- Configure Auth0 application settings with specific allowed URLs (no wildcards)
- Update CORS configuration to work with Auth0 requirements
- Create dedicated staging branch deployment for Auth0 testing
- Maintain pattern-based CORS for deploy previews (non-auth testing)
- Document Auth0 testing workflow for team

### Acceptance Criteria

- [ ] Auth0 application configured with allowed callback URLs
- [ ] Auth0 application configured with allowed web origins
- [ ] Auth0 application configured with allowed logout URLs
- [ ] Dedicated staging branch created: `staging`
- [ ] Staging branch deploys to: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] CORS configuration updated for Auth0 compatibility
- [ ] Auth0 environment variables configured in backend
- [ ] Auth0 environment variables configured in Netlify (production and staging only)
- [ ] Login/logout tested on production
- [ ] Login/logout tested on staging
- [ ] Deploy previews documented as non-auth testing only
- [ ] Team documentation updated with Auth0 workflow

### Technical Details

**Auth0 Application Configuration:**

In Auth0 Dashboard → Applications → GSC Tracking → Settings:

1. **Allowed Callback URLs:**
   ```
   http://localhost:5173/callback,
   https://gsc-tracking-ui.netlify.app/callback,
   https://staging--gsc-tracking-ui.netlify.app/callback
   ```

2. **Allowed Logout URLs:**
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

3. **Allowed Web Origins:**
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

**Note:** Do NOT use wildcards like `https://deploy-preview-*--gsc-tracking-ui.netlify.app` as they may not work reliably.

**Backend CORS Update:**

Keep existing CORS configuration from Issue #1, no changes needed. The pattern-based approach already works for non-authenticated API calls.

**Backend Auth0 Configuration:**

```csharp
// In appsettings.json
{
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "Audience": "https://your-api-audience"
  },
  "AllowedLocalPorts": "5173,5174,3000"
}

// In Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
    });

app.UseAuthentication();
app.UseAuthorization();
```

**Frontend Auth0 Configuration (Netlify):**

In `netlify.toml`:
```toml
[context.production.environment]
  VITE_API_URL = "https://gsc-tracking-api.fly.dev/api"
  VITE_AUTH0_DOMAIN = "your-domain.auth0.com"
  VITE_AUTH0_CLIENT_ID = "your-production-client-id"
  VITE_AUTH0_AUDIENCE = "https://your-api-audience"

[context.deploy-preview.environment]
  VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
  # Deploy previews: No Auth0 config (not registered in Auth0)

# Add staging context for Auth0 testing
[context.staging.environment]
  VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
  VITE_AUTH0_DOMAIN = "your-domain.auth0.com"
  VITE_AUTH0_CLIENT_ID = "your-staging-client-id"
  VITE_AUTH0_AUDIENCE = "https://your-api-audience"
```

**Staging Branch Setup:**

```bash
# Create staging branch
git checkout -b staging main
git push origin staging

# Configure in Netlify Dashboard:
# Site settings → Continuous Deployment → Deploy contexts
# Enable "Branch deploys" → Select "staging" branch
```

### Category

Authentication/Security

### Priority

High

### Dependencies

- Issue #1 (Pre-Auth0 CORS configuration) must be complete
- Auth0 account and application created
- Backend and frontend deployed

### Additional Context

- See `docs/CORS-AUTH0-CONSIDERATIONS.md` for detailed strategy (Strategy 2)
- Deploy previews will NOT have Auth0 configured (URLs not registered)
- Use deploy previews for: UI testing, non-auth API calls, general functionality
- Use staging for: Login/logout flows, protected routes, token handling, full auth testing
- Production for: Final validation before release

**Testing Workflow:**

1. **Deploy Previews** (non-auth testing):
   - Test UI changes
   - Test non-protected API endpoints
   - Test general functionality
   - Fast iteration, no auth setup needed

2. **Staging Branch** (auth testing):
   - Test login/logout
   - Test protected routes
   - Test token refresh
   - Test user management

3. **Production**:
   - Final validation
   - Monitor for auth issues

### Rollback Plan

If Auth0 causes issues:

1. Temporarily disable Auth0 in backend (comment out authentication middleware)
2. Remove `[Authorize]` attributes from controllers
3. Deploy updated backend
4. Continue with non-auth testing
5. Debug Auth0 configuration separately

### Testing Plan

**Phase 1: Localhost**
1. Configure Auth0 for localhost:5173
2. Test login on localhost
3. Test protected API calls
4. Test logout

**Phase 2: Staging**
1. Merge staging branch with Auth0 config
2. Deploy to `https://staging--gsc-tracking-ui.netlify.app`
3. Test login on staging
4. Test protected API calls on staging backend
5. Test logout on staging

**Phase 3: Production**
1. Merge to main with Auth0 config
2. Deploy to production
3. Test login on production
4. Test protected API calls on production backend
5. Test logout on production
6. Monitor for auth errors

**Phase 4: Verify Deploy Previews**
1. Create test PR
2. Open deploy preview URL
3. Verify Auth0 login fails (expected - URL not registered)
4. Verify non-auth functionality works
5. Document limitation for team

---

## Issue 3: Create Staging Branch for Auth0 Testing

**Title:** `[INFRA] Create dedicated staging branch for Auth0 testing`

**Labels:** `infrastructure`, `deployment`, `authentication`

**Description:**

### Description

Create a dedicated `staging` branch that deploys to a persistent staging URL (`https://staging--gsc-tracking-ui.netlify.app`) for Auth0 testing. This avoids wildcard issues with Auth0 configuration.

### Objectives

- Create `staging` branch from `main`
- Configure Netlify to deploy `staging` branch
- Document staging workflow for team
- Keep staging in sync with main for testing

### Acceptance Criteria

- [ ] `staging` branch created and pushed to GitHub
- [ ] Netlify configured to deploy `staging` branch
- [ ] Staging deploys to: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] Backend staging points to: `https://gsc-tracking-api-staging.fly.dev`
- [ ] Staging workflow documented for team
- [ ] Process for keeping staging updated from main

### Technical Details

**Create Staging Branch:**

```bash
# From main branch
git checkout main
git pull origin main

# Create staging branch
git checkout -b staging
git push origin staging
```

**Configure Netlify:**

1. Go to Netlify dashboard: `https://app.netlify.com/sites/gsc-tracking-ui`
2. Navigate to: **Site settings** → **Build & deploy** → **Continuous deployment**
3. Under **Deploy contexts** → **Branch deploys**
4. Select: **"Let me add individual branches"**
5. Add branch: `staging`
6. Save changes

**Verify Configuration:**

The existing `netlify.toml` should automatically handle the staging context, but verify:

```toml
[context.staging.environment]
  VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
```

If not present, add it after the `[context.deploy-preview]` section.

**Keeping Staging Updated:**

```bash
# Periodically sync staging with main
git checkout staging
git merge main
git push origin staging

# Or rebase to keep history clean
git checkout staging
git rebase main
git push origin staging --force-with-lease
```

### Category

Hosting/Deployment

### Priority

Medium (Do before Auth0 implementation)

### Dependencies

- Netlify site already deployed (complete)
- Backend staging environment exists (complete)

### Additional Context

- Staging branch is persistent (doesn't auto-delete like deploy previews)
- Staging URL is stable, perfect for Auth0 registration
- Staging can be used for other long-term testing needs
- Consider protecting the staging branch in GitHub (require reviews)

### Testing Plan

1. Create `staging` branch
2. Push to GitHub
3. Wait for Netlify to deploy (check dashboard)
4. Visit `https://staging--gsc-tracking-ui.netlify.app`
5. Verify it loads correctly
6. Verify it uses staging backend (check Network tab)
7. Make a small change on staging branch
8. Push and verify auto-deploy works

---

## Implementation Order

**Recommended sequence:**

1. **Issue #3** (Create Staging Branch) - Do first, independent of code changes
2. **Issue #1** (Pre-Auth0 CORS) - Implement before Auth0, test with deploy previews
3. **Issue #2** (Post-Auth0 Setup) - Implement when ready for Auth0

**Timeline estimate:**
- Issue #3: 30 minutes (setup only)
- Issue #1: 2-3 hours (implementation + testing)
- Issue #2: 4-6 hours (Auth0 setup + configuration + testing)

**Total:** ~7-10 hours spread across 3 issues

---

## Notes

- All issues reference `docs/CORS-AUTH0-CONSIDERATIONS.md` for detailed strategy
- Localhost ports configurable via `AllowedLocalPorts` app setting
- Pattern-based CORS ensures security without manual URL management
- Dedicated staging avoids Auth0 wildcard limitations
- Deploy previews remain useful for non-auth testing
