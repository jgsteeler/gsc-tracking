# Database Setup Guide

This document provides an overview of the database setup for the GSC Tracking application, including PostgreSQL configuration and database migrations.

---

## PostgreSQL Setup

The GSC Tracking application uses PostgreSQL as its primary database for staging and production environments. PostgreSQL provides reliability, scalability, and a rich feature set, making it an ideal choice for the application.

To configure PostgreSQL, follow the detailed instructions in the [PostgreSQL Connection Guide](./POSTGRESQL-CONNECTION-GUIDE.md).

### Key Steps

1. Install PostgreSQL on your local machine or use a managed service like Neon or Azure PostgreSQL.
2. Configure the connection string in the `appsettings.json` or environment variables.
3. Run database migrations to set up the schema:

   ```bash
   # Navigate to the backend directory
   cd backend/GscTracking.Api

   # Apply migrations
   dotnet ef database update
   ```

4. Test the database connection by running the application locally.

For more details, see the [PostgreSQL Connection Guide](./POSTGRESQL-CONNECTION-GUIDE.md).

---

## Database Migrations

The application uses Entity Framework Core for database migrations. Migrations allow developers to version control schema changes and apply them consistently across environments.

### Running Migrations

To create and apply migrations:

1. Create a new migration:

   ```bash
   dotnet ef migrations add <MigrationName>
   ```

2. Apply the migration to the database:

   ```bash
   dotnet ef database update
   ```

### Supported Database Providers

The current implementation officially supports PostgreSQL. Developers are welcome to submit merge requests (MRs) or open issues to request support for other database providers, such as:

- SQLite (for lightweight local development)
- SQL Server
- MySQL
