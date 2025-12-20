# Docker Setup and Usage Guide

This document provides comprehensive instructions for using Docker and Docker Compose with the GSC Tracking application.

---

## Updated Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/jgsteeler/gsc-tracking.git
cd gsc-tracking
```

### 2. Configure Environment Variables

Copy the example environment file and update values as needed:

```bash
cp .env.example .env
```

Edit `.env` with your preferred settings. Key updates include:

- **Database URL**:

  ```env
  DATABASE_URL=postgresql://gscadmin:gscpassword123@localhost:5432/gsc_tracking
  ```

- **Auth0 Configuration**:

  ```env
  AUTH0_DOMAIN=your-tenant.auth0.com
  AUTH0_AUDIENCE=https://your-api-audience
  ```

- **Mock Authentication**:

  ```env
  MOCK_AUTH=true
  VITE_MOCK_AUTH=true
  ```

### 3. Start All Services

Start all services (database, backend, and frontend) with hot reload:

```bash
docker-compose up
```

Or run in detached mode:

```bash
docker-compose up -d
```

### 4. Access the Application

- **Frontend**: <http://localhost:5173>
- **Backend API**: <http://localhost:8080>
- **API Health Check**: <http://localhost:8080/api/hello>
- **Database**: localhost:5432

### 5. Stop All Services

```bash
docker-compose down
```

To also remove volumes (database data):

```bash
docker-compose down -v
```

---

## Updated Container Architecture

### 1. Database (PostgreSQL)

- **Image**: `postgres:16-alpine`
- **Purpose**: Persistent data storage
- **Volume**: Named volume `postgres_data` for data persistence
- **Environment Variables**:

  ```env
  POSTGRES_DB=gsc_tracking
  POSTGRES_USER=gscadmin
  POSTGRES_PASSWORD=gscpassword123
  ```

- **Health Check**: Ensures database is ready before backend starts

### 2. Backend (ASP.NET Core API)

- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:10.0` (production)
- **Base Image**: `mcr.microsoft.com/dotnet/sdk:10.0` (development)
- **Purpose**: REST API server
- **Environment Variables**:

  ```env
  DATABASE_URL=postgresql://gscadmin:gscpassword123@localhost:5432/gsc_tracking
  AUTH0_DOMAIN=your-tenant.auth0.com
  AUTH0_AUDIENCE=https://your-api-audience
  MOCK_AUTH=true
  ```

- **Features**:
  - Multi-stage build for optimized production images
  - Hot reload support in development mode
  - Health check endpoint

### 3. Frontend (React + Vite)

- **Base Image**: `nginx:alpine` (production)
- **Base Image**: `node:20-alpine` (development)
- **Purpose**: Web application UI
- **Environment Variables**:

  ```env
  VITE_API_URL=http://localhost:8080
  VITE_MOCK_AUTH=true
  ```

- **Features**:
  - Multi-stage build for optimized production images
  - Hot reload support in development mode
  - Nginx reverse proxy in production

---

## Updated Environment Variables

### Database Configuration

```env
DATABASE_URL=postgresql://gscadmin:gscpassword123@localhost:5432/gsc_tracking
```

### Backend Configuration

```env
AUTH0_DOMAIN=your-tenant.auth0.com
AUTH0_AUDIENCE=https://your-api-audience
MOCK_AUTH=true
```

### Frontend Configuration

```env
VITE_API_URL=http://localhost:8080
VITE_MOCK_AUTH=true
```

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Errors

- **Error**: "Unable to connect to the database."
- **Solution**: Ensure the `DATABASE_URL` is correctly configured and the database container is running.

#### 2. Auth0 Misconfiguration

- **Error**: "Invalid audience" or "Unauthorized."
- **Solution**: Verify `AUTH0_DOMAIN` and `AUTH0_AUDIENCE` values in `.env` match your Auth0 tenant settings.

#### 3. Docker Volume Issues

- **Error**: "Permission denied" or "Volume not found."
- **Solution**: Ensure Docker has the necessary permissions to create and manage volumes. Use `docker volume ls` to verify.

### Debugging Tips

- Use `docker-compose logs` to view logs for all services.
- Use `docker-compose ps` to check the status of running containers.
- Use `docker exec -it <container_name> bash` to access a container shell for debugging.

---

## Testing the Setup

### Smoke Tests

Run the provided smoke test scripts to verify the setup:

```bash
./smoke-test.sh
```

### Backend Health Check

Verify the backend API is running:

```bash
curl http://localhost:8080/api/hello
```

### Frontend Accessibility

Ensure the frontend is accessible at:

```bash
http://localhost:5173
```

---

## CI/CD Integration

### Docker in CI/CD

- Ensure the `docker-compose.yml` file is used in your CI/CD pipeline for consistent builds.
- Use the `docker-compose.override.yml` file for development-specific configurations.
- Refer to the `CICD-PIPELINE.md` document for detailed CI/CD setup instructions.
