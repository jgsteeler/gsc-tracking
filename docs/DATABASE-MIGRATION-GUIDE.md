# Database Migration Guide - GSC Tracking

**Document Version:** 1.0  
**Last Updated:** 2025-12-11  
**Status:** Active

---

## Table of Contents

1. [Overview](#overview)
2. [Entity Framework Core Migrations](#entity-framework-core-migrations)
3. [Running Migrations Locally](#running-migrations-locally)
4. [Deploying Migrations to Staging](#deploying-migrations-to-staging)
5. [Deploying Migrations to Production](#deploying-migrations-to-production)
6. [Migration Best Practices](#migration-best-practices)
7. [Common Migration Scenarios](#common-migration-scenarios)
8. [Troubleshooting](#troubleshooting)

---

## Overview

This guide covers database migrations for the GSC Tracking application using Entity Framework Core. Migrations allow you to evolve your database schema over time while preserving existing data.

### Migration Philosophy

- **Code-First Approach**: Define your schema in C# entity classes
- **Version Control**: All migrations are stored in source control
- **Reproducible**: Migrations can be applied to any environment consistently
- **Rollback Support**: Each migration includes both `Up()` and `Down()` methods

### Prerequisites

- .NET 10 SDK installed
- Entity Framework Core CLI tools
- Database connection string (local SQLite or PostgreSQL)

---

## Entity Framework Core Migrations

### Installing EF Core CLI Tools

```bash
# Install globally (one-time setup)
dotnet tool install --global dotnet-ef

# Or update to latest version
dotnet tool update --global dotnet-ef

# Verify installation
dotnet ef --version
```

Expected output: `Entity Framework Core .NET Command-line Tools 10.0.0` (or similar)

### Migration Commands Reference

```bash
# List all migrations
dotnet ef migrations list

# Create a new migration
dotnet ef migrations add <MigrationName>

# Remove the last migration (if not applied)
dotnet ef migrations remove

# Apply migrations to database
dotnet ef database update

# Apply migrations up to specific migration
dotnet ef database update <MigrationName>

# Rollback to previous migration
dotnet ef database update <PreviousMigrationName>

# Generate SQL script without applying
dotnet ef migrations script

# Generate SQL for specific migration range
dotnet ef migrations script <FromMigration> <ToMigration>

# Drop database (development only!)
dotnet ef database drop
```

---

## Running Migrations Locally

### Step 1: Navigate to Project Directory

```bash
cd backend/GscTracking.Api
```

### Step 2: Create a New Migration

```bash
# Example: Adding a Jobs table
dotnet ef migrations add AddJobsTable

# Example: Adding a field to Customer
dotnet ef migrations add AddCustomerStatus
```

**What happens:**
- Creates new files in `Migrations/` folder:
  - `YYYYMMDDHHMMSS_MigrationName.cs` - Migration logic
  - `YYYYMMDDHHMMSS_MigrationName.Designer.cs` - Metadata
  - `ApplicationDbContextModelSnapshot.cs` - Current schema snapshot (updated)

### Step 3: Review the Generated Migration

```bash
# Open the migration file and review the Up() and Down() methods
cat Migrations/YYYYMMDDHHMMSS_AddJobsTable.cs
```

Example migration:
```csharp
public partial class AddJobsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Job",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Job", x => x.Id);
                table.ForeignKey(
                    name: "FK_Job_Customer_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customer",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Job");
    }
}
```

### Step 4: Apply the Migration

```bash
# Apply to local SQLite database
dotnet ef database update

# View applied migrations
dotnet ef migrations list
```

Expected output:
```
20251209034037_InitialCreate (Applied)
20251211120000_AddJobsTable (Applied)
```

### Step 5: Verify the Changes

```bash
# Connect to SQLite database
sqlite3 gsctracking.db

# View tables
.tables

# Describe table structure
.schema Job

# View migration history
SELECT * FROM __EFMigrationsHistory;

# Exit
.exit
```

---

## Deploying Migrations to Staging

### Option 1: Apply Migrations from Local Machine (Recommended for Testing)

```bash
# Set the Neon PostgreSQL connection string
export DATABASE_URL="postgresql://user:password@host.neon.tech/dbname?sslmode=require"

# Navigate to project directory
cd backend/GscTracking.Api

# Apply migrations
dotnet ef database update

# Verify migrations
dotnet ef migrations list
```

**Note:** You need the Neon connection string from the Neon Dashboard.

### Option 2: Apply Migrations from Deployed App (Production-Like)

```bash
# SSH into the Fly.io staging app
flyctl ssh console --app gsc-tracking-api-staging

# Navigate to app directory
cd /app

# Apply migrations (DATABASE_URL is already set via Fly.io secrets)
dotnet ef database update

# Exit SSH session
exit
```

### Option 3: Automated Migrations on Startup (Not Recommended)

You can apply migrations automatically when the app starts by adding this to `Program.cs`:

```csharp
// Apply migrations on startup (USE WITH CAUTION)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
```

**⚠️ Warning:**
- Not recommended for production
- Can cause issues with multiple app instances
- No rollback capability
- Prefer manual or CI/CD-based migration application

### Option 4: CI/CD Pipeline (Future Enhancement)

Add a migration step to GitHub Actions before deployment:

```yaml
- name: Apply Database Migrations
  run: |
    cd backend/GscTracking.Api
    dotnet ef database update
  env:
    DATABASE_URL: ${{ secrets.STAGING_DATABASE_URL }}
```

---

## Deploying Migrations to Production

### Pre-Deployment Checklist

- [ ] Migrations tested in local environment
- [ ] Migrations tested in staging environment
- [ ] Database backup taken
- [ ] Migration scripts reviewed
- [ ] Rollback plan prepared
- [ ] Maintenance window scheduled (if needed)
- [ ] Team notified

### Step 1: Backup Production Database

**Neon PostgreSQL:**
```bash
# Manual backup using pg_dump
pg_dump $PRODUCTION_DATABASE_URL > production_backup_$(date +%Y%m%d_%H%M%S).sql

# Or create a branch in Neon (instant backup)
neonctl branches create --name pre-migration-backup --parent main
```

**Azure PostgreSQL:**
```bash
# Verify automated backups are enabled
az postgres flexible-server backup list \
  --resource-group gsc-tracking \
  --name gsc-tracking-db

# Create manual backup before migration
az postgres flexible-server backup create \
  --resource-group gsc-tracking \
  --name gsc-tracking-db \
  --backup-name pre-migration-$(date +%Y%m%d)
```

### Step 2: Generate Migration SQL Script

```bash
# Generate SQL script for review
cd backend/GscTracking.Api
dotnet ef migrations script --idempotent --output migration.sql

# Review the SQL script
cat migration.sql
```

**Why generate scripts?**
- Review exact SQL before applying
- Share with DBAs for approval
- Keep as documentation
- Can be applied manually if needed

### Step 3: Apply Migrations to Production

**Option 1: From Local Machine (Small Teams)**
```bash
# Set production connection string
export DATABASE_URL="postgresql://user:password@host/dbname?sslmode=require"

# Apply migrations
cd backend/GscTracking.Api
dotnet ef database update

# Verify
dotnet ef migrations list
```

**Option 2: From Production App (Recommended)**
```bash
# SSH into production app
flyctl ssh console --app gsc-tracking-api

# Navigate to app directory
cd /app

# Apply migrations
dotnet ef database update

# Verify
dotnet ef migrations list

# Exit
exit
```

**Option 3: Apply SQL Script Manually**
```bash
# Apply the generated SQL script
psql $PRODUCTION_DATABASE_URL < migration.sql

# Or interactively
psql $PRODUCTION_DATABASE_URL
\i migration.sql
\q
```

### Step 4: Verify Production Deployment

```bash
# Check app health
curl https://gsc-tracking-api.fly.dev/api/hello

# Check database connection
flyctl ssh console --app gsc-tracking-api
cd /app
dotnet ef migrations list
exit

# View application logs
flyctl logs --app gsc-tracking-api

# Monitor for errors
flyctl status --app gsc-tracking-api
```

### Step 5: Rollback Plan (If Needed)

If migration causes issues:

**Option 1: Rollback to Previous Migration**
```bash
# SSH into app
flyctl ssh console --app gsc-tracking-api
cd /app

# Rollback to previous migration
dotnet ef database update <PreviousMigrationName>

# Exit
exit
```

**Option 2: Restore from Backup**

**Neon:**
```bash
# Restore from branch created in Step 1
neonctl branches create --name production-restored \
  --parent pre-migration-backup

# Update connection string to use restored database
flyctl secrets set DATABASE_URL="<restored-database-url>" --app gsc-tracking-api
```

**Azure:**
```bash
# Restore from backup
az postgres flexible-server restore \
  --resource-group gsc-tracking \
  --name gsc-tracking-db-restored \
  --source-server gsc-tracking-db \
  --restore-time "<backup-timestamp>"

# Update connection string
flyctl secrets set DATABASE_URL="<restored-database-url>" --app gsc-tracking-api
```

---

## Migration Best Practices

### Naming Conventions

Use descriptive migration names:

✅ **Good Names:**
- `AddJobsTable`
- `AddCustomerStatusField`
- `CreateJobEquipmentRelationship`
- `AddIndexToCustomerEmail`
- `RemoveDeprecatedFields`

❌ **Bad Names:**
- `Update1`
- `FixBug`
- `Changes`
- `Migration123`

### Migration Design

1. **Make Incremental Changes**
   - One logical change per migration
   - Small, focused migrations are easier to understand and rollback

2. **Never Modify Applied Migrations**
   - Create new migrations for changes
   - Modifying applied migrations breaks migration history

3. **Include Data Migrations**
   - Use `migrationBuilder.Sql()` for data transformations
   - Example: Populating default values for new required columns

4. **Test Both Up and Down**
   - Test `dotnet ef database update <Migration>` (Up)
   - Test `dotnet ef database update <PreviousMigration>` (Down)
   - Ensure rollback works correctly

5. **Use Idempotent Scripts**
   - Use `--idempotent` flag when generating scripts
   - Safe to run multiple times
   - Won't fail if already applied

### Testing Migrations

```bash
# 1. Create test database
export DATABASE_URL="postgresql://user:pass@host/test_db?sslmode=require"

# 2. Apply migrations
dotnet ef database update

# 3. Verify schema
psql $DATABASE_URL -c "\dt"

# 4. Test rollback
dotnet ef database update <PreviousMigration>

# 5. Verify rollback worked
psql $DATABASE_URL -c "\dt"

# 6. Re-apply migrations
dotnet ef database update
```

---

## Common Migration Scenarios

### Adding a New Table

```bash
# 1. Create entity class in Models/
# 2. Add DbSet to ApplicationDbContext
# 3. Create migration
dotnet ef migrations add AddJobsTable

# 4. Review and apply
dotnet ef database update
```

### Adding a Column to Existing Table

```bash
# 1. Add property to entity class
# 2. Create migration
dotnet ef migrations add AddCustomerStatusField

# 3. Review migration
# 4. Apply migration
dotnet ef database update
```

**Example migration with default value:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "Status",
        table: "Customer",
        type: "TEXT",
        maxLength: 50,
        nullable: false,
        defaultValue: "Active");
}
```

### Renaming a Column

```bash
# Create migration
dotnet ef migrations add RenameCustomerNameField

# Manually edit migration to use RenameColumn
```

**Example:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Name",
        table: "Customer",
        newName: "FullName");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "FullName",
        table: "Customer",
        newName: "Name");
}
```

### Adding an Index

```bash
# Create migration
dotnet ef migrations add AddIndexToCustomerEmail
```

**Example:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateIndex(
        name: "IX_Customer_Email",
        table: "Customer",
        column: "Email",
        unique: true);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropIndex(
        name: "IX_Customer_Email",
        table: "Customer");
}
```

### Adding a Foreign Key Relationship

```bash
# 1. Update entity classes with navigation properties
# 2. Create migration
dotnet ef migrations add AddJobCustomerRelationship

# 3. Review and apply
dotnet ef database update
```

**Example:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateIndex(
        name: "IX_Job_CustomerId",
        table: "Job",
        column: "CustomerId");

    migrationBuilder.AddForeignKey(
        name: "FK_Job_Customer_CustomerId",
        table: "Job",
        column: "CustomerId",
        principalTable: "Customer",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
}
```

### Data Migration (Populating Data)

```bash
# Create migration
dotnet ef migrations add SeedDefaultRoles

# Manually add SQL for data seeding
```

**Example:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        INSERT INTO Roles (Name, Description) 
        VALUES 
            ('Admin', 'Administrator with full access'),
            ('User', 'Regular user with limited access');
    ");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        DELETE FROM Roles WHERE Name IN ('Admin', 'User');
    ");
}
```

### Removing a Column

```bash
# 1. Remove property from entity
# 2. Create migration
dotnet ef migrations add RemoveCustomerNotes

# 3. Review and apply
dotnet ef database update
```

**⚠️ Warning:** Removing columns is irreversible (data is lost).

### Splitting a Database Schema

For large migrations (e.g., splitting Customer address into separate table):

```bash
# Create migration with multiple steps
dotnet ef migrations add SplitCustomerAddress
```

**Example:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Create new Address table
    migrationBuilder.CreateTable(
        name: "Address",
        columns: table => new
        {
            Id = table.Column<int>(nullable: false)
                .Annotation("Sqlite:Autoincrement", true),
            CustomerId = table.Column<int>(nullable: false),
            Street = table.Column<string>(maxLength: 200, nullable: false),
            City = table.Column<string>(maxLength: 100, nullable: false)
        });

    // 2. Copy data from Customer.Address to Address table
    migrationBuilder.Sql(@"
        INSERT INTO Address (CustomerId, Street, City)
        SELECT Id, Address, City FROM Customer;
    ");

    // 3. Remove old Address column from Customer
    migrationBuilder.DropColumn(name: "Address", table: "Customer");
}
```

---

## Troubleshooting

### Migration Fails to Apply

**Symptom:** `dotnet ef database update` fails with error.

**Common Causes:**
- Database is locked (another process using it)
- Syntax error in migration
- Constraint violation (e.g., adding non-nullable column to table with data)
- Network issue (for remote databases)

**Solutions:**
1. **Check error message carefully**
   ```bash
   dotnet ef database update --verbose
   ```

2. **Resolve constraint violations**
   ```csharp
   // Instead of:
   migrationBuilder.AddColumn<string>("Status", "Customer", nullable: false);
   
   // Use:
   migrationBuilder.AddColumn<string>("Status", "Customer", nullable: false, defaultValue: "Active");
   ```

3. **Rollback and retry**
   ```bash
   # Rollback to previous working migration
   dotnet ef database update <PreviousMigration>
   
   # Fix the migration
   dotnet ef migrations remove
   
   # Recreate migration
   dotnet ef migrations add <MigrationName>
   ```

### Cannot Remove Migration

**Symptom:** `dotnet ef migrations remove` fails.

**Cause:** Migration has already been applied to database.

**Solution:**
```bash
# Rollback database first
dotnet ef database update <PreviousMigration>

# Then remove migration
dotnet ef migrations remove
```

### Migration Applied But Changes Not Visible

**Symptom:** Migration appears applied but database schema unchanged.

**Solutions:**
1. **Verify migration was actually applied**
   ```sql
   SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC;
   ```

2. **Check correct database is connected**
   ```bash
   echo $DATABASE_URL
   # Or check appsettings.json
   ```

3. **Restart application**
   ```bash
   # Sometimes EF Core caches schema
   flyctl apps restart gsc-tracking-api-staging
   ```

### Connection String Issues

**Symptom:** Cannot connect to database.

**Solutions:**
1. **Verify connection string format**
   ```bash
   # PostgreSQL
   echo $DATABASE_URL
   # Should start with: postgresql://
   
   # SQLite
   # Should be: Data Source=gsctracking.db
   ```

2. **Test connection**
   ```bash
   # PostgreSQL
   psql $DATABASE_URL -c "SELECT version();"
   
   # SQLite
   sqlite3 gsctracking.db "SELECT sqlite_version();"
   ```

3. **Check Fly.io secrets**
   ```bash
   flyctl secrets list --app gsc-tracking-api-staging
   ```

### Migration Order Issues

**Symptom:** Migrations applied in wrong order or duplicated.

**Solution:**
```bash
# View migration history
dotnet ef migrations list

# Compare with database
psql $DATABASE_URL -c "SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;"

# If mismatch, may need to manually sync
# Add missing migration to history:
psql $DATABASE_URL -c "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20251211000000_MigrationName', '10.0.0');"
```

---

## Additional Resources

### Official Documentation

- **EF Core Migrations**: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
- **EF Core CLI Reference**: https://learn.microsoft.com/en-us/ef/core/cli/dotnet
- **PostgreSQL with EF Core**: https://www.npgsql.org/efcore/

### Related Documentation

- [Database Setup Guide](./DATABASE-SETUP.md) - Main database documentation
- [Deployment Guide](./FLYIO-DEPLOYMENT.md) - Fly.io deployment
- [Business Analysis](../business-management-app-analysis.md) - Data model requirements

### Getting Help

- **GitHub Discussions**: https://github.com/jgsteeler/gsc-tracking/discussions
- **EF Core Issues**: https://github.com/dotnet/efcore/issues
- **Stack Overflow**: Tag with `entity-framework-core` and `npgsql`

---

## Next Steps

1. ✅ Create your first migration
2. ✅ Test migration locally
3. ✅ Apply to staging database
4. ✅ Verify in staging environment
5. ⏳ Document any custom migration requirements
6. ⏳ Set up CI/CD for automated migrations (optional)
7. ⏳ Plan production migration schedule

---

**Last Updated:** 2025-12-11  
**Maintained By:** GSC Development Team  
**Status:** ✅ Active - Ready for Use
