# Fly.io Deployment Setup Checklist

This checklist will help you complete the setup for automatic deployment of the GSC Tracking backend API to Fly.io.

## ✅ Completed (Already Done)

The following has been configured in this PR:

- [x] Created `backend/fly.toml` - Fly.io application configuration
- [x] Created `.github/workflows/deploy-flyio.yml` - GitHub Actions deployment workflow
- [x] Created `docs/FLYIO-DEPLOYMENT.md` - Complete deployment documentation
- [x] Created `backend/.flyignore` - Optimized deployment context
- [x] Updated `README.md` - Added deployment information
- [x] Configured auto-scaling to free tier (256MB RAM, scale-to-zero)
- [x] Configured health checks on `/api/hello` endpoint
- [x] Set up automatic deployment triggers (push to main, backend changes only)

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

### Step 3: Authenticate and Create App (5 minutes)

```bash
# Login to Fly.io
flyctl auth login

# Navigate to backend directory
cd backend

# Create the app (without deploying yet)
flyctl launch --no-deploy --name gsc-tracking-api --region iad
```

**Important Notes:**
- The app name `gsc-tracking-api` must be globally unique on Fly.io
- If the name is taken, choose a different name and update:
  - `backend/fly.toml` (app = 'your-new-name')
  - `docs/FLYIO-DEPLOYMENT.md` (search and replace the app name)
  - `.github/workflows/deploy-flyio.yml` (deployment summary)
- Region `iad` = Northern Virginia (good for East Coast US)
- Use `flyctl platform regions` to see all available regions

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

### Step 5: Test Deployment (10 minutes)

You can now deploy in two ways:

**Option A: Manual CLI Deployment (Immediate)**
```bash
cd backend
flyctl deploy --remote-only
```

This will:
- Build the Docker image on Fly.io's servers
- Deploy to https://gsc-tracking-api.fly.dev
- Run health checks
- Show deployment status

**Option B: Trigger GitHub Actions (Automatic)**
```bash
# Make any change to backend code
cd backend/GscTracking.Api
echo "// Deployment test" >> Program.cs

# Commit and push to main
git add .
git commit -m "test: trigger Fly.io deployment"
git push origin main
```

Watch the deployment:
- Go to **Actions** tab in GitHub
- Click the latest **"Deploy Backend to Fly.io"** workflow run
- Monitor progress in real-time

### Step 6: Verify Deployment (2 minutes)

After deployment completes, test the API:

```bash
# Test health endpoint
curl https://gsc-tracking-api.fly.dev/api/hello

# Expected response:
# {"message":"Hello from GSC Tracking API!","version":"1.0.0","timestamp":"..."}
```

Open in browser:
- https://gsc-tracking-api.fly.dev/api/hello

Check app status:
```bash
flyctl status
flyctl logs
```

## 🎉 You're Done!

Your backend API is now automatically deployed to Fly.io!

### What Happens Now

Every time you:
- Push to `main` branch
- Make changes in `backend/` directory
- Or manually trigger the workflow

The API will automatically:
1. Build a new Docker image
2. Deploy to Fly.io
3. Run health checks
4. Update the live app with zero downtime

### Important URLs

- **Production API:** https://gsc-tracking-api.fly.dev
- **Health Check:** https://gsc-tracking-api.fly.dev/api/hello
- **Fly.io Dashboard:** https://fly.io/dashboard
- **Complete Documentation:** [docs/FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)

## 🔍 Monitoring and Management

### View Logs
```bash
flyctl logs               # Stream live logs
flyctl logs --count 200   # Last 200 lines
```

### Check Status
```bash
flyctl status             # App health and metrics
flyctl dashboard          # Open web dashboard
```

### Scaling
```bash
flyctl scale count 2              # Scale to 2 instances
flyctl scale vm shared-cpu-1x --memory 512  # Upgrade to 512MB
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
