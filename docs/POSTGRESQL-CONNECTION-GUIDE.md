# PostgreSQL Connection Guide

This guide provides detailed instructions for setting up and using PostgreSQL as the database for the GSC Tracking application during development.

---

## Local Development with Docker Compose

The local `docker-compose` setup includes a PostgreSQL instance for development. This is the recommended way to run PostgreSQL locally, as it ensures consistency across development environments.

### Steps to Use Local PostgreSQL

1. **Start the Docker Compose Services**:

   ```bash
   docker-compose up -d
   ```

   This will start a PostgreSQL instance along with other required services.

2. **Connection Details**:
   - **Host**: `localhost`
   - **Port**: `5432`
   - **Database Name**: `gsctracking`
   - **Username**: `postgres`
   - **Password**: `password`

3. **Test the Connection**:
   Use a database client like `psql`, DBeaver, or TablePlus to connect to the database using the above credentials.

4. **Apply Migrations**:

   ```bash
   # Navigate to the backend directory
   cd backend/GscTracking.Api

   # Apply migrations
   dotnet ef database update
   ```

---

## Using Neon for Development

[Neon](https://neon.tech) is a serverless PostgreSQL hosting provider that is ideal for development and staging environments. It offers features like instant database creation, auto-suspend, and GitHub integration.

### Steps to Use Neon

1. **Create a Neon Account**:
   - Visit [Neon](https://neon.tech) and sign up (GitHub login recommended).

2. **Create a New Project**:
   - Log in to the Neon dashboard.
   - Click "New Project" and follow the prompts to create a PostgreSQL database.

3. **Get the Connection String**:
   - After the project is created, copy the connection string from the dashboard.
   - Example: `postgresql://user:password@host:port/database?sslmode=require`

4. **Configure the Application**:
   - Update the `appsettings.Development.json` file or environment variables with the Neon connection string.

5. **Apply Migrations**:

   ```bash
   # Navigate to the backend directory
   cd backend/GscTracking.Api

   # Apply migrations
   dotnet ef database update
   ```

---

## Notes

- **Environment Variables**: Store sensitive connection details in environment variables to avoid hardcoding them in the codebase.
- **Testing**: Always test the database connection after setup to ensure everything is configured correctly.
- **Support**: If you encounter issues, refer to the [Database Setup Guide](./DATABASE-SETUP.md) or open an issue in the repository.

For any questions or further assistance, feel free to reach out to the development team.