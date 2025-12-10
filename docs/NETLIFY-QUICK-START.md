# Netlify Quick Start Guide

**TL;DR** - Deploy frontend to Netlify in 5 minutes

---

## Initial Setup (One-Time)

### 1. Connect to Netlify

1. Go to https://app.netlify.com/
2. Click **"Add new site"** → **"Import an existing project"**
3. Choose **GitHub** → Select `jgsteeler/gsc-tracking`
4. Configure:
   ```
   Base directory:    frontend
   Build command:     npm run build
   Publish directory: frontend/dist
   ```
5. Add environment variables:
   - Production: `VITE_API_URL` = `https://gsc-tracking-api.fly.dev/api`
   - Deploy previews: `VITE_API_URL` = `https://gsc-tracking-api-staging.fly.dev/api`
6. Click **"Deploy"**

### 2. Update Site Name

1. **Site settings** → **Site details** → **"Change site name"**
2. Choose: `gsc-tracking`
3. Your site: https://gsc-tracking.netlify.app

---

## Daily Workflow

### Production Deploy

```bash
# Merge to main = auto-deploy to production
git checkout main
git merge feature-branch
git push origin main
```

✅ Netlify automatically deploys to: https://gsc-tracking.netlify.app

### Staging Deploy (Deploy Preview)

```bash
# Create PR = auto-deploy to staging
git checkout -b feat/my-feature
# ... make changes ...
git push origin feat/my-feature
# Open PR on GitHub
```

✅ Netlify creates preview: `https://deploy-preview-{PR}--gsc-tracking.netlify.app`  
✅ URL posted as comment on the PR  
✅ Every push to PR updates the preview

---

## Key Concepts

### Deploy Previews = Staging

- **No separate staging branch needed**
- Each PR gets a unique preview URL
- Preview connects to staging backend: `https://gsc-tracking-api-staging.fly.dev/api`
- Perfect for testing features before production

### Environment Variables

Already configured in `netlify.toml`:

- **Production:** Uses production API
- **Deploy Previews:** Uses staging API
- **No manual configuration needed** after initial setup

### Instant Rollbacks

To revert a bad deploy:

1. **Netlify dashboard** → **Deploys**
2. Find last working deploy
3. Click **"Publish deploy"**
4. Done! (no rebuild needed)

---

## Common Tasks

### Test Build Locally

```bash
cd frontend
npm install
npm run build
# Check frontend/dist folder
npm run preview  # Test production build locally
```

### View Deploy Logs

1. Netlify dashboard → **Deploys**
2. Click on the deploy
3. View build logs

### Password Protect Deploy Previews

1. **Site settings** → **Build & deploy** → **Deploy previews**
2. Enable **"Password-protected"**
3. Set password
4. Share with team

### Add Custom Domain

1. **Site settings** → **Domain management** → **Add custom domain**
2. Follow DNS instructions
3. SSL auto-provisions (5-30 minutes)

---

## Troubleshooting

### Build Failed

```bash
# Test locally first
cd frontend
npm run build

# Common issues:
# - Missing dependencies → npm install
# - TypeScript errors → fix and test locally
# - Wrong Node version → update netlify.toml
```

### Can't Connect to API

```bash
# Check API is running
curl https://gsc-tracking-api.fly.dev/api/hello

# Check CORS in backend
# Ensure backend allows: https://*.netlify.app
```

### Deploy Preview Not Created

- Verify PR targets `main` branch
- Check **Site settings** → **Build & deploy** → **Deploy previews** is enabled
- Wait 2-3 minutes after opening PR

---

## Full Documentation

See [NETLIFY-DEPLOYMENT.md](./NETLIFY-DEPLOYMENT.md) for:
- Complete setup guide
- Environment variables
- Custom domains
- Advanced configuration
- Cost optimization

---

**Need Help?**
- Netlify docs: https://docs.netlify.com/
- Open an issue in the repository
- Check build logs in Netlify dashboard
