# Fly.io Backend Deployment Guide

This guide covers deploying the GSC Tracking backend API to [Fly.io](https://fly.io), a platform for running applications close to users with automatic scaling and global distribution.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Initial Setup](#initial-setup)
- [Deployment Methods](#deployment-methods)
- [Environment Configuration](#environment-configuration)
- [Monitoring and Logs](#monitoring-and-logs)
- [Scaling](#scaling)
- [Troubleshooting](#troubleshooting)
- [Cost Information](#cost-information)

---

## Overview

The GSC Tracking backend is deployed to Fly.io using:

- **Platform:** Fly.io
- **Region:** `iad` (Northern Virginia, USA)
- **App Name:** `gsc-tracking-api`
- **URL:** https://gsc-tracking-api.fly.dev
- **Health Check:** https://gsc-tracking-api.fly.dev/api/hello

### Deployment Triggers

Automatic deployment occurs when:
- Code is pushed to the `main` branch
- Changes are made in the `backend/` directory
- Changes are made to `.github/workflows/deploy-flyio.yml`
- Manual workflow dispatch is triggered

The deployment is handled by GitHub Actions using the workflow defined in `.github/workflows/deploy-flyio.yml`.

---

## Prerequisites

### For Automated Deployment (GitHub Actions)

1. **Fly.io Account:** [Sign up for free](https://fly.io/app/sign-up)
2. **FLY_API_TOKEN:** GitHub repository secret (see [Initial Setup](#initial-setup))

### For Manual Deployment

1. **Fly.io Account:** [Sign up for free](https://fly.io/app/sign-up)
2. **Fly.io CLI (`flyctl`):** Install locally
   ```bash
   # macOS
   brew install flyctl

   # Linux
   curl -L https://fly.io/install.sh | sh

   # Windows
   pwsh -Command "iwr https://fly.io/install.ps1 -useb | iex"
   ```
3. **Docker:** For local testing (optional)

---

## Initial Setup

### 1. Create Fly.io Account

1. Visit https://fly.io/app/sign-up
2. Sign up with GitHub (recommended) or email
3. Verify your email address

### 2. Generate Fly.io API Token

```bash
# Login to Fly.io
flyctl auth login

# Generate a deploy token (valid for 1 year)
flyctl tokens create deploy --expiry 8760h
```

This creates a deploy token valid for 1 year. Copy the token output.

**Security Note:** For better security, rotate tokens annually. Set a calendar reminder to regenerate and update the token before expiration.

### 3. Add Token to GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `FLY_API_TOKEN`
5. Value: Paste the token from step 2
6. Click **Add secret**

### 4. Create Fly.io App (First Time Only)

```bash
cd backend
flyctl launch --no-deploy --name gsc-tracking-api --region iad
```

This creates the app in Fly.io without deploying. The configuration is already defined in `backend/fly.toml`.

**Note:** The app name `gsc-tracking-api` must be globally unique on Fly.io. If taken, choose a different name and update `backend/fly.toml` and this documentation.

---

## Deployment Methods

### Method 1: Automatic Deployment (Recommended)

Push changes to the `main` branch:

```bash
git add .
git commit -m "feat(api): add new endpoint"
git push origin main
```

GitHub Actions will automatically:
1. Build the Docker image
2. Deploy to Fly.io
3. Run health checks
4. Show deployment summary

Monitor the deployment:
- Go to **Actions** tab in GitHub
- Click on the latest **"Deploy Backend to Fly.io"** workflow run

### Method 2: Manual Deployment via CLI

```bash
# Navigate to backend directory
cd backend

# Login to Fly.io
flyctl auth login

# Deploy
flyctl deploy

# Or deploy without local Docker build (uses Fly.io builders)
flyctl deploy --remote-only
```

### Method 3: Manual Workflow Trigger

1. Go to **Actions** tab in GitHub
2. Click **Deploy Backend to Fly.io** workflow
3. Click **Run workflow** dropdown
4. Select `main` branch
5. Click **Run workflow** button

---

## Environment Configuration

### Environment Variables

The app uses these environment variables (configured in `fly.toml`):

```toml
[env]
  ASPNETCORE_ENVIRONMENT = "Production"
  ASPNETCORE_URLS = "http://+:8080"
```

### Adding Secrets

For sensitive configuration (database connection strings, API keys):

```bash
# Set a secret
flyctl secrets set DATABASE_URL="postgres://user:pass@host:5432/db"

# Set multiple secrets
flyctl secrets set \
  AUTH0_DOMAIN="your-domain.auth0.com" \
  AUTH0_CLIENT_SECRET="your-secret"

# List secrets (values are hidden)
flyctl secrets list

# Remove a secret
flyctl secrets unset DATABASE_URL
```

Secrets are encrypted and injected as environment variables at runtime.

### Custom Domains

To use a custom domain (e.g., `api.gsctracking.com`):

```bash
# Add certificate for your domain
flyctl certs create api.gsctracking.com

# Fly.io will provide DNS records to add to your domain registrar
flyctl certs show api.gsctracking.com
```

Add the provided DNS records:
```dns
CNAME api gsc-tracking-api.fly.dev
```

SSL certificates are automatically provisioned via Let's Encrypt.

---

## Monitoring and Logs

### View Logs

```bash
# Stream live logs
flyctl logs

# View last 200 lines
flyctl logs --count 200

# Filter by severity
flyctl logs --level error

# View logs from specific instance
flyctl logs --instance <instance-id>
```

### Health Checks

The app has a health check configured:
- **Endpoint:** `/api/hello`
- **Interval:** Every 15 seconds
- **Timeout:** 10 seconds
- **Grace Period:** 30 seconds on startup

View health status:
```bash
flyctl status
```

### Monitoring Dashboard

Access the Fly.io dashboard:
```bash
flyctl dashboard
```

Or visit: https://fly.io/dashboard

The dashboard shows:
- App status and health
- CPU and memory usage
- Network traffic
- Recent deployments
- Logs and metrics

### Application Metrics

```bash
# View current resource usage
flyctl status

# View detailed VM metrics
flyctl vm status
```

---

## Scaling

### Vertical Scaling (VM Size)

Increase VM resources by editing `backend/fly.toml`:

```toml
[[vm]]
  memory = '512mb'  # Upgrade from 256mb
  cpu_kind = 'shared'
  cpus = 1
```

Then deploy:
```bash
flyctl deploy
```

**Available VM sizes:**
- `256mb` - Free tier
- `512mb` - $0.0000008/sec (~$2/month)
- `1gb` - $0.0000016/sec (~$4/month)
- `2gb` - $0.0000032/sec (~$8/month)

### Horizontal Scaling (Multiple Instances)

Scale to multiple VMs:

```bash
# Scale to 2 instances
flyctl scale count 2

# Scale to specific regions
flyctl regions add ord  # Add Chicago
flyctl scale count 2 --region iad --region ord
```

Update `fly.toml` to maintain minimum instances:

```toml
[http_service]
  min_machines_running = 1  # Always keep 1 running
```

### Auto-Scaling

The current configuration uses auto-scaling:

```toml
[http_service]
  auto_stop_machines = 'stop'    # Stop machines when idle
  auto_start_machines = true      # Start on request
  min_machines_running = 0        # Scale to zero when idle
```

For production, consider:
```toml
[http_service]
  auto_stop_machines = 'suspend'  # Suspend instead of stop (faster wake)
  min_machines_running = 1        # Always keep 1 running
```

---

## Troubleshooting

### Deployment Fails

1. **Check logs:**
   ```bash
   flyctl logs
   ```

2. **Verify Dockerfile builds locally:**
   ```bash
   cd backend
   docker build -t gsc-api-test .
   docker run -p 8080:8080 gsc-api-test
   curl http://localhost:8080/api/hello
   ```

3. **Check GitHub Actions workflow:**
   - Go to **Actions** tab
   - Review error messages in failed workflow run

### App Won't Start

1. **Check app status:**
   ```bash
   flyctl status
   ```

2. **Check health checks:**
   ```bash
   flyctl checks list
   ```

3. **View detailed VM status:**
   ```bash
   flyctl vm status
   ```

4. **SSH into running instance:**
   ```bash
   flyctl ssh console
   ```

### Connection Issues

1. **Verify app is running:**
   ```bash
   flyctl status
   ```

2. **Test health endpoint:**
   ```bash
   curl https://gsc-tracking-api.fly.dev/api/hello
   ```

3. **Check DNS resolution:**
   ```bash
   nslookup gsc-tracking-api.fly.dev
   ```

4. **Review firewall rules:**
   ```bash
   flyctl ips list
   ```

### High Memory Usage

1. **Monitor metrics:**
   ```bash
   flyctl status
   ```

2. **Increase VM memory in `fly.toml`:**
   ```toml
   [[vm]]
     memory = '512mb'
   ```

3. **Restart app:**
   ```bash
   flyctl apps restart gsc-tracking-api
   ```

### Build Errors

If build fails with "No space left on device":

```bash
# Use remote builders (Fly.io handles build)
flyctl deploy --remote-only
```

---

## Cost Information

### Free Tier (Development/Testing)

Fly.io free tier includes:
- **3 shared-cpu-1x VMs** (256MB RAM each)
- **3GB persistent volume storage**
- **160GB outbound data transfer**

With the current configuration (1 VM, 256MB RAM, no volumes), the app runs **completely free**.

### Paid Tier (Production)

Costs are based on actual usage (billed per second):

| Resource | Cost | Example |
|----------|------|---------|
| **shared-cpu-1x (256MB)** | $0.0000008/sec | ~$2.07/month |
| **shared-cpu-1x (512MB)** | $0.0000016/sec | ~$4.14/month |
| **shared-cpu-1x (1GB)** | $0.0000032/sec | ~$8.28/month |
| **Outbound Data** | $0.02/GB | First 160GB free |
| **Persistent Volumes** | $0.15/GB/month | When database is added |

**Typical Production Costs:**
- **Single instance (512MB):** ~$4-5/month
- **2 instances (512MB, HA):** ~$8-10/month
- **With Postgres (10GB):** +$1.50/month

**With Auto-Scaling (Scale to Zero):**
- No charges when app is idle
- Only pay when app is running
- Great for development/staging environments

**Important:** The default configuration uses `auto_stop_machines = 'stop'` with `min_machines_running = 0`, which means:
- ✅ **Best for:** Development/testing (free tier, zero cost when idle)
- ⚠️ **Cold starts:** First request after idle period takes 3-5 seconds
- 💡 **For production:** Consider `auto_stop_machines = 'suspend'` (faster wake) or `min_machines_running = 1` (no cold starts, ~$2/month)

### Monitor Costs

View current usage:
```bash
flyctl billing status
```

View detailed usage:
- https://fly.io/dashboard/personal/billing

Set spending alerts in the dashboard to avoid surprises.

---

## Database Setup (Optional)

When ready to add a database:

```bash
# Create Fly Postgres database
flyctl postgres create --name gsc-tracking-db --region iad

# Attach to app (sets DATABASE_URL secret)
flyctl postgres attach gsc-tracking-db --app gsc-tracking-api

# View database info
flyctl postgres db list --app gsc-tracking-db

# Connect to database
flyctl postgres connect --app gsc-tracking-db
```

**Free Tier Postgres:**
- 1 shared CPU instance
- 256MB RAM
- 3GB storage

**Paid Postgres (Recommended for Production):**
- 2+ instances (high availability)
- 1-2GB RAM per instance
- Automatic failover
- Daily backups

---

## Additional Commands

### App Management

```bash
# Restart app
flyctl apps restart gsc-tracking-api

# Destroy app (WARNING: deletes everything)
flyctl apps destroy gsc-tracking-api

# View app info
flyctl info

# Open app in browser
flyctl open

# Open in Fly.io dashboard
flyctl dashboard
```

### Regions

```bash
# List available regions
flyctl platform regions

# Add region
flyctl regions add ord  # Chicago

# Remove region
flyctl regions remove ord

# List current regions
flyctl regions list
```

### Networking

```bash
# List IP addresses
flyctl ips list

# Allocate IPv4 (required for custom domains)
flyctl ips allocate-v4

# Allocate IPv6
flyctl ips allocate-v6
```

---

## Security Best Practices

1. **Use Secrets for Sensitive Data**
   - Never commit secrets to `fly.toml`
   - Use `flyctl secrets set` for database URLs, API keys, etc.

2. **Enable HTTPS**
   - Already configured via `force_https = true`
   - Free SSL certificates via Let's Encrypt

3. **Run as Non-Root User**
   - Already configured in `Dockerfile`
   - App runs as `appuser`

4. **Keep Dependencies Updated**
   - Regularly update .NET SDK version
   - Update base images in Dockerfile

5. **Monitor Logs and Metrics**
   - Check logs regularly for errors
   - Set up external monitoring (e.g., Sentry)

6. **Use Private Networking for Databases**
   - Fly.io provides private networking via WireGuard
   - Database is not exposed to internet

---

## CI/CD Integration

The deployment is automated via GitHub Actions (`.github/workflows/deploy-flyio.yml`).

### Workflow Features

- ✅ Deploys on push to `main` branch
- ✅ Only runs when backend code changes
- ✅ Uses remote builds (no local Docker required)
- ✅ Skips if `FLY_API_TOKEN` is not configured
- ✅ Provides deployment summary
- ✅ Supports manual workflow dispatch

### Customizing the Workflow

To deploy to staging/production:

1. Create separate Fly.io apps:
   ```bash
   flyctl launch --name gsc-tracking-api-staging --region iad
   flyctl launch --name gsc-tracking-api-prod --region iad
   ```

2. Update workflow to deploy to different apps based on branch:
   ```yaml
   - name: Deploy to Staging
     if: github.ref == 'refs/heads/develop'
     run: flyctl deploy --app gsc-tracking-api-staging

   - name: Deploy to Production
     if: github.ref == 'refs/heads/main'
     run: flyctl deploy --app gsc-tracking-api-prod
   ```

---

## Migration from Azure (If Needed)

If migrating from Azure App Service:

1. **Export Azure SQL Database**
   ```bash
   # Use Azure Data Studio or SSMS
   # Or use SQL export
   ```

2. **Create Fly.io Postgres**
   ```bash
   flyctl postgres create --name gsc-tracking-db
   flyctl postgres attach gsc-tracking-db
   ```

3. **Import Database**
   ```bash
   flyctl postgres connect -a gsc-tracking-db
   # Run SQL scripts or use pg_restore
   ```

4. **Update Connection Strings**
   ```bash
   # Connection string is auto-set via DATABASE_URL
   # Or manually set:
   flyctl secrets set ConnectionStrings__DefaultConnection="..."
   ```

5. **Migrate Blob Storage**
   - Use Cloudflare R2 (S3-compatible)
   - Or Tigris (Fly.io partner)
   - Or keep Azure Blob Storage

6. **Test and Verify**
   ```bash
   # Test API endpoints
   curl https://gsc-tracking-api.fly.dev/api/hello
   ```

---

## Support and Resources

### Official Documentation
- **Fly.io Docs:** https://fly.io/docs/
- **Fly.io .NET Guide:** https://fly.io/docs/languages-and-frameworks/dotnet/
- **Fly.io Postgres:** https://fly.io/docs/postgres/

### Community
- **Fly.io Community:** https://community.fly.io/
- **GitHub Discussions:** https://github.com/superfly/flyctl/discussions

### Getting Help

1. Check the [Fly.io documentation](https://fly.io/docs/)
2. Search [community forums](https://community.fly.io/)
3. Review app logs: `flyctl logs`
4. Check app status: `flyctl status`
5. Open a support ticket (Enterprise plan)

---

## Changelog

| Date | Version | Changes |
|------|---------|---------|
| 2025-12-09 | 1.0 | Initial Fly.io deployment setup |

---

**Last Updated:** 2025-12-09  
**Maintained By:** GSC Development Team  
**Status:** ✅ Active
