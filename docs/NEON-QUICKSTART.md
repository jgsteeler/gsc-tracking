# Neon PostgreSQL Quick Start Guide

**Document Version:** 1.0  
**Last Updated:** 2025-12-11  
**Status:** Active

---

## Quick Setup (5 Minutes)

This guide will help you set up Neon PostgreSQL for staging in under 5 minutes.

### Step 1: Create Neon Account (1 minute)

1. Go to https://neon.tech
2. Click "Sign Up"
3. Choose "Continue with GitHub" (recommended)
4. Authorize Neon to access your GitHub account
5. ✅ Done! No credit card required.

### Step 2: Create Staging Database Project (2 minutes)

1. **In Neon Dashboard** (https://console.neon.tech)
   
2. **Click "New Project"**

3. **Configure Project:**
   - **Project name**: `gsc-tracking-staging`
   - **Region**: `US East (Ohio) - aws-us-east-2`
   - **PostgreSQL version**: `16` (latest)
   - **Database name**: `gsctracking`
   - Leave other settings as default

4. **Click "Create Project"**

5. **Copy Connection String:**
   - After creation, you'll see a connection string
   - Copy the **Pooled connection** string
   - Format: `postgresql://user:password@ep-xxx.us-east-2.aws.neon.tech/gsctracking?sslmode=require`
   - Save this securely (you'll need it in Step 3)

### Step 3: Configure Fly.io Staging App (2 minutes)

1. **Set DATABASE_URL secret in Fly.io:**

   ```bash
   flyctl secrets set DATABASE_URL="postgresql://[user]:[password]@[host]/gsctracking?sslmode=require" \
     --app gsc-tracking-api-staging
   ```

   Replace `[user]`, `[password]`, and `[host]` with values from your Neon connection string.

2. **Verify secret is set:**

   ```bash
   flyctl secrets list --app gsc-tracking-api-staging
   ```

   You should see `DATABASE_URL` listed (value is hidden).

### Step 4: Apply Migrations to Staging Database (Optional)

If you want to test migrations immediately:

```bash
# Set connection string locally
export DATABASE_URL="postgresql://[user]:[password]@[host]/gsctracking?sslmode=require"

# Navigate to backend
cd backend/GscTracking.Api

# Apply migrations
dotnet ef database update

# Verify
dotnet ef migrations list
```

**Output:**
```
20251209034037_InitialCreate (Applied)
```

### Step 5: Test Staging Deployment

1. **Deploy staging app (or wait for next PR deployment):**

   ```bash
   cd backend
   flyctl deploy --config fly.staging.toml --remote-only
   ```

2. **Verify app is running:**

   ```bash
   curl https://gsc-tracking-api-staging.fly.dev/api/hello
   ```

   Expected response:
   ```json
   {
     "message": "Hello from GSC Tracking API!",
     "version": "1.0.0",
     "timestamp": "2025-12-11T..."
   }
   ```

3. **Check logs:**

   ```bash
   flyctl logs --app gsc-tracking-api-staging
   ```

   Look for successful database connection logs.

## ✅ You're Done!

Your staging database is now configured and ready to use.

---

## What You Get with Neon Free Tier

- ✅ **3GB Storage** - More than enough for staging
- ✅ **191 Hours Compute/Month** - ~6 hours per day (auto-suspend saves hours)
- ✅ **PostgreSQL 16** - Latest PostgreSQL version
- ✅ **Auto-suspend** - Database suspends after 5 minutes of inactivity
- ✅ **Instant wake-up** - < 500ms cold start
- ✅ **7-day history** - Point-in-time recovery
- ✅ **No credit card required** - Completely free

---

## Neon Dashboard Overview

Access your Neon dashboard at: https://console.neon.tech

### Key Features

**1. Monitoring Tab**
- View active connections
- Monitor CPU and memory usage
- Track storage usage
- View query performance

**2. Branches Tab**
- Create database branches for PR previews
- Each branch is a copy-on-write clone
- Perfect for testing migrations

**3. Operations Tab**
- View recent database operations
- Monitor suspends and resumes
- Track scaling events

**4. Settings Tab**
- Configure auto-suspend timing
- Set up compute limits
- Manage connection pooling
- Configure backups

---

## Testing Database Connection

### From Local Machine

```bash
# Using psql
psql "postgresql://user:pass@host/gsctracking?sslmode=require"

# Run test query
psql "postgresql://user:pass@host/gsctracking?sslmode=require" -c "SELECT version();"

# List tables
psql "postgresql://user:pass@host/gsctracking?sslmode=require" -c "\dt"

# View migration history
psql "postgresql://user:pass@host/gsctracking?sslmode=require" -c "SELECT * FROM __EFMigrationsHistory;"
```

### From Fly.io Staging App

```bash
# SSH into staging app
flyctl ssh console --app gsc-tracking-api-staging

# Test connection (DATABASE_URL is already set)
psql $DATABASE_URL -c "SELECT version();"

# Exit
exit
```

---

## Common Tasks

### View Database Metrics

```bash
# In Neon Dashboard:
# 1. Go to https://console.neon.tech
# 2. Select your project
# 3. Click "Monitoring" tab
# 4. View real-time metrics
```

### Create a Database Branch (for PR Testing)

```bash
# Install Neon CLI
npm install -g neonctl

# Login
neonctl auth

# Create branch
neonctl branches create --name pr-123 --parent main

# Get connection string for branch
neonctl connection-string pr-123

# Delete branch after testing
neonctl branches delete pr-123
```

### Backup Database

```bash
# Manual backup using pg_dump
pg_dump "postgresql://user:pass@host/gsctracking?sslmode=require" > staging_backup.sql

# Or create a Neon branch (instant backup)
neonctl branches create --name backup-$(date +%Y%m%d)
```

### Restore Database

```bash
# From SQL file
psql "postgresql://user:pass@host/gsctracking?sslmode=require" < staging_backup.sql

# Or restore from Neon branch
neonctl branches create --name restored --parent backup-20251211
```

---

## Troubleshooting

### Database Connection Fails

**Error:** `connection refused` or `timeout`

**Solution:**
```bash
# 1. Verify connection string
echo $DATABASE_URL

# 2. Check if database is suspended (wait 2-3 seconds for wake-up)
# Neon auto-suspends after 5 minutes of inactivity

# 3. Test with psql
psql $DATABASE_URL -c "SELECT 1;"

# 4. Check Neon dashboard for issues
# Go to https://console.neon.tech
```

### SSL Certificate Error

**Error:** `SSL error: certificate verify failed`

**Solution:**
```bash
# Ensure connection string includes sslmode=require
# Correct format:
postgresql://user:pass@host/db?sslmode=require

# Or disable SSL verification (NOT recommended for production)
postgresql://user:pass@host/db?sslmode=require&sslrootcert=none
```

### Migration Fails

**Error:** Migration fails to apply

**Solution:**
```bash
# 1. Check connection
psql $DATABASE_URL -c "SELECT version();"

# 2. Check migration status
dotnet ef migrations list

# 3. Apply with verbose logging
dotnet ef database update --verbose

# 4. Check Neon dashboard for errors
```

### App Can't Connect to Database

**Error:** App logs show database connection errors

**Solution:**
```bash
# 1. Verify Fly.io secret is set
flyctl secrets list --app gsc-tracking-api-staging

# 2. Restart app
flyctl apps restart gsc-tracking-api-staging

# 3. Check logs
flyctl logs --app gsc-tracking-api-staging

# 4. SSH into app and test connection
flyctl ssh console --app gsc-tracking-api-staging
psql $DATABASE_URL -c "SELECT 1;"
exit
```

---

## Next Steps

✅ **Completed:** Staging database set up with Neon  
⏳ **Next:** Apply migrations and test your API endpoints  
⏳ **Future:** Set up database branching for PR previews  
⏳ **Future:** Configure production database (Neon Pro or Azure)

---

## Resources

- **Neon Documentation**: https://neon.tech/docs
- **Neon Dashboard**: https://console.neon.tech
- **Neon Discord Community**: https://discord.gg/neon
- **Full Database Setup Guide**: [DATABASE-SETUP.md](./DATABASE-SETUP.md)
- **Migration Guide**: [DATABASE-MIGRATION-GUIDE.md](./DATABASE-MIGRATION-GUIDE.md)

---

## Support

**Need help?**
- Check [DATABASE-SETUP.md](./DATABASE-SETUP.md) for detailed documentation
- Join Neon Discord: https://discord.gg/neon
- Open GitHub Discussion: https://github.com/jgsteeler/gsc-tracking/discussions

---

**Last Updated:** 2025-12-11  
**Maintained By:** GSC Development Team  
**Status:** ✅ Ready to Use
