# GSC Tracking - Small Engine Repair Business Management App

GSC Tracking is a software application designed to help small engine repair shops manage their operations efficiently. It includes features for tracking equipment, managing projects, recording expenses, and monitoring sales.

## Technology Stack

- **Backend:** .NET 10 Web API (C#) with Entity Framework Core
- **Frontend:** React + Vite + TypeScript
- **Database:** PostgreSQL (Docker) / Azure SQL Database
- **Authentication:** Auth0
- **Containerization:** Docker & Docker Compose
- **Hosting:** Azure App Service (containerized) or alternatives
- **Storage:** Azure Blob Storage or alternatives

## Quick Start

### Option 1: Docker (Recommended)

The easiest way to run the full stack with database:

```bash
# Copy environment variables
cp .env.example .env

# Start all services (database, backend, frontend)
docker-compose up
```

- **Frontend**: <http://localhost:5173>
- **Backend API**: <http://localhost:8080>
- **Database**: PostgreSQL on localhost:5432

Detailed Docker setup instructions can be found in the [Docker Setup and Usage Guide](./docs/DOCKER.md).

### Option 2: Local Development

**Backend (.NET 10 Web API):**

```bash
cd backend/GscTracking.Api
dotnet run
```

The API will be available at `http://localhost:5091` (or `https://localhost:7075` with HTTPS profile)

**Frontend (React + Vite):**

```bash
cd frontend
npm install  # First time only
npm run dev
```

The app will be available at `http://localhost:5173`

For detailed setup instructions, see the [Getting Started Guide](./docs/README.md).

## Contributing

We welcome contributions from the community! To get started:

1. Read the [Code of Conduct](./CODE_OF_CONDUCT.md).
2. Review the [Contributing Guidelines](./CONTRIBUTING.md).
3. Fork the repository and create a branch for your changes.
4. Follow the [Conventional Commits](./docs/COMMIT_GUIDELINES.md) format for commit messages.
5. Open a pull request with a clear description of your changes.

For more details, see the [Contributing Guidelines](./CONTRIBUTING.md).

## License

This project is licensed under the AGPL-3.0 License.
