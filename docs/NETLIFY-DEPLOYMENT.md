# Netlify Frontend Deployment Guide

**Version:** 1.0  
**Last Updated:** 2025-12-10  
**Status:** Production Ready

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Initial Setup](#initial-setup)
4. [Deploy Preview as Staging Environment](#deploy-preview-as-staging-environment)
5. [Environment Variables](#environment-variables)
6. [Custom Domain Setup](#custom-domain-setup)
7. [Deployment Workflow](#deployment-workflow)
8. [Troubleshooting](#troubleshooting)
9. [Cost Considerations](#cost-considerations)

---

## Overview

This guide explains how to deploy the GSC Tracking frontend (React + Vite) to Netlify with automatic deployments and deploy previews serving as the staging environment.

### Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     GitHub Repository                    │
│                  (jgsteeler/gsc-tracking)               │
└─────────────────────────────────────────────────────────┘
                              │
                              │ Push/PR
                              ▼
┌─────────────────────────────────────────────────────────┐
│                         Netlify                          │
├─────────────────────────────────────────────────────────┤
│  main branch → Production                               │
│  https://gsc-tracking.netlify.app                       │
│                                                          │
│  Pull Requests → Deploy Previews (Staging)             │
│  https://deploy-preview-{pr-number}--gsc-tracking...    │
└─────────────────────────────────────────────────────────┘
                              │
                              │ API Calls
                              ▼
┌─────────────────────────────────────────────────────────┐
│              Fly.io Backend (.NET API)                   │
├─────────────────────────────────────────────────────────┤
│  Production: gsc-tracking-api.fly.dev                   │
│  Staging: gsc-tracking-api-staging.fly.dev              │
└─────────────────────────────────────────────────────────┘
```

### Key Features

✅ **Zero Cost Development** - Free tier supports 100GB bandwidth/month  
✅ **No Separate Staging Branch** - Deploy previews from PRs serve as staging  
✅ **Automatic Deployments** - Push to `main` = instant production deploy  
✅ **Instant Rollbacks** - One-click rollback to any previous deployment  
✅ **Branch Previews** - Every PR gets a unique URL for testing  
✅ **Global CDN** - Fast loading worldwide with 300+ edge locations  
✅ **HTTPS by Default** - Free SSL certificates with auto-renewal

---

## Prerequisites

Before you begin, ensure you have:

- [x] GitHub repository: `jgsteeler/gsc-tracking`
- [x] Netlify account (free): https://app.netlify.com/signup
- [x] Backend API deployed to Fly.io (see [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md))
- [x] Admin access to the GitHub repository

---

## Initial Setup

> **Already have a Netlify site deployed?** See [NETLIFY-EXISTING-SITE-SETUP.md](./NETLIFY-EXISTING-SITE-SETUP.md) for instructions on adding staging deploy previews to your existing site.

### Step 1: Create Netlify Account

1. Go to https://app.netlify.com/signup
2. Click **"Sign up with GitHub"**
3. Authorize Netlify to access your GitHub repositories

### Step 2: Connect Repository

1. In Netlify dashboard, click **"Add new site"** → **"Import an existing project"**
2. Choose **"Deploy with GitHub"**
3. Select the repository: `jgsteeler/gsc-tracking`
4. Configure build settings:

   ```
   Base directory:    frontend
   Build command:     npm run build
   Publish directory: frontend/dist
   ```

5. Click **"Show advanced"** to add environment variables (see next section)
6. Click **"Deploy site"**

### Step 3: Configure Site Settings

After the initial deployment:

1. Go to **Site settings** → **General** → **Site details**
2. Click **"Change site name"** and choose a custom subdomain:
   - Suggested: `gsc-tracking` → https://gsc-tracking.netlify.app
3. Go to **Site settings** → **Build & deploy** → **Continuous deployment**
4. Verify these settings:
   - **Production branch:** `main`
   - **Deploy contexts:** All (production, deploy previews, branch deploys)

---

## Deploy Preview as Staging Environment

**Key Insight:** Netlify deploy previews eliminate the need for a separate staging branch!

### How It Works

1. **Create a Pull Request** to `main` branch
2. **Netlify automatically builds and deploys** a preview URL
3. **Test your changes** on the unique preview URL
4. **Share the URL** with team members for review
5. **Merge the PR** → Changes go live to production

### Deploy Preview URLs

Deploy previews follow this pattern:

```
https://deploy-preview-{PR-NUMBER}--gsc-tracking.netlify.app
```

**Example:**
- PR #42 → https://deploy-preview-42--gsc-tracking.netlify.app
- PR #43 → https://deploy-preview-43--gsc-tracking.netlify.app

### Deploy Preview Features

✅ **Unique URL per PR** - No conflicts between different features being tested  
✅ **Automatic Updates** - Every push to the PR branch triggers a new preview deploy  
✅ **Connects to Staging API** - Uses `VITE_API_URL=https://gsc-tracking-api-staging.fly.dev/api`  
✅ **Persistent Until Merge** - Preview stays live until PR is merged or closed  
✅ **Password Protection (Optional)** - Add password protection to preview deployments

### Viewing Deploy Previews

**Option 1: From GitHub PR**
- Netlify posts a comment on the PR with the preview URL
- Click the "Visit Preview" link

**Option 2: From Netlify Dashboard**
- Go to **Deploys** tab
- Find the deploy with type "Deploy Preview"
- Click to view details and URL

### Setting Up PR Comments (Recommended)

Netlify automatically posts deploy preview URLs as PR comments. To enable:

1. Go to **Site settings** → **Build & deploy** → **Deploy notifications**
2. Enable **"GitHub Pull Request Comments"**
3. Netlify will post:
   - ✅ Deploy preview URL
   - ✅ Build logs link
   - ✅ Deploy status

---

## Environment Variables

Environment variables are configured in Netlify dashboard and injected at build time.

### Required Environment Variables

| Variable | Production | Deploy Preview | Description |
|----------|-----------|----------------|-------------|
| `VITE_API_URL` | `https://gsc-tracking-api.fly.dev/api` | `https://gsc-tracking-api-staging.fly.dev/api` | Backend API URL |
| `NODE_VERSION` | `20` | `20` | Node.js version |

### Adding Environment Variables

**Method 1: Netlify Dashboard**

1. Go to **Site settings** → **Environment variables**
2. Click **"Add a variable"**
3. Choose the appropriate scope:
   - **Production** - Only for production deployments
   - **Deploy previews** - Only for PR previews (staging)
   - **Branch deploys** - For all non-production branches
   - **All** - For all deployments (use cautiously)

4. Add the variable:
   ```
   Key:   VITE_API_URL
   Value: https://gsc-tracking-api.fly.dev/api (for production)
   Value: https://gsc-tracking-api-staging.fly.dev/api (for deploy previews)
   ```

**Method 2: netlify.toml (Recommended)**

Environment variables are already configured in `netlify.toml`:

```toml
[context.production.environment]
  VITE_API_URL = "https://gsc-tracking-api.fly.dev/api"

[context.deploy-preview.environment]
  VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
```

### Future Environment Variables (When Implementing)

Add these when Auth0 or other services are integrated:

```toml
# Auth0 Configuration
VITE_AUTH0_DOMAIN = "your-domain.auth0.com"
VITE_AUTH0_CLIENT_ID = "your-client-id"
VITE_AUTH0_AUDIENCE = "https://your-api-audience"

# Feature Flags
VITE_ENABLE_ANALYTICS = "true"
VITE_ENABLE_LOGGING = "true"
```

---

## Custom Domain Setup

### Option 1: Netlify Subdomain (Free)

Default subdomain: `https://gsc-tracking.netlify.app`

To customize:
1. **Site settings** → **Domain management** → **Custom domains**
2. Click **"Options"** next to the default domain
3. Choose **"Edit site name"**
4. Enter your preferred subdomain: `gsc-tracking`

### Option 2: Custom Domain (Optional)

If you have a custom domain (e.g., `gsctracking.com`):

1. **Site settings** → **Domain management** → **Custom domains**
2. Click **"Add custom domain"**
3. Enter your domain: `gsctracking.com` or `app.gsctracking.com`
4. Netlify provides DNS instructions:

   **For Apex Domain (gsctracking.com):**
   ```dns
   A Record @ → 75.2.60.5
   ```

   **For Subdomain (app.gsctracking.com):**
   ```dns
   CNAME app → gsc-tracking.netlify.app
   ```

5. Wait 5-30 minutes for DNS propagation
6. Netlify automatically provisions SSL certificate

### DNS Providers

Netlify works with any DNS provider:
- **Cloudflare** (Recommended for DDoS protection)
- **Namecheap**
- **GoDaddy**
- **Google Domains**
- **AWS Route 53**

---

## Deployment Workflow

### Production Deployment (main branch)

```bash
# 1. Make changes on a feature branch
git checkout -b feat/new-feature
# ... make changes ...
git add .
git commit -m "feat: add new feature"

# 2. Push to GitHub
git push origin feat/new-feature

# 3. Create Pull Request
# GitHub UI or `gh pr create`

# 4. Netlify deploys a preview automatically
# Test on: https://deploy-preview-{PR}--gsc-tracking.netlify.app

# 5. After approval, merge PR
# Production deploys automatically to: https://gsc-tracking.netlify.app
```

### Manual Deployment (Optional)

For emergency hotfixes or manual control:

**Option 1: Netlify CLI**

```bash
# Install Netlify CLI
npm install -g netlify-cli

# Login
netlify login

# Link to site
netlify link

# Deploy to production
cd frontend
netlify deploy --prod --dir=dist
```

**Option 2: Netlify Dashboard**

1. Go to **Deploys** tab
2. Drag and drop the `frontend/dist` folder
3. Click **"Deploy"**

### Rollback Deployment

To rollback to a previous version:

1. Go to **Deploys** tab
2. Find the working deployment
3. Click **"..."** → **"Publish deploy"**
4. Confirm rollback

**Rollback is instant** - No rebuild required!

---

## Troubleshooting

### Build Fails

**Symptom:** Netlify build fails with error messages

**Solution:**

1. Check build logs in Netlify dashboard
2. Verify `netlify.toml` configuration:
   ```toml
   base = "frontend"
   command = "npm run build"
   publish = "dist"
   ```
3. Test build locally:
   ```bash
   cd frontend
   npm install
   npm run build
   # Should create frontend/dist folder
   ```
4. Check Node.js version:
   - Netlify uses Node 20 (configured in `netlify.toml`)
   - Local version: `node --version`

### API Connection Issues

**Symptom:** Frontend can't connect to backend API

**Solution:**

1. Verify `VITE_API_URL` is set correctly:
   - Production: `https://gsc-tracking-api.fly.dev/api`
   - Deploy Preview: `https://gsc-tracking-api-staging.fly.dev/api`
2. Check CORS settings in backend (backend should allow Netlify domains)
3. Test API directly:
   ```bash
   curl https://gsc-tracking-api.fly.dev/api/hello
   ```
4. Verify Fly.io backend is running:
   ```bash
   flyctl status --app gsc-tracking-api
   ```

### 404 on Page Refresh

**Symptom:** SPA routes return 404 when refreshed

**Solution:**

Ensure `netlify.toml` has redirect rule:

```toml
[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
  force = false
```

This tells Netlify to serve `index.html` for all routes (SPA routing).

### Deploy Preview Not Created

**Symptom:** PR doesn't trigger deploy preview

**Solution:**

1. Check **Site settings** → **Build & deploy** → **Continuous deployment**
2. Verify **"Deploy previews"** is set to **"Any pull request against your production branch"**
3. Check **Site settings** → **Build & deploy** → **Deploy contexts**
4. Ensure PR targets the `main` branch

### Environment Variables Not Working

**Symptom:** `import.meta.env.VITE_API_URL` is undefined

**Solution:**

1. Environment variables must start with `VITE_` for Vite
2. Verify in Netlify dashboard: **Site settings** → **Environment variables**
3. Redeploy after adding variables (changes don't apply retroactively)
4. Check browser console:
   ```javascript
   console.log(import.meta.env.VITE_API_URL)
   ```

---

## Cost Considerations

### Free Tier (Starter)

**Included:**
- ✅ 100 GB bandwidth/month
- ✅ 300 build minutes/month
- ✅ Unlimited sites
- ✅ Unlimited deploy previews
- ✅ Instant rollbacks
- ✅ Free SSL certificates
- ✅ Continuous deployment from GitHub

**Sufficient for:**
- Development and staging environments
- Low-traffic production apps (<10,000 visitors/month)
- Personal projects and MVPs

### Paid Tier (Pro - $19/month)

**Additional features:**
- ✅ 400 GB bandwidth/month
- ✅ Unlimited build minutes
- ✅ Password-protected sites
- ✅ Role-based access control
- ✅ Webhook notifications
- ✅ Background functions (10 hours compute/month)

**When to upgrade:**
- Production app with >10,000 visitors/month
- Team collaboration with multiple developers
- Need password-protected deploy previews
- Require advanced access controls

### Cost Optimization Tips

1. **Optimize Assets:**
   - Use image optimization (Vite handles this)
   - Enable compression (Netlify does this automatically)
   - Lazy load heavy components

2. **Monitor Bandwidth:**
   - Check **Site analytics** → **Bandwidth** in Netlify dashboard
   - Set up alerts for 80% bandwidth usage

3. **Use Cloudflare (Optional):**
   - Add Cloudflare as a CDN proxy
   - Offload bandwidth to Cloudflare (unlimited free)
   - Keep Netlify for build/deploy automation

4. **Avoid Over-Building:**
   - Skip deploy for documentation-only changes
   - Use `[skip ci]` in commit message to skip build

---

## Next Steps

After deploying to Netlify:

- [ ] **Test Deploy Preview Workflow:**
  1. Create a feature branch
  2. Make a change (e.g., update homepage text)
  3. Push and create PR
  4. Verify deploy preview URL is posted to PR
  5. Test the preview site

- [ ] **Configure Custom Domain** (if applicable)
  - Add custom domain in Netlify
  - Update DNS records
  - Wait for SSL provisioning

- [ ] **Set Up Monitoring:**
  - Enable Netlify Analytics (paid) or integrate Google Analytics
  - Set up uptime monitoring (Uptime Robot, Better Uptime)
  - Configure Sentry for error tracking

- [ ] **Update Backend CORS:**
  - Add Netlify domains to backend CORS allow list
  - Production: `https://gsc-tracking-ui.netlify.app` (or your site name)
  - Staging: `https://staging--gsc-tracking-ui.netlify.app`
  - Deploy previews: Use pattern matching (see [CORS-AUTH0-CONSIDERATIONS.md](./CORS-AUTH0-CONSIDERATIONS.md))
  - **Note:** Wildcards like `https://*.netlify.app` may not work reliably with Auth0

- [ ] **Document for Team:**
  - Share this guide with team members
  - Create quick reference for common tasks
  - Schedule training session on deploy workflow

---

## Additional Resources

**GSC Tracking Documentation:**
- **Existing Site Setup:** [NETLIFY-EXISTING-SITE-SETUP.md](./NETLIFY-EXISTING-SITE-SETUP.md) - Add staging to existing Netlify deployment
- **CORS/Auth0 Considerations:** [CORS-AUTH0-CONSIDERATIONS.md](./CORS-AUTH0-CONSIDERATIONS.md) - Wildcard issues and solutions
- **Quick Start:** [NETLIFY-QUICK-START.md](./NETLIFY-QUICK-START.md) - Quick reference guide
- **Setup Checklist:** [NETLIFY-SETUP-CHECKLIST.md](./NETLIFY-SETUP-CHECKLIST.md) - Step-by-step checklist
- **Deploy Preview Strategy:** [DEPLOY-PREVIEW-AS-STAGING.md](./DEPLOY-PREVIEW-AS-STAGING.md) - Using previews as staging
- **Fly.io Backend Deployment:** [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)
- **Hosting Evaluation:** [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md)

**Netlify Official Documentation:**
- **Netlify Documentation:** https://docs.netlify.com/
- **Netlify CLI:** https://docs.netlify.com/cli/get-started/
- **Deploy Previews:** https://docs.netlify.com/site-deploys/deploy-previews/
- **Environment Variables:** https://docs.netlify.com/environment-variables/overview/
- **Custom Domains:** https://docs.netlify.com/domains-https/custom-domains/

---

## Support and Feedback

For issues or questions:

1. Check the [Troubleshooting](#troubleshooting) section above
2. Review Netlify's [Support Forums](https://answers.netlify.com/)
3. Open an issue in the repository
4. Contact Netlify support (Pro plan includes priority support)

---

**Document Status:** ✅ Complete  
**Next Review:** 2026-03-10 (or after 3 months of usage)  
**Feedback:** Open an issue to suggest improvements
