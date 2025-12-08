# Docker Setup and Usage Guide

This document provides comprehensive instructions for using Docker and Docker Compose with the GSC Tracking application.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Container Architecture](#container-architecture)
- [Environment Variables](#environment-variables)
- [Development Workflow](#development-workflow)
- [Production Deployment](#production-deployment)
- [Port Mappings](#port-mappings)
- [Service Dependencies](#service-dependencies)
- [Building Containers](#building-containers)
- [Common Commands](#common-commands)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before running the application with Docker, ensure you have the following installed:

- [Docker](https://docs.docker.com/get-docker/) (version 20.10 or higher)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 2.0 or higher)

Verify installation:
```bash
docker --version
docker-compose --version
```

## Quick Start

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

Edit `.env` with your preferred settings. The defaults are suitable for local development.

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

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:8080
- **API Health Check**: http://localhost:8080/api/hello
- **Database**: localhost:5432

### 5. Stop All Services

```bash
docker-compose down
```

To also remove volumes (database data):

```bash
docker-compose down -v
```

## Container Architecture

The application consists of three main services:

### 1. Database (PostgreSQL)
- **Image**: `postgres:16-alpine`
- **Purpose**: Persistent data storage
- **Volume**: Named volume `postgres_data` for data persistence
- **Health Check**: Ensures database is ready before backend starts

### 2. Backend (ASP.NET Core API)
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:10.0` (production)
- **Base Image**: `mcr.microsoft.com/dotnet/sdk:10.0` (development)
- **Purpose**: REST API server
- **Features**:
  - Multi-stage build for optimized production images
  - Hot reload support in development mode
  - Runs as non-root user for security
  - Health check endpoint

### 3. Frontend (React + Vite)
- **Base Image**: `nginx:alpine` (production)
- **Base Image**: `node:20-alpine` (development)
- **Purpose**: Web application UI
- **Features**:
  - Multi-stage build for optimized production images
  - Hot reload support in development mode
  - Nginx reverse proxy in production
  - Runs as non-root user for security

## Environment Variables

All environment variables are configured in the `.env` file. Key variables include:

### Database Configuration

```env
POSTGRES_DB=gsc_tracking          # Database name
POSTGRES_USER=gscadmin             # Database user
POSTGRES_PASSWORD=gscpassword123   # Database password (CHANGE IN PRODUCTION!)
POSTGRES_PORT=5432                 # Database port
```

### Backend Configuration

```env
BACKEND_PORT=8080                  # Backend API port
ASPNETCORE_ENVIRONMENT=Development # Environment (Development/Production)
```

### Frontend Configuration

```env
FRONTEND_PORT=5173                 # Frontend port
VITE_API_URL=http://localhost:8080 # Backend API URL for frontend
```

### CORS Configuration

```env
CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:8081
```

### Future Configuration (Placeholders)

```env
# AUTH0_DOMAIN=your-domain.auth0.com
# AUTH0_CLIENT_ID=your-client-id
# AZURE_STORAGE_CONNECTION_STRING=your-connection-string
```

## Development Workflow

### Hot Reload Development

The default `docker-compose up` command uses `docker-compose.override.yml` automatically, which provides:

- **Backend**: `dotnet watch` for automatic recompilation on code changes
- **Frontend**: Vite dev server with HMR (Hot Module Replacement)
- **Volume Mounts**: Source code is mounted into containers

### Making Code Changes

1. Edit files in `./backend` or `./frontend`
2. Changes are automatically detected and applied
3. Refresh browser (frontend) or wait for API restart (backend)

### Viewing Logs

View logs for all services:
```bash
docker-compose logs -f
```

View logs for a specific service:
```bash
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f database
```

### Accessing Container Shell

```bash
# Backend container
docker-compose exec backend bash

# Frontend container
docker-compose exec frontend sh

# Database container
docker-compose exec database psql -U gscadmin -d gsc_tracking
```

### Running Backend Commands

```bash
# Run Entity Framework migrations
docker-compose exec backend dotnet ef migrations add InitialCreate --project GscTracking.Api

# Update database
docker-compose exec backend dotnet ef database update --project GscTracking.Api

# Run tests (if available)
docker-compose exec backend dotnet test
```

### Running Frontend Commands

```bash
# Install new npm package
docker-compose exec frontend npm install <package-name>

# Run linter
docker-compose exec frontend npm run lint

# Run build
docker-compose exec frontend npm run build
```

## Production Deployment

### Building Production Images

Build production-optimized images:

```bash
docker-compose -f docker-compose.yml build
```

### Using Production Configuration

Create a production environment file:

```bash
cp .env.example .env.production
```

Update `.env.production` with production values (strong passwords, production URLs, etc.)

Start with production configuration:

```bash
docker-compose -f docker-compose.yml --env-file .env.production up -d
```

### Best Practices for Production

1. **Security**:
   - Use strong passwords for database
   - Enable HTTPS with proper SSL certificates
   - Use secrets management (Azure Key Vault, AWS Secrets Manager)
   - Regularly update base images for security patches

2. **Performance**:
   - Use health checks for container orchestration
   - Configure resource limits in docker-compose
   - Use separate networks for service isolation
   - Enable logging and monitoring

3. **Reliability**:
   - Use `restart: unless-stopped` policy
   - Configure backup strategy for database volume
   - Use container orchestration (Kubernetes, Azure Container Apps)

## Port Mappings

Default port mappings (configurable via `.env`):

| Service  | Container Port | Host Port | Description                    |
|----------|----------------|-----------|--------------------------------|
| Frontend | 8080 (prod)    | 5173      | React application (dev mode)   |
| Frontend | 8080 (prod)    | 8081      | Nginx web server (prod mode)   |
| Backend  | 8080           | 8080      | ASP.NET Core Web API           |
| Database | 5432           | 5432      | PostgreSQL database server     |

### Changing Ports

Edit `.env` file:

```env
FRONTEND_PORT=3000  # Change frontend to port 3000
BACKEND_PORT=5000   # Change backend to port 5000
POSTGRES_PORT=5433  # Change database to port 5433
```

## Service Dependencies

The services have the following dependency chain:

```
Frontend → Backend → Database
```

- **Frontend** depends on **Backend** being healthy
- **Backend** depends on **Database** being healthy
- **Database** starts first and must pass health check

Health checks ensure services start in correct order and retry if dependencies are not ready.

## Building Containers

### Build All Services

```bash
docker-compose build
```

### Build Specific Service

```bash
docker-compose build backend
docker-compose build frontend
```

### Rebuild Without Cache

```bash
docker-compose build --no-cache
```

### Multi-Stage Build Targets

Production Dockerfiles use multi-stage builds:

**Backend stages**:
1. `build` - Compile .NET application
2. `publish` - Publish optimized output
3. `final` - Runtime image with ASP.NET Core runtime only

**Frontend stages**:
1. `build` - Build React app with Node.js
2. `final` - Serve with Nginx (production-ready)

## Common Commands

### Start Services

```bash
# Start all services with logs
docker-compose up

# Start in background (detached)
docker-compose up -d

# Start specific service
docker-compose up backend
```

### Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Stop and remove images
docker-compose down --rmi all
```

### Restart Services

```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart backend
```

### View Status

```bash
# List running containers
docker-compose ps

# View container resource usage
docker-compose stats
```

### Clean Up

```bash
# Remove stopped containers
docker-compose rm

# Remove unused images
docker image prune

# Remove all unused resources
docker system prune -a
```

## Troubleshooting

### Common Issues

#### 1. Port Already in Use

**Problem**: Error message like "port is already allocated"

**Solution**:
```bash
# Check what's using the port
lsof -i :5173  # On macOS/Linux
netstat -ano | findstr :5173  # On Windows

# Change port in .env file
FRONTEND_PORT=3000
```

#### 2. Database Connection Failed

**Problem**: Backend cannot connect to database

**Solutions**:
1. Ensure database is healthy:
   ```bash
   docker-compose ps
   ```

2. Check database logs:
   ```bash
   docker-compose logs database
   ```

3. Verify connection string in `.env` matches database credentials

4. Wait longer for database initialization (can take 10-30 seconds on first run)

#### 3. Hot Reload Not Working

**Problem**: Changes not reflecting in running application

**Solutions**:

For **Backend**:
```bash
# Restart backend service
docker-compose restart backend
```

For **Frontend**:
```bash
# Restart frontend service
docker-compose restart frontend
```

Check volume mounts are correct in `docker-compose.override.yml`

#### 4. Build Failures

**Problem**: Container build fails

**Solutions**:

1. Clear build cache:
   ```bash
   docker-compose build --no-cache
   ```

2. Remove old images:
   ```bash
   docker-compose down --rmi all
   docker system prune -a
   ```

3. Check Dockerfile syntax and build context

#### 5. Permission Denied Errors

**Problem**: Permission errors when accessing files in container

**Solutions**:

1. Ensure correct user ownership in Dockerfile
2. Check volume mount permissions
3. On Linux, you may need to adjust user IDs:
   ```bash
   # Add your user to docker group
   sudo usermod -aG docker $USER
   ```

#### 6. Out of Disk Space

**Problem**: Docker runs out of disk space

**Solution**:
```bash
# Clean up unused resources
docker system prune -a --volumes

# Check disk usage
docker system df
```

#### 7. Network Issues

**Problem**: Services cannot communicate

**Solutions**:

1. Check network configuration:
   ```bash
   docker network ls
   docker network inspect gsc-tracking_gsc-network
   ```

2. Ensure services are on same network (defined in docker-compose.yml)

3. Use service names (not localhost) for inter-service communication

#### 8. Container Keeps Restarting

**Problem**: Container enters restart loop

**Solutions**:

1. Check container logs:
   ```bash
   docker-compose logs -f <service-name>
   ```

2. Check health check configuration
3. Verify environment variables are correct
4. Ensure dependencies are met

### Getting Help

If you encounter issues not covered here:

1. Check container logs: `docker-compose logs`
2. Inspect container: `docker-compose exec <service> sh`
3. Review Docker Compose configuration
4. Check GitHub issues: https://github.com/jgsteeler/gsc-tracking/issues
5. Consult Docker documentation: https://docs.docker.com

## Performance Optimization

### Image Size Optimization

1. Use Alpine-based images where possible
2. Multi-stage builds separate build and runtime dependencies
3. Use `.dockerignore` to exclude unnecessary files
4. Combine RUN commands to reduce layers

### Build Speed Optimization

1. Order Dockerfile commands from least to most frequently changed
2. Use BuildKit for parallel builds:
   ```bash
   DOCKER_BUILDKIT=1 docker-compose build
   ```
3. Leverage build cache
4. Use specific base image tags (not `latest`)

### Runtime Optimization

1. Set resource limits in docker-compose.yml:
   ```yaml
   services:
     backend:
       deploy:
         resources:
           limits:
             cpus: '0.5'
             memory: 512M
   ```

2. Use health checks to prevent traffic to unhealthy containers
3. Enable logging drivers for centralized logging

## Security Best Practices

1. **Run as non-root user**: All containers use non-root users
2. **Keep secrets out of images**: Use environment variables or secret management
3. **Scan images for vulnerabilities**: Use `docker scan <image>`
4. **Use official base images**: Always use official images from trusted sources
5. **Update regularly**: Keep base images and dependencies up to date
6. **Minimal images**: Use minimal base images (Alpine) when possible
7. **Read-only file systems**: Where applicable, use read-only root filesystems

## CI/CD Integration

Docker containers are designed for CI/CD pipelines:

### Building in CI

```bash
# Build all services
docker-compose build

# Tag images for registry
docker tag gsc-tracking-backend:latest <registry>/gsc-tracking-backend:$VERSION
docker tag gsc-tracking-frontend:latest <registry>/gsc-tracking-frontend:$VERSION

# Push to registry
docker push <registry>/gsc-tracking-backend:$VERSION
docker push <registry>/gsc-tracking-frontend:$VERSION
```

### Testing in CI

```bash
# Start services in background
docker-compose up -d

# Wait for services to be healthy
docker-compose ps

# Run tests
docker-compose exec -T backend dotnet test
docker-compose exec -T frontend npm run test

# Clean up
docker-compose down -v
```

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Node.js Docker Images](https://hub.docker.com/_/node)
- [PostgreSQL Docker Images](https://hub.docker.com/_/postgres)
- [Nginx Docker Images](https://hub.docker.com/_/nginx)

---

**Last Updated**: 2025-12-08  
**Version**: 1.0
