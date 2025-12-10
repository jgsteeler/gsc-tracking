# Fly.io Deployment Setup Checklist

This checklist will help you complete the setup for automatic deployment of the GSC Tracking backend API to Fly.io.

## ✅ Completed (Already Done)

The following has been configured in this PR:

- [x] Created `backend/fly.toml` - Production environment configuration
- [x] Created `backend/fly.staging.toml` - Staging environment configuration
- [x] Created `.github/workflows/deploy-flyio.yml` - GitHub Actions deployment workflow with GitHub Flow
- [x] Created `docs/FLYIO-DEPLOYMENT.md` - Complete deployment documentation
- [x] Created `backend/.flyignore` - Optimized deployment context
- [x] Updated `README.md` - Added deployment information
- [x] Configured auto-scaling to free tier (256MB RAM, scale-to-zero)
- [x] Configured health checks on `/api/hello` endpoint
- [x] Set up GitHub Flow: staging on PR, production on merge to main
- [x] Automatic PR comments with staging URLs

## ⏳ Next Steps (Action Required)

To enable automatic deployment to Fly.io, complete these steps:

### Step 1: Create Fly.io Account (5 minutes)

1. Visit https://fly.io/app/sign-up
2. Sign up with GitHub (recommended) or email
3. Verify your email address
4. Add a credit card (required for verification; you will only be charged if usage exceeds free tier limits)

**Free Tier Includes:**
- 3 shared-cpu VMs (256MB RAM each)
- 3GB persistent storage
- 160GB outbound data transfer per month
- **No charges for this configuration**

### Step 2: Install Fly.io CLI (5 minutes)

Choose your operating system:

**macOS:**
```bash
brew install flyctl
```

**Linux:**
```bash
curl -L https://fly.io/install.sh | sh
```

**Windows:**
```powershell
pwsh -Command "iwr https://fly.io/install.ps1 -useb | iex"
```

Verify installation:
```bash
flyctl version
```

### Step 3: Authenticate and Create Apps (10 minutes)

You need to create both production and staging apps:

```bash
# Login to Fly.io
flyctl auth login

# Navigate to backend directory
cd backend

# Create production app (without deploying yet)
flyctl launch --no-deploy --name gsc-tracking-api --region iad --config fly.toml

# Create staging app (without deploying yet)
flyctl launch --no-deploy --name gsc-tracking-api-staging --region iad --config fly.staging.toml
```

**Important Notes:**
- Both app names must be globally unique on Fly.io
- If names are taken, choose different names and update:
  - `backend/fly.toml` (app = 'your-production-name')
  - `backend/fly.staging.toml` (app = 'your-staging-name')
  - `docs/FLYIO-DEPLOYMENT.md` (search and replace the app names)
  - `.github/workflows/deploy-flyio.yml` (update URLs in comments)
- Region `iad` = Northern Virginia (good for East Coast US)
- Use `flyctl platform regions` to see all available regions
- Both apps use the same region for consistency

### Step 4: Generate and Add Deploy Token (5 minutes)

```bash
# Generate a deploy token (valid for 1 year)
flyctl tokens create deploy --expiry 8760h
```

**Copy the token output** (it looks like: `FlyV1 ey...`)

**Security Note:** Set a calendar reminder to rotate this token annually.

Add to GitHub:
1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `FLY_API_TOKEN`
5. Value: Paste the token
6. Click **Add secret**

### Step 5: Test Deployment (15 minutes)

Test both staging and production deployments:

**Option A: Test GitHub Flow (Recommended)**
```bash
# Create a feature branch
git checkout -b test/deployment

# Make a small change
cd backend/GscTracking.Api
echo "// Deployment test" >> Program.cs

# Commit and push
git add .
git commit -m "test: trigger staging deployment"
git push origin test/deployment

# Open a pull request on GitHub
```

This will:
1. Automatically deploy to staging: https://gsc-tracking-api-staging.fly.dev
2. Comment on the PR with the staging URL
3. Allow you to test before merging to production

After testing staging:
```bash
# Merge the PR via GitHub UI, then locally:
git checkout main
git pull origin main
```

This will automatically deploy to production: https://gsc-tracking-api.fly.dev

**Option B: Manual CLI Deployment**
```bash
cd backend

# Deploy to staging
flyctl deploy --config fly.staging.toml --remote-only

# Deploy to production
flyctl deploy --config fly.toml --remote-only
```

**Monitor Deployments:**
- Go to **Actions** tab in GitHub
- Click the latest **"Deploy Backend to Fly.io"** workflow run
- Monitor progress in real-time
- Check PR comments for staging URLs

### Step 6: Verify Deployment (5 minutes)

After deployments complete, test both environments:

**Test Staging:**
```bash
# Test staging health endpoint
curl https://gsc-tracking-api-staging.fly.dev/api/hello

# Expected response:
# {"message":"Hello from GSC Tracking API!","version":"1.0.0","timestamp":"..."}

# Check staging app status
flyctl status --app gsc-tracking-api-staging
flyctl logs --app gsc-tracking-api-staging
```

**Test Production:**
```bash
# Test production health endpoint
curl https://gsc-tracking-api.fly.dev/api/hello

# Check production app status
flyctl status --app gsc-tracking-api
flyctl logs --app gsc-tracking-api
```

**Open in browser:**
- Staging: https://gsc-tracking-api-staging.fly.dev/api/hello
- Production: https://gsc-tracking-api.fly.dev/api/hello

## 🎉 You're Done!

Your backend API is now automatically deployed to Fly.io with GitHub Flow!

### What Happens Now (GitHub Flow)

**When you create a PR:**
- Opens PR → Automatically deploys to **staging**
- Update PR → Automatically redeploys to **staging**
- PR comment shows staging URL for testing
- Changes in `backend/` directory trigger deployment

**When you merge to main:**
- Merge PR → Automatically deploys to **production**
- Changes in `backend/` directory trigger deployment
- Zero-downtime deployment
- Health checks verify successful deployment

The workflow:
1. Create a feature branch
2. Open a PR (deploys to staging)
3. Test on staging URL
4. Merge to main (deploys to production)

### Important URLs

**Production Environment:**
- **API:** https://gsc-tracking-api.fly.dev
- **Health Check:** https://gsc-tracking-api.fly.dev/api/hello

**Staging Environment (PR Previews):**
- **API:** https://gsc-tracking-api-staging.fly.dev
- **Health Check:** https://gsc-tracking-api-staging.fly.dev/api/hello

**Management:**
- **Fly.io Dashboard:** https://fly.io/dashboard
- **Complete Documentation:** [docs/FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)

## 🔍 Monitoring and Management

### View Logs
```bash
# Production logs
flyctl logs --app gsc-tracking-api
flyctl logs --app gsc-tracking-api --count 200

# Staging logs
flyctl logs --app gsc-tracking-api-staging
```

### Check Status
```bash
# Production status
flyctl status --app gsc-tracking-api
flyctl dashboard --app gsc-tracking-api

# Staging status
flyctl status --app gsc-tracking-api-staging
```

### Scaling
```bash
# Scale production
flyctl scale count 2 --app gsc-tracking-api
flyctl scale vm shared-cpu-1x --memory 512 --app gsc-tracking-api

# Staging typically doesn't need scaling (stays at free tier)
```

### Secrets Management
```bash
flyctl secrets set DATABASE_URL="postgres://..."
flyctl secrets list
```

## 💰 Cost Management

Current configuration = **$0/month** (within free tier)

### Free Tier Limits
- 3 VMs × 256MB RAM = **Free**
- 3GB storage = **Free**
- 160GB bandwidth/month = **Free**

### When You Exceed Free Tier
- Additional VMs: ~$2/month per 256MB VM
- More RAM: ~$4/month per 512MB VM
- More storage: $0.15/GB/month
- Bandwidth overage: $0.02/GB

**Set up billing alerts:**
1. Go to https://fly.io/dashboard/personal/billing
2. Set spending limit (e.g., $10/month)
3. Get email alerts when approaching limit

## 🐛 Troubleshooting

### Deployment Fails

**Check GitHub Actions logs:**
- Go to Actions tab → failed workflow → click job → view error

**Common issues:**
- `FLY_API_TOKEN` not set → Add token to GitHub secrets
- App name conflict → Use a different app name in fly.toml
- Build errors → Check Dockerfile and .NET project

**Check Fly.io logs:**
```bash
flyctl logs
```

### App Won't Start

**Check health checks:**
```bash
flyctl status
flyctl checks list
```

**Common issues:**
- Port mismatch → Ensure Dockerfile exposes 8080
- Health endpoint → Verify `/api/hello` returns 200 OK
- Memory issues → Increase VM memory in fly.toml

**SSH into app:**
```bash
flyctl ssh console
```

### Need Help?

- **Documentation:** [docs/FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)
- **Fly.io Docs:** https://fly.io/docs/
- **Fly.io Community:** https://community.fly.io/
- **GitHub Issues:** https://github.com/jgsteeler/gsc-tracking/issues

## 📚 Additional Resources

- [Fly.io .NET Guide](https://fly.io/docs/languages-and-frameworks/dotnet/)
- [Fly.io Configuration Reference](https://fly.io/docs/reference/configuration/)
- [Fly.io CLI Reference](https://fly.io/docs/flyctl/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

---

**Last Updated:** 2025-12-09  
**Status:** ✅ Ready for deployment  
**Estimated Setup Time:** 20-30 minutes
