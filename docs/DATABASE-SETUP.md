# Database Setup Guide - GSC Tracking

**Document Version:** 1.0  
**Last Updated:** 2025-12-11  
**Status:** Active

---

## Table of Contents

1. [Overview](#overview)
2. [Database Choice Rationale](#database-choice-rationale)
3. [Environment Setup](#environment-setup)
   - [Local Development](#local-development)
   - [Staging (Neon PostgreSQL)](#staging-neon-postgresql)
   - [Production Planning](#production-planning)
4. [Connection Configuration](#connection-configuration)
5. [Database Schema and Migrations](#database-schema-and-migrations)
6. [Backup and Restore Procedures](#backup-and-restore-procedures)
7. [Monitoring and Alerts](#monitoring-and-alerts)
8. [Security Best Practices](#security-best-practices)
9. [Troubleshooting](#troubleshooting)

---

## Overview

The GSC Tracking application uses PostgreSQL as its primary database for staging and production environments, with SQLite for local development. This document provides comprehensive guidance for setting up and managing databases across all environments.

### Quick Reference

| Environment | Database | Hosting | Purpose |
|-------------|----------|---------|---------|
| **Local Development** | SQLite | Local file | Quick development, no external dependencies |
| **Staging** | PostgreSQL 16 | Neon | Testing, PR previews, integration testing |
| **Production** | PostgreSQL 16 | Neon (planned) or Azure PostgreSQL | Live application data |

---

## Database Choice Rationale

### Why PostgreSQL?

1. **Reliability and ACID Compliance**
   - Full ACID compliance ensures data integrity
   - Battle-tested in production environments
   - Strong consistency guarantees

2. **Rich Feature Set**
   - Advanced data types (JSON, arrays, UUID)
   - Full-text search capabilities
   - Robust indexing options
   - Excellent query optimizer

3. **Entity Framework Core Support**
   - First-class support in EF Core
   - Comprehensive migration tooling
   - Performance optimizations

4. **Scalability**
   - Vertical scaling: Up to TB-scale databases
   - Horizontal scaling: Read replicas, partitioning
   - Connection pooling support

5. **Cost-Effective**
   - Open-source with no licensing fees
   - Generous free tiers available (Neon, Supabase)
   - Predictable pricing at scale

### Why Neon for Staging?

1. **Developer Experience**
   - Instant database creation (< 1 second)
   - Branch databases for each PR (database-per-branch)
   - Generous free tier (3GB storage, 191 hours compute/month)
   - PostgreSQL 16 with latest features

2. **Serverless Architecture**
   - Auto-suspend after 5 minutes of inactivity
   - Instant activation on query
   - Scale to zero = zero cost when not in use
   - Separated storage and compute

3. **GitHub Integration**
   - Easy integration with GitHub Actions
   - API for automated database creation
   - Branch databases for preview deployments

4. **Performance**
   - Fast cold starts (< 500ms)
   - Built on PostgreSQL 16
   - Connection pooling included
   - Read replicas available

5. **Cost**
   - **Free Tier**: 3GB storage, 191 hours compute/month
   - **Paid**: $19/month for 10GB + unlimited compute
   - No surprise charges with scale-to-zero

### Why SQLite for Local Development?

1. **Zero Configuration**
   - No installation required
   - File-based, no server needed
   - Perfect for quick local development

2. **Fast Iterations**
   - Instant startup
   - Easy to reset (delete file)
   - No network latency

3. **Testing**
   - In-memory databases for unit tests
   - Fast test execution
   - Isolated test environments

---

## Environment Setup

### Local Development

**Database:** SQLite (file-based)

#### Configuration

The application is already configured for SQLite in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gsctracking.db"
  }
}
```

#### Usage

```bash
# Navigate to backend
cd backend/GscTracking.Api

# Run migrations (creates SQLite database)
dotnet ef database update

# Run the application
dotnet run
```

The database file `gsctracking.db` will be created in the project directory.

#### Reset Local Database

```bash
# Delete the SQLite database file
rm gsctracking.db

# Recreate from migrations
dotnet ef database update
```

---

### Staging (Neon PostgreSQL)

**Database:** PostgreSQL 16 (Neon Serverless)  
**Hosting:** Neon (https://neon.tech)  
**Cost:** Free tier (3GB storage, 191 hours compute/month)

#### Step 1: Create Neon Account

1. Visit https://neon.tech
2. Sign up with GitHub (recommended for integration)
3. Verify your email address
4. No credit card required for free tier

#### Step 2: Create Staging Database Project

1. **Log in to Neon Dashboard**
   - Go to https://console.neon.tech

2. **Create New Project**
   - Click "New Project"
   - **Name**: `gsc-tracking-staging`
   - **Region**: US East (Ohio) - `aws-us-east-2` (closest to Fly.io `iad` region)
   - **PostgreSQL Version**: 16 (latest)
   - Click "Create Project"

3. **Get Connection String**
   - After creation, copy the connection string
   - Format: `postgresql://[user]:[password]@[endpoint]/[dbname]?sslmode=require`
   - Example: `postgresql://gscuser:abc123@ep-cool-voice-12345.us-east-2.aws.neon.tech/gsctracking?sslmode=require`

#### Step 3: Configure Connection Pooling

Neon provides connection pooling for better performance:

1. In Neon Dashboard, go to your project
2. Click on "Connection Details"
3. Enable "Pooled connection"
4. Copy the pooled connection string (port 5432 with `?sslmode=require&pooler=true`)

**Pooled vs Direct Connection:**
- **Pooled** (recommended): Better for serverless, handles many concurrent connections
- **Direct**: Use for migrations and administrative tasks

#### Step 4: Set Up Secrets in Fly.io (Staging)

Add the database connection string as a secret in Fly.io staging app:

```bash
# Set database URL for staging app
flyctl secrets set DATABASE_URL="postgresql://[user]:[password]@[endpoint]/[dbname]?sslmode=require" \
  --app gsc-tracking-api-staging

# Verify secret is set (value will be hidden)
flyctl secrets list --app gsc-tracking-api-staging
```

#### Step 5: Update Backend Configuration

The backend will automatically use the `DATABASE_URL` environment variable. Update `Program.cs` to support PostgreSQL:

```csharp
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Determine database provider based on connection string
if (connectionString?.StartsWith("Data Source=") == true)
{
    // SQLite for local development
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    // PostgreSQL for staging/production
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
```

#### Step 6: Install Npgsql Provider

Add PostgreSQL provider to the project:

```bash
cd backend/GscTracking.Api
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### Step 7: Run Migrations on Staging

```bash
# Set connection string locally for migration
export DATABASE_URL="postgresql://[user]:[password]@[endpoint]/[dbname]?sslmode=require"

# Run migrations
dotnet ef database update

# Or run migrations from deployed app
flyctl ssh console --app gsc-tracking-api-staging
cd /app
dotnet ef database update
```

#### Neon Features for Staging

**1. Database Branching (PR Preview Databases)**

Neon supports creating branch databases for each pull request:

```bash
# Create a branch database from main
neonctl branches create --name pr-123 --parent main

# Get connection string for branch
neonctl connection-string pr-123

# Delete branch after PR is merged
neonctl branches delete pr-123
```

**2. Time Travel (Point-in-Time Recovery)**

Neon keeps 7 days of database history (free tier):

```bash
# Restore database to specific timestamp
neonctl branches create --name restored-db \
  --parent main \
  --timestamp "2025-12-10T12:00:00Z"
```

**3. Auto-Suspend**

Staging database automatically suspends after 5 minutes of inactivity (free tier):
- Saves compute hours
- No charges when suspended
- Instant wake-up on first query (< 500ms)

#### Monitoring Staging Database

**Neon Dashboard:**
- Go to https://console.neon.tech
- Select your project
- View metrics: CPU, memory, storage, active connections
- View query logs and slow queries

**Connection Pooling Stats:**
```bash
# View connection pool statistics
flyctl ssh console --app gsc-tracking-api-staging
psql $DATABASE_URL -c "SELECT * FROM pg_stat_database WHERE datname = 'gsctracking';"
```

---

### Production Planning

**Recommended Options:**

1. **Neon PostgreSQL (Recommended for Initial Production)**
   - **Cost**: $19/month (Pro plan) - 10GB storage, unlimited compute
   - **Pros**: Same as staging, proven reliability, easy migration from staging
   - **Cons**: Not yet enterprise-certified (but very stable)
   - **Scaling**: Up to 100GB storage, read replicas, point-in-time recovery

2. **Azure Database for PostgreSQL (Recommended for Scale)**
   - **Cost**: $50-200/month (depending on tier)
   - **Pros**: Enterprise SLA (99.99% uptime), integration with Azure services, managed backups
   - **Cons**: More expensive, complex setup
   - **Scaling**: Up to 4TB storage, read replicas, automatic failover

3. **Supabase (Alternative Serverless Option)**
   - **Cost**: $25/month (Pro plan) - 8GB storage
   - **Pros**: Similar to Neon, includes real-time subscriptions, built-in auth
   - **Cons**: Slightly more expensive than Neon
   - **Scaling**: Up to 100GB storage, read replicas

#### Production Requirements

1. **High Availability**
   - Multiple availability zones
   - Automatic failover
   - 99.9%+ uptime SLA

2. **Backup and Recovery**
   - Daily automated backups
   - Point-in-time recovery (7-35 days)
   - Backup retention policy

3. **Security**
   - Encrypted at rest (AES-256)
   - Encrypted in transit (TLS 1.2+)
   - Private networking (no public access)
   - Regular security patches

4. **Performance**
   - Connection pooling (PgBouncer)
   - Read replicas for reporting
   - Query performance monitoring
   - Slow query logging

5. **Monitoring**
   - Uptime monitoring
   - Performance metrics
   - Alert notifications
   - Log aggregation

#### Migration Path from Staging to Production

When moving from staging (Neon) to production:

**Option 1: Stay with Neon (Recommended for MVP)**
```bash
# 1. Create production project in Neon
neonctl projects create --name gsc-tracking-production --region aws-us-east-2

# 2. Upgrade to Pro plan ($19/month)
# Done via Neon Dashboard: Settings > Billing

# 3. Configure automatic backups (included in Pro)
# Configure in Dashboard: Settings > Backups

# 4. Set up production secrets in Fly.io
flyctl secrets set DATABASE_URL="postgresql://..." --app gsc-tracking-api
```

**Option 2: Migrate to Azure PostgreSQL**
```bash
# 1. Create Azure Database for PostgreSQL
az postgres flexible-server create \
  --resource-group gsc-tracking \
  --name gsc-tracking-db \
  --location eastus \
  --admin-user gscadmin \
  --admin-password <secure-password> \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --version 16

# 2. Export data from Neon
pg_dump $NEON_DATABASE_URL > backup.sql

# 3. Import to Azure
psql $AZURE_DATABASE_URL < backup.sql

# 4. Update Fly.io secrets
flyctl secrets set DATABASE_URL="postgresql://..." --app gsc-tracking-api
```

---

## Connection Configuration

### Environment Variables

The application uses the following environment variables for database connection:

```bash
# Development (SQLite)
ConnectionStrings__DefaultConnection="Data Source=gsctracking.db"

# Staging/Production (PostgreSQL)
DATABASE_URL="postgresql://[user]:[password]@[endpoint]/[dbname]?sslmode=require"
```

### Connection String Formats

**SQLite (Local Development):**
```
Data Source=gsctracking.db
```

**PostgreSQL (Direct Connection):**
```
Host=ep-cool-voice-12345.us-east-2.aws.neon.tech;
Database=gsctracking;
Username=gscuser;
Password=abc123;
SSL Mode=Require;
Trust Server Certificate=true
```

**PostgreSQL (URL Format):**
```
postgresql://gscuser:abc123@ep-cool-voice-12345.us-east-2.aws.neon.tech/gsctracking?sslmode=require
```

**PostgreSQL (Pooled Connection):**
```
postgresql://gscuser:abc123@ep-cool-voice-12345-pooler.us-east-2.aws.neon.tech/gsctracking?sslmode=require&pooler=true
```

### Best Practices

1. **Never commit connection strings to source control**
   - Use environment variables or secret management
   - Add `.env` to `.gitignore`

2. **Use connection pooling in production**
   - Reduces connection overhead
   - Better performance with serverless
   - Neon includes pooling (PgBouncer)

3. **Use SSL/TLS for all connections**
   - Protects data in transit
   - Required by Neon and Azure

4. **Rotate credentials regularly**
   - Change passwords every 90 days
   - Use strong, random passwords
   - Store in secure secret managers (Azure Key Vault, Fly.io secrets)

---

## Database Schema and Migrations

### Current Schema

The application currently has one entity:

**Customer** (`Customer` table):
- `Id` (int, primary key)
- `Name` (string, required, max 200 chars)
- `Email` (string, required, max 200 chars)
- `Phone` (string, required, max 50 chars)
- `Address` (string, required, max 500 chars)
- `Notes` (string, optional, max 2000 chars)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)
- `IsDeleted` (boolean, soft delete)
- `DeletedAt` (datetime, nullable)

### Creating Migrations

```bash
# Create a new migration
cd backend/GscTracking.Api
dotnet ef migrations add MigrationName

# Review the generated migration files
# Located in: Migrations/

# Apply migration to local database
dotnet ef database update

# Apply to specific environment
export DATABASE_URL="postgresql://..."
dotnet ef database update
```

### Migration Best Practices

1. **Test migrations locally first**
   - Always test on SQLite or local PostgreSQL
   - Review generated SQL before applying

2. **Name migrations descriptively**
   - `AddJobsTable` instead of `Update1`
   - Use consistent naming convention

3. **Never modify applied migrations**
   - Create new migrations for changes
   - Maintain migration history

4. **Include rollback logic**
   - Use `Down()` method for rollbacks
   - Test rollback before deploying

5. **Backup before major migrations**
   - Especially in production
   - Test restore process

### Applying Migrations to Environments

**Local Development:**
```bash
cd backend/GscTracking.Api
dotnet ef database update
```

**Staging (Neon):**
```bash
# Option 1: From local machine
export DATABASE_URL="postgresql://..."
dotnet ef database update

# Option 2: From deployed app
flyctl ssh console --app gsc-tracking-api-staging
cd /app
dotnet ef database update
```

**Production:**
```bash
# Always test in staging first
# Backup database before applying
# Apply during maintenance window

export DATABASE_URL="postgresql://..."
dotnet ef database update
```

---

## Backup and Restore Procedures

### Local Development (SQLite)

**Backup:**
```bash
# Simple file copy
cp gsctracking.db gsctracking.db.backup

# Or with timestamp
cp gsctracking.db gsctracking.db.$(date +%Y%m%d_%H%M%S)
```

**Restore:**
```bash
# Restore from backup
cp gsctracking.db.backup gsctracking.db
```

### Staging (Neon PostgreSQL)

**Automated Backups:**
- Neon automatically maintains 7-day history (free tier)
- Pro plan: 7-30 day retention
- Point-in-time recovery included

**Manual Backup (pg_dump):**
```bash
# Export entire database
pg_dump $DATABASE_URL > staging_backup_$(date +%Y%m%d).sql

# Export specific table
pg_dump $DATABASE_URL -t Customer > customer_backup.sql

# Export with data only
pg_dump $DATABASE_URL --data-only > data_backup.sql

# Export schema only
pg_dump $DATABASE_URL --schema-only > schema_backup.sql
```

**Restore from Backup:**
```bash
# Restore entire database
psql $DATABASE_URL < staging_backup_20251211.sql

# Restore specific table
psql $DATABASE_URL < customer_backup.sql
```

**Point-in-Time Recovery (Neon):**
```bash
# Create branch from specific timestamp
neonctl branches create --name restored-staging \
  --parent main \
  --timestamp "2025-12-11T10:00:00Z"

# Get connection string for restored database
neonctl connection-string restored-staging

# Test the restored database
# If good, swap connection strings
```

### Production (Neon Pro or Azure)

**Neon Pro:**
- Automated daily backups
- 7-30 day retention (configurable)
- Point-in-time recovery
- One-click restore from dashboard

**Azure Database for PostgreSQL:**
```bash
# Automated backups are enabled by default
# 7-35 day retention

# Manual backup
az postgres flexible-server backup create \
  --resource-group gsc-tracking \
  --name gsc-tracking-db \
  --backup-name manual-backup-$(date +%Y%m%d)

# List backups
az postgres flexible-server backup list \
  --resource-group gsc-tracking \
  --name gsc-tracking-db

# Restore from backup
az postgres flexible-server restore \
  --resource-group gsc-tracking \
  --name gsc-tracking-db-restored \
  --source-server gsc-tracking-db \
  --restore-time "2025-12-11T10:00:00Z"
```

### Backup Schedule Recommendations

| Environment | Frequency | Retention | Type |
|-------------|-----------|-----------|------|
| **Local Dev** | Manual | N/A | File copy |
| **Staging** | Daily (automated) | 7 days | Neon automatic + weekly manual |
| **Production** | Daily (automated) | 30 days | Automated + weekly manual + pre-deployment |

### Backup Testing

Test backup restoration regularly:

```bash
# Monthly backup test procedure
# 1. Take manual backup
pg_dump $DATABASE_URL > test_backup.sql

# 2. Create test database
neonctl branches create --name backup-test

# 3. Restore backup
psql $TEST_DATABASE_URL < test_backup.sql

# 4. Verify data integrity
psql $TEST_DATABASE_URL -c "SELECT COUNT(*) FROM Customer;"

# 5. Delete test database
neonctl branches delete backup-test
```

---

## Monitoring and Alerts

### Database Health Metrics

Monitor these key metrics:

1. **Connection Count**
   - Warning: > 80% of max connections
   - Critical: > 95% of max connections

2. **Query Performance**
   - Warning: Average query time > 100ms
   - Critical: Average query time > 500ms

3. **Storage Usage**
   - Warning: > 80% capacity
   - Critical: > 90% capacity

4. **CPU Usage**
   - Warning: > 70% sustained
   - Critical: > 85% sustained

5. **Error Rate**
   - Warning: > 1% of queries failing
   - Critical: > 5% of queries failing

### Neon Dashboard Monitoring

**Metrics Available:**
- Active connections
- CPU usage
- Storage usage (GB)
- Compute time used
- Query performance
- Slow queries (> 1 second)

**Access Dashboard:**
1. Go to https://console.neon.tech
2. Select your project
3. View "Monitoring" tab
4. Set up email alerts in Settings

### Application-Level Monitoring

**Log Database Queries (Development):**
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**Track Query Performance:**
```csharp
// Add to Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});
```

### Alert Configuration

**Recommended Alerts:**

1. **Database Down**
   - Check: Health endpoint fails
   - Action: Immediate notification
   - Escalation: 5 minutes

2. **High Connection Count**
   - Check: Connections > 80% of max
   - Action: Email notification
   - Escalation: Review connection pooling

3. **Slow Queries**
   - Check: Query time > 1 second
   - Action: Log and review
   - Escalation: Investigate after 10 occurrences

4. **Storage Near Capacity**
   - Check: Storage > 80% used
   - Action: Email notification
   - Escalation: Plan for upgrade

5. **Failed Migrations**
   - Check: Migration error in logs
   - Action: Immediate notification
   - Escalation: Rollback procedure

### Third-Party Monitoring (Optional)

Consider these tools for production:

1. **Application Insights (Azure)**
   - Database dependency tracking
   - Query performance
   - Failure analysis

2. **Datadog**
   - PostgreSQL monitoring
   - Custom dashboards
   - Alert management

3. **Sentry**
   - Error tracking
   - Performance monitoring
   - Database query insights

---

## Security Best Practices

### Connection Security

1. **Always use SSL/TLS**
   ```
   ?sslmode=require
   ```

2. **Use connection pooling**
   - Reduces connection overhead
   - Prevents connection exhaustion

3. **Limit connection lifetime**
   ```csharp
   options.UseNpgsql(connectionString, npgsqlOptions =>
   {
       npgsqlOptions.CommandTimeout(30);
       npgsqlOptions.EnableRetryOnFailure(3);
   });
   ```

### Access Control

1. **Principle of Least Privilege**
   - Application user: SELECT, INSERT, UPDATE, DELETE
   - Migration user: All permissions
   - Read-only user for reports

2. **Separate Users per Environment**
   - Development: full access
   - Staging: same as production
   - Production: restricted access

3. **Rotate Credentials**
   - Change passwords quarterly
   - Use strong, random passwords
   - Store in secret managers

### Data Protection

1. **Encryption at Rest**
   - Neon: AES-256 encryption (automatic)
   - Azure: Transparent Data Encryption (TDE)

2. **Encryption in Transit**
   - TLS 1.2 or higher
   - Verify certificates in production

3. **Sensitive Data**
   - Hash passwords (never store plain text)
   - Encrypt PII if required by compliance
   - Use data masking for non-production

### Network Security

1. **Neon:**
   - IP allowlisting available (Pro plan)
   - Private networking via Fly.io

2. **Azure PostgreSQL:**
   - Virtual Network (VNet) integration
   - Private endpoints
   - Firewall rules

### Audit Logging

Enable audit logging in production:

```sql
-- Enable logging in PostgreSQL
ALTER DATABASE gsctracking SET log_statement = 'mod';  -- Log all data modifications
ALTER DATABASE gsctracking SET log_connections = 'on'; -- Log connections
ALTER DATABASE gsctracking SET log_disconnections = 'on'; -- Log disconnections
```

---

## Troubleshooting

### Connection Issues

**Symptom: Cannot connect to database**

```bash
# Test connection
psql $DATABASE_URL -c "SELECT version();"

# Check if database is accessible
ping ep-cool-voice-12345.us-east-2.aws.neon.tech

# Verify SSL requirement
psql $DATABASE_URL -c "SHOW ssl;" # Should be "on"
```

**Common Causes:**
- Incorrect connection string
- Database suspended (Neon auto-suspend)
- Network/firewall issues
- SSL certificate issues

**Solutions:**
- Verify connection string format
- Wait 2-3 seconds for Neon wake-up
- Check Fly.io secrets: `flyctl secrets list`
- Add `?sslmode=require` to connection string

### Migration Failures

**Symptom: Migration fails to apply**

```bash
# Check migration status
dotnet ef migrations list

# Get detailed error
dotnet ef database update --verbose

# Check database version
psql $DATABASE_URL -c "SELECT * FROM __EFMigrationsHistory;"
```

**Common Causes:**
- Database locked
- Syntax errors in migration
- Missing permissions
- Duplicate migration

**Solutions:**
- Retry after waiting
- Review migration code
- Check user permissions
- Remove duplicate migration file

### Performance Issues

**Symptom: Slow queries**

```sql
-- Find slow queries
SELECT query, mean_exec_time, calls
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;

-- Check active queries
SELECT pid, usename, state, query, now() - query_start AS duration
FROM pg_stat_activity
WHERE state != 'idle'
ORDER BY duration DESC;

-- Kill long-running query
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE pid = <process_id>;
```

**Common Causes:**
- Missing indexes
- N+1 query problems
- Large result sets
- Connection pool exhaustion

**Solutions:**
- Add indexes to frequently queried columns
- Use `.Include()` for eager loading
- Implement pagination
- Increase connection pool size

### Storage Issues

**Symptom: Running out of storage**

```sql
-- Check database size
SELECT pg_size_pretty(pg_database_size('gsctracking'));

-- Check table sizes
SELECT 
  schemaname,
  tablename,
  pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

**Solutions:**
- Archive old data
- Implement data retention policies
- Upgrade storage capacity
- Optimize data types

### Connection Pool Exhaustion

**Symptom: "Too many connections" error**

```sql
-- Check current connections
SELECT count(*) FROM pg_stat_activity;

-- Check max connections
SHOW max_connections;

-- Check connections by state
SELECT state, count(*) 
FROM pg_stat_activity 
GROUP BY state;
```

**Solutions:**
- Use connection pooling (PgBouncer)
- Reduce connection timeout
- Scale up database (more connections)
- Fix connection leaks in code

---

## Additional Resources

### Official Documentation

- **Neon Documentation**: https://neon.tech/docs
- **PostgreSQL Documentation**: https://www.postgresql.org/docs/16/
- **EF Core PostgreSQL Provider**: https://www.npgsql.org/efcore/
- **Azure PostgreSQL**: https://learn.microsoft.com/en-us/azure/postgresql/

### Tools

- **pgAdmin**: GUI for PostgreSQL management
- **Neon CLI**: `npm install -g neonctl`
- **psql**: PostgreSQL command-line client
- **EF Core CLI**: `dotnet tool install --global dotnet-ef`

### Support

- **Neon Community**: https://neon.tech/discord
- **PostgreSQL Community**: https://www.postgresql.org/support/
- **GitHub Discussions**: https://github.com/jgsteeler/gsc-tracking/discussions

---

## Next Steps

1. ✅ **Set up Neon account** (5 minutes)
2. ✅ **Create staging database** (2 minutes)
3. ✅ **Install Npgsql provider** (1 minute)
4. ✅ **Configure connection strings** (5 minutes)
5. ✅ **Run migrations** (2 minutes)
6. ✅ **Test staging deployment** (10 minutes)
7. ⏳ **Set up monitoring** (15 minutes)
8. ⏳ **Document backup procedures** (10 minutes)
9. ⏳ **Plan production database** (30 minutes)

---

**Last Updated:** 2025-12-11  
**Maintained By:** GSC Development Team  
**Status:** ✅ Active - Ready for Staging Setup
