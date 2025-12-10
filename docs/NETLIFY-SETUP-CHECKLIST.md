# Netlify Frontend Deployment Setup Checklist

Use this checklist to deploy the GSC Tracking frontend to Netlify for the first time.

---

## Pre-Deployment Checklist

- [ ] **Backend is deployed to Fly.io**
  - [ ] Production backend: `https://gsc-tracking-api.fly.dev`
  - [ ] Staging backend: `https://gsc-tracking-api-staging.fly.dev`
  - [ ] Health checks passing (see [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md))

- [ ] **Test frontend build locally**
  ```bash
  cd frontend
  npm install
  npm run build
  # Should create dist folder with no errors
  ```

- [ ] **Netlify configuration files exist**
  - [ ] `netlify.toml` in root directory
  - [ ] `frontend/.env.example` exists
  - [ ] Environment variables documented

---

## Netlify Account Setup

- [ ] **Create Netlify account**
  1. Go to https://app.netlify.com/signup
  2. Click "Sign up with GitHub"
  3. Authorize Netlify to access GitHub

- [ ] **Connect GitHub repository**
  1. Click "Add new site" → "Import an existing project"
  2. Choose "Deploy with GitHub"
  3. Select repository: `jgsteeler/gsc-tracking`

---

## Netlify Build Configuration

- [ ] **Configure build settings**
  ```
  Base directory:    frontend
  Build command:     npm run build
  Publish directory: frontend/dist
  ```

- [ ] **Verify build settings** in Site settings → Build & deploy

- [ ] **Check Node version** (should be 20, set in netlify.toml)

---

## Environment Variables (Optional)

Environment variables are already configured in `netlify.toml`, but you can override them:

- [ ] **Production environment variables** (if needed)
  - Navigate to: Site settings → Environment variables → Production
  - Add: `VITE_API_URL` = `https://gsc-tracking-api.fly.dev/api`

- [ ] **Deploy preview environment variables** (if needed)
  - Navigate to: Site settings → Environment variables → Deploy previews
  - Add: `VITE_API_URL` = `https://gsc-tracking-api-staging.fly.dev/api`

> **Note:** The default configuration in `netlify.toml` should work without manual environment variable setup.

---

## Deploy Configuration

- [ ] **Enable deploy previews**
  1. Site settings → Build & deploy → Continuous deployment
  2. Verify "Deploy previews" is set to "Any pull request against your production branch"

- [ ] **Enable branch deploys** (optional)
  1. Site settings → Build & deploy → Continuous deployment
  2. Set "Branch deploys" to your preference (e.g., "All branches")

- [ ] **Enable GitHub PR comments**
  1. Site settings → Build & deploy → Deploy notifications
  2. Enable "GitHub Pull Request Comments"
  3. Netlify will post deploy preview URLs to PRs

---

## Initial Deployment

- [ ] **Trigger first deployment**
  - Click "Deploy site" button in Netlify dashboard
  - Wait for build to complete (2-5 minutes)

- [ ] **Verify deployment succeeded**
  - Check Deploys tab for green checkmark
  - Note the deploy URL

- [ ] **Test deployed site**
  - Visit the Netlify-provided URL
  - Verify app loads correctly
  - Check browser console for errors
  - Test API connection (customers page, etc.)

---

## Site Settings

- [ ] **Change site name**
  1. Site settings → General → Site details
  2. Click "Change site name"
  3. Enter: `gsc-tracking`
  4. New URL: `https://gsc-tracking.netlify.app`

- [ ] **Note your site URLs**
  - Production: `https://gsc-tracking.netlify.app`
  - Deploy previews: `https://deploy-preview-{PR}--gsc-tracking.netlify.app`
  - Branch deploys: `https://{branch}--gsc-tracking.netlify.app`

---

## Backend CORS Configuration

- [ ] **Update backend CORS** to allow Netlify domains
  
  In backend `Program.cs`, ensure CORS allows:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowFrontend", policy =>
      {
          policy.WithOrigins(
              "http://localhost:5173",
              "https://gsc-tracking.netlify.app",
              "https://deploy-preview-*.netlify.app",
              "https://*.gsc-tracking.netlify.app"
          )
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
      });
  });
  ```

- [ ] **Test CORS** from deployed frontend
  - Open browser console on deployed site
  - Try fetching customers
  - Verify no CORS errors

---

## Testing Deploy Preview Workflow

- [ ] **Create test PR**
  ```bash
  git checkout -b test/netlify-preview
  # Make a small change (e.g., update a text string)
  git commit -m "test: verify deploy preview workflow"
  git push origin test/netlify-preview
  # Open PR on GitHub
  ```

- [ ] **Verify deploy preview created**
  - Check PR for Netlify comment with preview URL
  - Wait for "Deploy Preview ready!" status
  - Click preview URL

- [ ] **Test preview site**
  - Verify changes appear on preview
  - Confirm it connects to staging backend
  - Make another commit to PR
  - Verify preview updates automatically

- [ ] **Clean up test PR**
  - Close PR (don't merge)
  - Verify deploy preview is deleted

---

## Production Deployment Test

- [ ] **Merge a PR to main**
  - Merge test PR or create small change
  - Watch Netlify deploy to production

- [ ] **Verify production deployment**
  - Visit `https://gsc-tracking.netlify.app`
  - Verify changes appear
  - Test full functionality
  - Check that it connects to production backend

---

## Optional: Custom Domain

If you have a custom domain:

- [ ] **Add custom domain**
  1. Site settings → Domain management → Custom domains
  2. Click "Add custom domain"
  3. Enter your domain (e.g., `app.gsctracking.com`)

- [ ] **Configure DNS**
  
  **For subdomain (app.gsctracking.com):**
  ```dns
  CNAME app → gsc-tracking.netlify.app
  ```
  
  **For apex domain (gsctracking.com):**
  ```dns
  A @ → 75.2.60.5
  ```

- [ ] **Wait for SSL provisioning**
  - Netlify automatically provisions Let's Encrypt SSL
  - Takes 5-30 minutes after DNS propagation
  - Green checkmark appears when ready

- [ ] **Verify custom domain**
  - Visit your custom domain
  - Ensure HTTPS works
  - Test all functionality

---

## Monitoring and Notifications

- [ ] **Set up deploy notifications** (optional)
  1. Site settings → Build & deploy → Deploy notifications
  2. Add Slack, email, or webhook notifications
  3. Configure for "Deploy succeeded" and "Deploy failed"

- [ ] **Enable Netlify Analytics** (optional, paid feature)
  1. Site settings → Analytics
  2. Enable analytics
  3. Costs $9/month per site

- [ ] **Set up external monitoring**
  - Consider: UptimeRobot, Pingdom, Better Uptime
  - Monitor: `https://gsc-tracking.netlify.app`
  - Alert on downtime or slow response

---

## Security Configuration

- [ ] **Review security headers**
  - Headers already configured in `netlify.toml`
  - Verify at: https://securityheaders.com/

- [ ] **Enable password protection** (optional, Pro plan)
  - Site settings → Access & security → Visitor access
  - Set password for deploy previews
  - Share password with team

- [ ] **Configure role-based access** (optional, Team plan)
  - Team → Members → Invite team
  - Set roles: Owner, Admin, Developer, Viewer

---

## Documentation

- [ ] **Update team documentation**
  - Share `docs/NETLIFY-DEPLOYMENT.md` with team
  - Add deploy URLs to internal wiki/docs
  - Document any custom configuration

- [ ] **Train team on workflow**
  - Explain deploy preview process
  - Show how to find preview URLs
  - Demonstrate rollback procedure

---

## Post-Deployment Verification

- [ ] **Functional testing**
  - [ ] Homepage loads
  - [ ] Navigation works
  - [ ] Customers page loads
  - [ ] Can create/edit/delete customer
  - [ ] API calls succeed
  - [ ] No console errors

- [ ] **Performance testing**
  - [ ] Run Lighthouse audit (aim for 90+ score)
  - [ ] Check load time (<3 seconds)
  - [ ] Test on mobile device
  - [ ] Verify images load

- [ ] **Cross-browser testing**
  - [ ] Chrome/Edge
  - [ ] Firefox
  - [ ] Safari (if available)
  - [ ] Mobile browsers

---

## Troubleshooting

If deployment fails, check:

- [ ] **Build logs** in Netlify dashboard
  - Deploys tab → Click on failed deploy → View logs

- [ ] **Local build** works
  ```bash
  cd frontend
  npm install
  npm run build
  ```

- [ ] **netlify.toml** syntax
  - Validate TOML syntax
  - Check indentation

- [ ] **Environment variables**
  - Verify VITE_ prefix
  - Check for typos in URLs

- [ ] **Node version**
  - Should be 20 (check netlify.toml)

See [NETLIFY-DEPLOYMENT.md](./NETLIFY-DEPLOYMENT.md) troubleshooting section for more details.

---

## Success Criteria

Deployment is successful when:

- ✅ Production site is live at `https://gsc-tracking.netlify.app`
- ✅ Deploy previews work for PRs
- ✅ Frontend connects to backend without CORS errors
- ✅ All pages load and function correctly
- ✅ No console errors in browser
- ✅ HTTPS/SSL working
- ✅ Team can deploy by merging to main

---

## Maintenance Tasks

### Weekly
- [ ] Check deploy logs for errors
- [ ] Verify monitoring/uptime checks passing

### Monthly
- [ ] Review bandwidth usage (free tier = 100GB)
- [ ] Check build minutes (free tier = 300 min)
- [ ] Update dependencies if needed
- [ ] Review and close old deploy previews

### Quarterly
- [ ] Review Netlify bill (if on paid plan)
- [ ] Evaluate need for upgrade/downgrade
- [ ] Review custom domain SSL (auto-renews)
- [ ] Audit environment variables

---

## Resources

- **Full Guide:** [NETLIFY-DEPLOYMENT.md](./NETLIFY-DEPLOYMENT.md)
- **Quick Reference:** [NETLIFY-QUICK-START.md](./NETLIFY-QUICK-START.md)
- **Deploy Preview Strategy:** [DEPLOY-PREVIEW-AS-STAGING.md](./DEPLOY-PREVIEW-AS-STAGING.md)
- **Backend Deployment:** [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)
- **Netlify Docs:** https://docs.netlify.com/

---

**Completed:** _______________  
**Deployed By:** _______________  
**Production URL:** _______________  
**Notes:** _______________
