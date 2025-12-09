# GSC Tracking - Small Engine Repair Business Management App

Software for tracking equipment, projects, expenses, and sales for GSC Small Engine Repair Shop.

## 📋 Project Documentation

- **[Business Analysis](./business-management-app-analysis.md)** - Comprehensive technology stack analysis and requirements
- **[Hosting Evaluation](./docs/HOSTING-EVALUATION.md)** - Analysis of hosting alternatives (Azure, Fly.io, Railway, Netlify, Cloudflare) with cost estimates and recommendations
- **[GitHub Issues](./ISSUES.md)** - Complete specifications for 25 project issues
- **[Setup Instructions](./SETUP-INSTRUCTIONS.md)** - Step-by-step guide for creating labels, milestones, and issues
- **[Docker Guide](./DOCKER.md)** - Complete Docker and Docker Compose documentation
- **[Release Process](./RELEASE.md)** - Semantic versioning and automated release workflow

## 🎯 Quick Start

### Option 1: Docker (Recommended)

The easiest way to run the full stack with database:

```bash
# Copy environment variables
cp .env.example .env

# Start all services (database, backend, frontend)
docker-compose up
```

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:8080
- **Database**: PostgreSQL on localhost:5432

See [DOCKER.md](./DOCKER.md) for complete Docker documentation.

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

### Project Setup

1. Review the [business analysis document](./business-management-app-analysis.md) for project context
2. Follow the [setup instructions](./SETUP-INSTRUCTIONS.md) to create GitHub labels and milestones
3. Create issues from the [ISSUES.md](./ISSUES.md) specifications

## 🏗️ Technology Stack

- **Backend:** .NET 10 Web API (C#) with Entity Framework Core
- **Frontend:** React + Vite + TypeScript
- **Database:** PostgreSQL (Docker) / Azure SQL Database
- **Authentication:** Auth0
- **Containerization:** Docker & Docker Compose
- **Hosting:** Azure App Service (containerized) or alternatives
- **Storage:** Azure Blob Storage or alternatives

## 📊 Project Structure

```
gsc-tracking/
├── backend/               # .NET 10 Web API
│   └── GscTracking.Api/  # Main API project
├── frontend/             # React + Vite + TypeScript
│   └── src/             # Source files
├── .github/             # GitHub configuration and templates
└── docs/                # Documentation files
```

### Development Roadmap

- **MVP Features:** 6 core features for minimum viable product
- **Roadmap Features:** 11 enhanced features for future phases
- **Infrastructure:** 8 setup and DevOps tasks

## 🤝 Contributing

This is an open-source project. Issue templates and contribution guidelines are available in `.github/ISSUE_TEMPLATE/`.

## 📄 License

TBD - Choose appropriate license (MIT, Apache 2.0, etc.)

# GSC Tracking - Backend API

.NET 10 Web API for the GSC Small Engine Repair business management application.

## Repository Documentation

- [LICENSE](./LICENSE)
- [CONTRIBUTING.md](./CONTRIBUTING.md)
- [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)
- [MANUAL_CONFIGURATION_GUIDE.md](./MANUAL_CONFIGURATION_GUIDE.md)
- [Setup Instructions](./SETUP-INSTRUCTIONS.md)
- [Business Analysis](./business-management-app-analysis.md)
- [.github/copilot-instructions.md](./.github/copilot-instructions.md)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

...existing code...
