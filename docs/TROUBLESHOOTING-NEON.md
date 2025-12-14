# Neon PostgreSQL Troubleshooting Guide

**Quick Reference for Common Issues**

---

## 🔴 Connection Refused / Timeout

**Symptoms:**
- `Npgsql.NpgsqlException: Connection timeout`
- `System.Net.Sockets.SocketException: Connection refused`

**Cause:**
Neon database is suspended (auto-suspend after 5 minutes of inactivity on free tier)

**Solution:**
✅ **Wait 2-3 seconds** - Database automatically wakes up on first connection attempt

```bash
# Test connection (will wake up database)
psql "$DATABASE_URL" -c "SELECT 1;"

# Then try again
dotnet ef database update
```

**Prevention:**
- Use paid plan to disable auto-suspend
- Keep a ping service running (not recommended for free tier)

---

## 🔴 SSL/TLS Certificate Errors

**Symptoms:**
- `Npgsql.NpgsqlException: The remote certificate is invalid`
- `SSL connection requested but server only allows non-SSL connections`

**Solution:**
✅ **Always include `sslmode=require`** in Neon connection strings:

```bash
# Correct format
postgresql://user:pass@host/db?sslmode=require

# Add to existing URL
postgresql://user:pass@host/db?sslmode=require&other=params
```

**For standard format:**
```
Host=host;Database=db;Username=user;Password=pass;SSL Mode=Require
```

---

## 🔴 Authentication Failed (Password Error)

**Symptoms:**
- `Npgsql.NpgsqlException: 28P01: password authentication failed`

**Common Causes & Solutions:**

### 1. Special Characters in Password

❌ **Wrong:**
```bash
postgresql://user:pass@word123@host/db
```

✅ **Correct (URL-encoded):**
```bash
postgresql://user:pass%40word123@host/db
```

**URL Encoding Reference:**
- `@` → `%40`
- `#` → `%23`
- `%` → `%25`
- `&` → `%26`
- `/` → `%2F`
- `?` → `%3F`

### 2. Use Standard Format Instead

✅ **Best for passwords with special characters:**
```
Host=host;Database=db;Username=user;Password=pass@word123;SSL Mode=Require
```

### 3. Get Fresh Credentials

```bash
# Neon Dashboard → Your Project → Connection Details
# Copy the connection string directly
```

---

## 🔴 Database Does Not Exist

**Symptoms:**
- `Npgsql.NpgsqlException: 3D000: database "gsctracking" does not exist`

**Solution:**
✅ **Verify database name** in Neon Dashboard

1. Go to https://console.neon.tech
2. Select your project
3. Check "Database" name (default: `neondb`, not `gsctracking`)

**Fix connection string:**
```bash
# If default database name is 'neondb'
postgresql://user:pass@host/neondb?sslmode=require

# Or create the database
psql "postgresql://user:pass@host/neondb?sslmode=require" -c "CREATE DATABASE gsctracking;"
```

---

## 🔴 Migrations Fail with Type Errors

**Symptoms:**
- `Npgsql.NpgsqlException: type "INTEGER" does not exist`
- `Npgsql.NpgsqlException: type "TEXT" does not exist`

**Cause:**
Old SQLite-specific migrations

**Solution:**
✅ **Migrations are now database-agnostic** (as of commit 3c5a9b4)

If you still see errors:

1. **Pull latest changes:**
   ```bash
   git pull origin main
   ```

2. **Remove old migrations** (if you created them before the fix):
   ```bash
   cd backend/GscTracking.Api
   rm -rf Migrations/
   ```

3. **Regenerate migrations:**
   ```bash
   dotnet ef migrations add InitialCreate
   ```

4. **Apply to Neon:**
   ```bash
   export DATABASE_URL="postgresql://..."
   dotnet ef database update
   ```

---

## 🔴 Connection Pool Exhausted

**Symptoms:**
- `Npgsql.NpgsqlException: The connection pool has been exhausted`
- Too many open connections

**Solution:**
✅ **Connection pooling with retry logic is now enabled by default**

If issue persists:

1. **Use Neon's pooled connection:**
   ```bash
   # Look for "Pooled connection" in Neon Dashboard
   postgresql://user:pass@ep-xxx-pooler.region.aws.neon.tech/db?sslmode=require
   ```

2. **Increase pool size** (if needed):
   ```bash
   postgresql://user:pass@host/db?sslmode=require&Maximum Pool Size=50
   ```

3. **Check for connection leaks** in your code:
   - Ensure DbContext is disposed (`using` statements)
   - Use dependency injection (scoped lifetime)

---

## 🔴 Migrations Hang or Timeout

**Symptoms:**
- `dotnet ef database update` hangs indefinitely
- Command times out after 30 seconds

**Solution:**

1. **Check Neon database status:**
   - Go to https://console.neon.tech
   - Check if database is active

2. **Increase command timeout:**
   ```bash
   # Already configured in Program.cs to 30 seconds
   # If still timing out, check network connectivity
   ```

3. **Test connection first:**
   ```bash
   psql "$DATABASE_URL" -c "SELECT version();"
   ```

4. **Use verbose mode:**
   ```bash
   dotnet ef database update --verbose
   ```

---

## 🔴 Environment Variable Not Set

**Symptoms:**
- `InvalidOperationException: No database connection string found`

**Solution:**

### Check if variable is set:
```bash
# Bash/Linux/macOS
echo $DATABASE_URL

# PowerShell
echo $env:DATABASE_URL
```

### Set the variable:

**Bash/Linux/macOS:**
```bash
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"
```

**PowerShell:**
```powershell
$env:DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"
```

**Fly.io:**
```bash
flyctl secrets set DATABASE_URL="postgresql://..." --app gsc-tracking-api-staging
```

---

## 🔴 Docker Compose Connection Issues

**Symptoms:**
- Backend container cannot connect to `database` container

**Solution:**

1. **Use service name as hostname:**
   ```yaml
   environment:
     DATABASE_URL: "Host=database;Port=5432;Database=gsc_tracking;Username=gscadmin;Password=gscpassword123"
   ```

2. **Wait for database to be ready:**
   ```bash
   docker-compose up -d database
   # Wait 10-15 seconds for PostgreSQL to start
   docker-compose up backend
   ```

3. **Check health status:**
   ```bash
   docker-compose ps
   # Ensure database shows "healthy"
   ```

---

## 🟢 Testing Connection

### Quick Connection Test

```bash
# Set connection string
export DATABASE_URL="postgresql://user:pass@host/db?sslmode=require"

# Test with psql (install if needed)
psql "$DATABASE_URL" -c "SELECT version();"

# Expected: PostgreSQL 16.x
```

### Test Application Connection

```bash
cd backend/GscTracking.Api

# Test migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update

# Run application
dotnet run

# Test API
curl http://localhost:5091/api/hello
```

---

## 🟢 Common Commands

### Neon CLI

```bash
# Install
npm install -g neonctl

# Login
neonctl auth

# List projects
neonctl projects list

# Get connection string
neonctl connection-string <branch-name>

# Create database branch
neonctl branches create --name pr-123

# Delete branch
neonctl branches delete pr-123
```

### EF Core Migrations

```bash
# List migrations
dotnet ef migrations list

# Create migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigration

# Remove last migration (not applied)
dotnet ef migrations remove

# Verbose output
dotnet ef database update --verbose
```

---

## 📚 Additional Resources

- **Full PostgreSQL Guide**: [POSTGRESQL-CONNECTION-GUIDE.md](./POSTGRESQL-CONNECTION-GUIDE.md)
- **Database Setup**: [DATABASE-SETUP.md](./DATABASE-SETUP.md)
- **Neon Quick Start**: [NEON-QUICKSTART.md](./NEON-QUICKSTART.md)
- **Neon Documentation**: https://neon.tech/docs
- **Neon Discord**: https://discord.gg/neon

---

## 🆘 Still Having Issues?

1. **Check all items in troubleshooting checklist:**
   - [ ] Connection string format is correct
   - [ ] SSL mode is set to `require`
   - [ ] Database name matches Neon dashboard
   - [ ] Username and password are correct
   - [ ] Special characters in password are URL-encoded (or use standard format)
   - [ ] Neon database is active (wait 2-3 seconds if suspended)
   - [ ] Network connectivity is working
   - [ ] Environment variable is set correctly
   - [ ] Latest migrations are pulled from git

2. **Test with psql** to isolate application vs. connection issues

3. **Check Neon dashboard** for:
   - Database status (active/suspended)
   - Connection limits
   - Error logs

4. **Open a GitHub Issue** with:
   - Exact error message
   - Connection string format (redacted password)
   - Steps to reproduce

---

**Last Updated:** 2025-12-14  
**Maintained By:** GSC Development Team
