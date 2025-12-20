# Local Development Getting Started Guide

This guide provides step-by-step instructions for setting up the GSC Tracking application for local development and testing. Each section includes links to detailed documentation for further reference.

---

## 1. Clone the Repository

Start by cloning the repository to your local machine:

```bash
git clone https://github.com/jgsteeler/gsc-tracking.git
cd gsc-tracking
```

---

## 2. Build and Run the Application Locally

### Option 1: Using Docker (Recommended)

Docker simplifies the setup by running all services (frontend, backend, database) in containers:

1. Copy the example environment file:

   ```bash
   cp .env.example .env
   ```

2. Start all services:

   ```bash
   docker-compose up
   ```

3. Access the application:
   - **Frontend**: <http://localhost:5173>
   - **Backend API**: <http://localhost:8080>
   - **Database**: PostgreSQL on localhost:5432

For more details, see the [Docker Setup and Usage Guide](./DOCKER.md).

### Option 2: Manual Setup

You can run the API and frontend separately:

Start the Backend API:

```bash
cd backend/GscTracking.Api
dotnet run
```

The API will be available at `http://localhost:5091` (or `https://localhost:7075` with HTTPS profile)

Start the Frontend:

```bash
cd frontend
npm install  # First time only
npm run dev
```

The app will be available at `http://localhost:5173`'

---

## 3. Set Up the Application for Development and Testing

### Docker Configuration

Follow the [Docker Setup and Usage Guide](./DOCKER.md) for detailed instructions on configuring Docker for development.

### Authentication and Authorization

The application uses Auth0 for authentication and authorization. To configure Auth0:

1. Update the `.env` file with your Auth0 credentials.
2. For more details, see the [Authentication Guide](./AUTH.md).

### Authorization Configuration

The backend API is configured to use Auth0 for securing endpoints. Role-based access control (RBAC) is implemented to manage user permissions. The following roles are available:

- `tracking-admin`: Full access to all features.
- `tracking-read`: Read-only access to data.
- `tracking-write`: Write access to data, but no administrative privileges.

To test authorization locally:

1. Ensure your Auth0 credentials are correctly set in the `.env` file.
2. Use the provided test accounts or create new ones in your Auth0 tenant.
3. Assign the appropriate roles to your test accounts in Auth0.

> **Note:** RBAC is not supported with mock authentication. All mock users will default to the `tracking-admin` role. Proper RBAC testing requires a fully configured Auth0 setup.

### Database Setup

The application uses PostgreSQL for data storage. To set up the database:

1. Follow the [Database Setup Guide](./DATABASE-SETUP.md) to configure the database for local development.

---

## Additional Resources

- [Docker Setup and Usage Guide](./DOCKER.md)
- [Auth0 Setup Guide](./AUTH0-SETUP.md)
- [Database Setup Guide](./DATABASE-SETUP.md)
- [PostgreSQL Connection Guide](./POSTGRESQL-CONNECTION-GUIDE.md)