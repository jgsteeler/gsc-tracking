# Adding Staging Deploy Previews to Existing Netlify Site

**For:** Sites already deployed on Netlify (e.g., `gsc-tracking-ui.netlify.app`)  
**Goal:** Add deploy preview staging without disrupting production  
**Version:** 1.0  
**Last Updated:** 2025-12-10

---

## Overview

If you already have a Netlify site deployed (like `gsc-tracking-ui`), you can add staging deploy previews without creating a new site or disrupting production deployments.

---

## Quick Steps

### 1. Add `netlify.toml` to Repository

Copy the `netlify.toml` file to your repository root:

```bash
# If the file exists in this repo, copy it
cp /path/to/netlify.toml /path/to/your/repo/

# Or create it manually (see configuration below)
```

### 2. Update Existing Site Settings

**In Netlify Dashboard:**

1. Go to your site: `https://app.netlify.com/sites/gsc-tracking-ui`
2. Navigate to **Site settings** → **Build & deploy** → **Continuous deployment**

**Build Settings:**
- Base directory: `frontend` (if not already set)
- Build command: `npm run build`
- Publish directory: `frontend/dist`

### 3. Enable Deploy Previews

**In the same Continuous Deployment section:**

1. Scroll to **Deploy contexts**
2. Under **Deploy previews**, select:
   - **"Any pull request against your production branch (main)"**
3. Click **"Save"**

### 4. Add Environment Variables

**Site settings** → **Environment variables** → **Add a variable**

Add variables for different contexts:

**For Production:**
- Scope: **Production**
- Key: `VITE_API_URL`
- Value: `https://gsc-tracking-api.fly.dev/api`

**For Deploy Previews (Staging):**
- Scope: **Deploy previews**
- Key: `VITE_API_URL`
- Value: `https://gsc-tracking-api-staging.fly.dev/api`

**Note:** If you already have `netlify.toml` with these settings, the file takes precedence. You can skip manual environment variable setup.

### 5. Test Deploy Preview

Create a test PR:

```bash
git checkout -b test/deploy-preview
echo "<!-- Test deploy preview -->" >> frontend/src/App.tsx
git add .
git commit -m "test: verify deploy preview workflow"
git push origin test/deploy-preview
```

Open a PR on GitHub, and Netlify will:
- Build and deploy a preview
- Post a comment with the preview URL
- Update the preview on each push to the PR

---

## Configuration File (`netlify.toml`)

Add this file to your repository root:

```toml
# Netlify Configuration for GSC Tracking Frontend
[build]
  base = "frontend"
  command = "npm run build"
  publish = "dist"
  
  [build.environment]
    NODE_VERSION = "20"

# Production branch configuration
[context.production]
  command = "npm run build"
  [context.production.environment]
    VITE_API_URL = "https://gsc-tracking-api.fly.dev/api"

# Deploy Preview configuration (for PRs - staging environment)
[context.deploy-preview]
  command = "npm run build"
  [context.deploy-preview.environment]
    VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"

# Branch deploy configuration (optional)
[context.branch-deploy]
  command = "npm run build"
  [context.branch-deploy.environment]
    VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"

# Redirect rules for SPA routing
[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
  force = false

# Headers for security and caching
[[headers]]
  for = "/*"
  [headers.values]
    X-Frame-Options = "DENY"
    X-Content-Type-Options = "nosniff"
    X-XSS-Protection = "1; mode=block"
    Referrer-Policy = "strict-origin-when-cross-origin"

[[headers]]
  for = "/assets/*"
  [headers.values]
    Cache-Control = "public, max-age=31536000, immutable"
```

---

## Differences from New Site Setup

| Aspect | New Site Setup | Existing Site |
|--------|---------------|---------------|
| **Site Creation** | Create new site in Netlify | Use existing site |
| **URL** | New URL assigned | Keep existing URL |
| **Configuration** | Set during import | Update in settings |
| **Environment Vars** | Add during setup | Add to existing site |
| **Production Impact** | None (new site) | None if done correctly |

---

## Verifying It Works

### Check 1: Production Still Works

1. Push a change to `main` branch
2. Verify production deploys as normal
3. Visit your production URL: `https://gsc-tracking-ui.netlify.app`
4. Confirm it works and uses production API

### Check 2: Deploy Preview Created

1. Open a PR to `main`
2. Wait 2-3 minutes
3. Check for Netlify comment on PR with preview URL
4. Visit preview URL
5. Confirm it uses staging API (check Network tab in browser dev tools)

### Check 3: Environment Variables

In browser console on preview site:

```javascript
// This should show the staging API URL
console.log(import.meta.env.VITE_API_URL)
// Should output: https://gsc-tracking-api-staging.fly.dev/api
```

On production site:

```javascript
// This should show the production API URL
console.log(import.meta.env.VITE_API_URL)
// Should output: https://gsc-tracking-api.fly.dev/api
```

---

## Common Issues

### Issue: Deploy Previews Not Created

**Cause:** Deploy previews not enabled

**Fix:**
1. **Site settings** → **Build & deploy** → **Continuous deployment**
2. Under **Deploy contexts** → **Deploy previews**
3. Select: "Any pull request against your production branch"
4. Save

### Issue: Preview Uses Wrong API

**Cause:** Environment variables not set correctly

**Fix:**
1. Check `netlify.toml` has correct `[context.deploy-preview.environment]` section
2. Or manually set in **Site settings** → **Environment variables**
3. Scope must be "Deploy previews"
4. Redeploy preview (push to PR branch)

### Issue: Production Broken After Changes

**Cause:** Configuration error in `netlify.toml`

**Fix:**
1. Check TOML syntax: https://www.toml.io/
2. Verify `[context.production]` section is correct
3. Test locally: `cd frontend && npm run build`
4. Rollback in Netlify: **Deploys** → Find last working deploy → **Publish deploy**

---

## Rollback Plan

If something goes wrong:

### Rollback Option 1: In Netlify Dashboard

1. Go to **Deploys** tab
2. Find the last working production deploy (before changes)
3. Click **"..."** → **"Publish deploy"**
4. Production immediately reverts (no rebuild)

### Rollback Option 2: Remove `netlify.toml`

1. Delete or rename `netlify.toml`
2. Commit and push to `main`
3. Site reverts to manual configuration in Netlify dashboard

### Rollback Option 3: Disable Deploy Previews

1. **Site settings** → **Build & deploy** → **Deploy contexts**
2. Under **Deploy previews**, select **"None"**
3. PRs will no longer trigger preview deploys
4. Production unaffected

---

## Best Practices

### 1. Test in a Branch First

Before merging `netlify.toml` to `main`:

1. Create a feature branch with `netlify.toml`
2. Open a PR
3. Verify preview deploys correctly
4. Verify preview uses staging API
5. Merge to `main` after confirmation

### 2. Keep Netlify Dashboard as Backup

Even with `netlify.toml`, you can override settings in Netlify dashboard:
- Environment variables in dashboard take precedence
- Build settings can be overridden
- Useful for testing without committing changes

### 3. Document Your Configuration

Add comments to `netlify.toml`:
```toml
# Updated: 2025-12-10 - Added deploy preview staging
# Production API: gsc-tracking-api.fly.dev
# Staging API: gsc-tracking-api-staging.fly.dev
```

### 4. Monitor First Few Deploys

After enabling deploy previews:
- Watch the first 2-3 PR deployments
- Check build logs for errors
- Verify API connections work
- Ask team to test preview URLs

---

## Migration Checklist

Use this checklist when adding deploy previews to existing site:

- [ ] **Backup current configuration**
  - [ ] Screenshot Netlify build settings
  - [ ] Screenshot environment variables
  - [ ] Note current production URL

- [ ] **Add `netlify.toml` to repository**
  - [ ] Copy configuration from this guide
  - [ ] Update API URLs if different
  - [ ] Commit to feature branch (not main yet)

- [ ] **Test on feature branch**
  - [ ] Open PR with `netlify.toml`
  - [ ] Verify preview deploys
  - [ ] Check preview uses staging API
  - [ ] Confirm no errors in build logs

- [ ] **Update Netlify settings**
  - [ ] Enable deploy previews in dashboard
  - [ ] Add environment variables (if not in netlify.toml)
  - [ ] Enable PR comments

- [ ] **Merge to main**
  - [ ] Get PR approval
  - [ ] Merge to main
  - [ ] Verify production still works
  - [ ] Check production uses correct API

- [ ] **Document for team**
  - [ ] Share deploy preview workflow
  - [ ] Update team documentation
  - [ ] Explain how to find preview URLs

- [ ] **Monitor and verify**
  - [ ] Test next 2-3 PRs
  - [ ] Confirm team can use previews
  - [ ] Check for any issues

---

## Additional Resources

- **Full Deployment Guide:** [NETLIFY-DEPLOYMENT.md](./NETLIFY-DEPLOYMENT.md)
- **Deploy Preview Strategy:** [DEPLOY-PREVIEW-AS-STAGING.md](./DEPLOY-PREVIEW-AS-STAGING.md)
- **CORS/Auth0 Considerations:** [CORS-AUTH0-CONSIDERATIONS.md](./CORS-AUTH0-CONSIDERATIONS.md)
- **Netlify Docs:** https://docs.netlify.com/

---

**Questions?** Open an issue or check the troubleshooting section above.
