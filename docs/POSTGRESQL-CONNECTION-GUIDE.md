# PostgreSQL Connection Guide - EF Core with Neon

**Document Version:** 1.0  
**Last Updated:** 2025-12-14  
**Status:** Active

---

## Overview

This guide provides comprehensive instructions for connecting Entity Framework Core to PostgreSQL databases, with specific focus on Neon PostgreSQL serverless databases.

## ⚠️ Important: About SQLite Migrations

**The migrations in this repository were generated using SQLite (local development database) and contain SQLite-specific types like `INTEGER` and `TEXT`. This is expected and correct behavior.**

### Why SQLite Migrations Work with PostgreSQL

- **Entity Framework Core automatically translates** SQLite types to PostgreSQL types at runtime
- The DbContext is configured to be **database-agnostic** (using `HasPrecision()` instead of `HasColumnType()`)
- When you run `dotnet ef database update` with PostgreSQL, EF Core creates PostgreSQL-compatible tables

### Type Translations

| SQLite (Migration Files) | PostgreSQL (Runtime) |
|--------------------------|----------------------|
| INTEGER                  | integer              |
| TEXT                     | text/varchar         |
| REAL                     | double precision     |
| BLOB                     | bytea                |

**✅ You can safely apply SQLite migrations to PostgreSQL without modification.**

See the [Migration Workflow](#migration-workflow) section for more details.

---

## Connection String Formats

The application supports multiple PostgreSQL connection string formats:

### 1. PostgreSQL URL Format (Neon Default)

```
postgresql://username:password@hostname:port/database?sslmode=require
```

**Example (Neon):**
```
postgresql://user:pass@ep-cool-voice-12345.us-east-2.aws.neon.tech/gsctracking?sslmode=require
```

### 2. Standard Format

```
Host=hostname;Port=5432;Database=database;Username=username;Password=password;SSL Mode=Require
```

**Example:**
```
Host=ep-cool-voice-12345.us-east-2.aws.neon.tech;Port=5432;Database=gsctracking;Username=user;Password=pass;SSL Mode=Require
```

### 3. Docker Compose Format

```
Host=database;Port=5432;Database=gsc_tracking;Username=gscadmin;Password=gscpassword123
```

## Configuration

### Environment Variables

The application reads the database connection string from:

1. **DATABASE_URL** environment variable (priority)
2. **ConnectionStrings:DefaultConnection** in appsettings.json (fallback)

### Setting DATABASE_URL

**Local Development (PowerShell):**
```powershell
$env:DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"
dotnet run
```

**Local Development (Bash):**
```bash
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"
dotnet run
```

**Fly.io (Staging/Production):**
```bash
flyctl secrets set DATABASE_URL="postgresql://user:pass@host/db?sslmode=require" --app gsc-tracking-api-staging
```

**Docker Compose:**
```yaml
environment:
  DATABASE_URL: "Host=database;Port=5432;Database=gsc_tracking;Username=gscadmin;Password=gscpassword123"
```

## Common Connection Issues and Solutions

### Issue 1: SSL/TLS Certificate Errors

**Error:**
```
Npgsql.NpgsqlException: SSL connection requested but server only allows non-SSL connections
```

**Solution:**
Add `sslmode=require` to your connection string:
```
postgresql://user:pass@host/db?sslmode=require
```

For Neon, SSL is **required** and should always be included.

---

### Issue 2: Connection Timeout

**Error:**
```
Npgsql.NpgsqlException: Connection timeout
```

**Causes:**
- Database is suspended (Neon auto-suspends after 5 minutes)
- Network connectivity issues
- Incorrect hostname

**Solutions:**

1. **Wait for Neon wake-up** (2-3 seconds):
   ```bash
   # Test connection
   psql "$DATABASE_URL" -c "SELECT 1;"
   ```

2. **Check connection string**:
   ```bash
   echo $DATABASE_URL
   ```

3. **Verify hostname**:
   ```bash
   ping ep-cool-voice-12345.us-east-2.aws.neon.tech
   ```

---

### Issue 3: Authentication Failed

**Error:**
```
Npgsql.NpgsqlException: 28P01: password authentication failed for user "username"
```

**Solutions:**

1. **Verify credentials**:
   - Check username and password in Neon dashboard
   - Ensure no extra spaces in connection string

2. **URL-encode special characters in password**:
   ```bash
   # If password contains @, #, %, etc.
   # Example: pass@123 becomes pass%40123
   postgresql://user:pass%40123@host/db?sslmode=require
   ```

3. **Use standard format instead**:
   ```
   Host=host;Database=db;Username=user;Password=pass@123;SSL Mode=Require
   ```

---

### Issue 4: Database Does Not Exist

**Error:**
```
Npgsql.NpgsqlException: 3D000: database "gsctracking" does not exist
```

**Solutions:**

1. **Verify database name** in Neon dashboard

2. **Create database**:
   ```bash
   # Connect to default database
   psql "postgresql://user:pass@host/postgres?sslmode=require"
   
   # Create database
   CREATE DATABASE gsctracking;
   ```

---

### Issue 5: Migrations Fail with Type Errors

**Error:**
```
Npgsql.NpgsqlException: type "INTEGER" does not exist in PostgreSQL
```

**Cause:**
Migrations were created for SQLite and contain SQLite-specific types (INTEGER, TEXT).

**Solution:**
Migrations are now database-agnostic. If you still see this error:

1. **Delete old migrations**:
   ```bash
   cd backend/GscTracking.Api
   rm -rf Migrations/
   ```

2. **Create new migrations with PostgreSQL as target**:
   ```bash
   export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"
   dotnet ef migrations add InitialCreate
   ```

3. **Apply migrations**:
   ```bash
   dotnet ef database update
   ```

---

### Issue 6: Connection Pool Exhaustion

**Error:**
```
Npgsql.NpgsqlException: The connection pool has been exhausted
```

**Cause:**
Too many open connections, not properly disposed.

**Solutions:**

1. **Connection pooling is now enabled by default** with retry logic

2. **Increase max pool size** (if needed):
   ```
   postgresql://user:pass@host/db?sslmode=require&Maximum Pool Size=50
   ```

3. **Check for connection leaks** in code

---

### Issue 7: Neon Connection String Not Recognized

**Error:**
```
InvalidOperationException: Unsupported database connection string format
```

**Solution:**
The application now supports:
- `postgresql://` (Neon format)
- `postgres://` (alternative format)
- `Host=` (standard format)

Ensure your connection string starts with one of these.

---

## Testing Connection

### Test with psql

```bash
# Install psql (if not already installed)
# Ubuntu/Debian:
sudo apt-get install postgresql-client

# macOS:
brew install postgresql

# Test connection
psql "$DATABASE_URL" -c "SELECT version();"
```

**Expected output:**
```
PostgreSQL 16.x on x86_64-pc-linux-gnu, compiled by gcc...
```

### Test with EF Core

```bash
cd backend/GscTracking.Api

# Set connection string
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"

# Test migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update

# Run application
dotnet run
```

### Test with curl

```bash
# Start application
dotnet run --urls http://localhost:5091

# In another terminal, test API
curl http://localhost:5091/api/hello
```

---

## Neon-Specific Configuration

### Connection Pooling

Neon provides **connection pooling** through PgBouncer:

```
postgresql://user:pass@ep-xxx-pooler.region.aws.neon.tech/db?sslmode=require
```

**When to use:**
- **Pooled** (recommended): For application connections, better for serverless
- **Direct**: For migrations and admin tasks

### Auto-Suspend Behavior

Neon databases auto-suspend after 5 minutes of inactivity (free tier).

**First connection after suspend:**
- Takes 2-3 seconds to wake up
- Subsequent connections are instant
- Application handles this automatically with retry logic

**Disable auto-suspend:**
- Not available on free tier
- Available on paid plans ($19/month)

### Branch Databases

Create database branches for PR testing:

```bash
# Install Neon CLI
npm install -g neonctl

# Login
neonctl auth

# Create branch
neonctl branches create --name pr-123

# Get connection string
neonctl connection-string pr-123

# Delete branch
neonctl branches delete pr-123
```

---

## Docker Compose Configuration

The `docker-compose.yml` is configured for local PostgreSQL:

```yaml
backend:
  environment:
    ConnectionStrings__DefaultConnection: "Host=database;Port=5432;Database=gsc_tracking;Username=gscadmin;Password=gscpassword123"
```

**Start local PostgreSQL:**
```bash
docker-compose up -d database
```

**Connect to local PostgreSQL:**
```bash
export DATABASE_URL="Host=localhost;Port=5432;Database=gsc_tracking;Username=gscadmin;Password=gscpassword123"
dotnet ef database update
```

---

## Application Configuration

### Program.cs Changes

The application now includes:

1. **Connection retry logic**: Automatically retries on transient errors
2. **Command timeout**: 30 seconds
3. **Multiple format support**: URL and standard formats
4. **Detailed logging**: In development mode

### ApplicationDbContext Changes

The DbContext now uses:

1. **HasPrecision()** instead of **HasColumnType()** for decimal fields
2. **Database-agnostic** configuration
3. **Cross-compatible** with SQLite and PostgreSQL

---

## Best Practices

### Security

1. **Never commit connection strings** to source control
2. **Use environment variables** or secret management
3. **Always use SSL** for production (`sslmode=require`)
4. **Rotate credentials** regularly (every 90 days)

### Performance

1. **Use connection pooling** (enabled by default)
2. **Use Neon pooled connections** for better performance
3. **Enable retry logic** (already configured)
4. **Monitor connection counts** in Neon dashboard

### Development

1. **Use SQLite locally** (zero configuration)
2. **Use Neon for staging** (free tier available)
3. **Test migrations locally** before applying to staging
4. **Create database branches** for PR testing

---

## Migration Workflow

### Local Development (SQLite)

```bash
cd backend/GscTracking.Api

# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Test
dotnet run
```

**Note:** Migrations generated with SQLite will contain SQLite-specific types (INTEGER, TEXT) but will work with PostgreSQL at runtime because EF Core translates them appropriately.

### Staging (Neon PostgreSQL)

```bash
# Set connection string
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"

# Apply migration (existing SQLite migrations work fine)
dotnet ef database update

# Verify
dotnet ef migrations list

# Deploy
flyctl deploy --config fly.staging.toml
```

**Alternative:** Generate PostgreSQL-specific migrations:
```bash
# Remove SQLite migrations
rm -rf Migrations/

# Set PostgreSQL connection
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"

# Generate with PostgreSQL provider
dotnet ef migrations add InitialCreate

# Apply
dotnet ef database update
```

### Cross-Database Compatibility

The application DbContext is configured to work with both SQLite and PostgreSQL:

- **Decimal fields**: Use `HasPrecision(18, 2)` instead of `HasColumnType()`
- **String fields**: Use `HasMaxLength()` which works on both
- **Primary keys**: Auto-increment is handled automatically by each provider
- **Timestamps**: DateTime works on both (stored as TEXT in SQLite, timestamp in PostgreSQL)

**Migration type mapping:**
| SQLite (Migration) | PostgreSQL (Runtime) |
|--------------------|----------------------|
| INTEGER            | integer              |
| TEXT               | text/varchar         |
| REAL               | double precision     |
| BLOB               | bytea                |

EF Core handles the translation automatically when you apply migrations to PostgreSQL.

---

## Troubleshooting Checklist

When connection fails, check:

- [ ] Connection string format is correct
- [ ] SSL mode is set to `require` for Neon
- [ ] Database name exists in Neon dashboard
- [ ] Username and password are correct
- [ ] Special characters in password are URL-encoded
- [ ] Neon database is not suspended (wait 2-3 seconds)
- [ ] Network connectivity is working
- [ ] Migrations are applied
- [ ] Environment variable is set correctly

---

## Additional Resources

- **Neon Documentation**: https://neon.tech/docs
- **Npgsql Documentation**: https://www.npgsql.org/
- **EF Core Documentation**: https://learn.microsoft.com/en-us/ef/core/
- **PostgreSQL Documentation**: https://www.postgresql.org/docs/16/

---

## Support

**Need help?**
- Check [DATABASE-SETUP.md](./DATABASE-SETUP.md) for general database setup
- Check [NEON-QUICKSTART.md](./NEON-QUICKSTART.md) for quick Neon setup
- Join Neon Discord: https://discord.gg/neon
- Open GitHub Issue: https://github.com/jgsteeler/gsc-tracking/issues

---

**Last Updated:** 2025-12-14  
**Maintained By:** GSC Development Team  
**Status:** ✅ Ready to Use
