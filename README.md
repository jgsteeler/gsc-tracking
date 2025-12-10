# GSC Tracking - Small Engine Repair Business Management App

Software for tracking equipment, projects, expenses, and sales for GSC Small Engine Repair Shop.

[![Docker Build and Push](https://github.com/jgsteeler/gsc-tracking/actions/workflows/docker-build.yml/badge.svg)](https://github.com/jgsteeler/gsc-tracking/actions/workflows/docker-build.yml)
[![Deploy to Fly.io](https://github.com/jgsteeler/gsc-tracking/actions/workflows/deploy-flyio.yml/badge.svg)](https://github.com/jgsteeler/gsc-tracking/actions/workflows/deploy-flyio.yml)
[![Validate PR](https://github.com/jgsteeler/gsc-tracking/actions/workflows/validate-pr.yml/badge.svg)](https://github.com/jgsteeler/gsc-tracking/actions/workflows/validate-pr.yml)
[![Release Please](https://github.com/jgsteeler/gsc-tracking/actions/workflows/release-please.yml/badge.svg)](https://github.com/jgsteeler/gsc-tracking/actions/workflows/release-please.yml)

## 📋 Project Documentation

### Core Documentation
- **[Business Analysis](./business-management-app-analysis.md)** - Comprehensive technology stack analysis and requirements
- **[GitHub Issues](./ISSUES.md)** - Complete specifications for 25 project issues
- **[Setup Instructions](./SETUP-INSTRUCTIONS.md)** - Step-by-step guide for creating labels, milestones, and issues

### CI/CD and Deployment
- **[CI/CD Pipeline](./docs/CICD-PIPELINE.md)** - Complete CI/CD documentation with workflows, branching strategy, and troubleshooting
- **[Release Process](./RELEASE.md)** - Semantic versioning and automated release workflow
- **[Fly.io Deployment](./docs/FLYIO-DEPLOYMENT.md)** - Complete guide for deploying the backend to Fly.io
- **[Deployment Setup Checklist](./docs/DEPLOYMENT-SETUP-CHECKLIST.md)** - Step-by-step checklist for first-time Fly.io setup
- **[Hosting Evaluation](./docs/HOSTING-EVALUATION.md)** - Analysis of hosting alternatives with cost estimates and recommendations

### Development
- **[Docker Guide](./DOCKER.md)** - Complete Docker and Docker Compose documentation
- **[Contributing Guidelines](./CONTRIBUTING.md)** - How to contribute to the project
- **[Commit Guidelines](./COMMIT_GUIDELINES.md)** - Conventional Commits format requirements

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

## 🚀 CI/CD and Deployment

This project uses **GitHub Actions** for continuous integration and deployment with **GitHub Flow** branching strategy. The application uses a **hybrid deployment approach** optimized for the MVP.

### CI/CD Pipeline

The automated pipeline includes:
- ✅ **PR Validation**: Conventional Commits format enforcement
- ✅ **Docker Build**: Automatic image building and publishing to GitHub Container Registry
- ✅ **Automated Deployment**: Backend to Fly.io, Frontend to Netlify
- ✅ **Deploy Previews**: Isolated frontend previews for each PR via Netlify
- ✅ **Release Management**: Automated versioning and changelog generation with Release Please

📖 **Full Documentation**: [CI/CD Pipeline Guide](./docs/CICD-PIPELINE.md)

### Deployment Architecture (MVP)

**Backend API** (Fly.io)
- **Production:** https://gsc-tracking-api.fly.dev
- **Staging:** https://gsc-tracking-api-staging.fly.dev (shared)
- **Trigger:** PR creates staging, merge to `main` deploys production
- **Features:** Docker-based, built-in PostgreSQL, global edge network

**Frontend** (Netlify)
- **Production:** Primary Netlify site URL
- **Staging:** Unique deploy preview per PR
- **Trigger:** Automatic deployment on PR and merge to `main`
- **Features:** Global CDN, atomic deploys, instant rollback, isolated PR previews

### Post-MVP Scaling

For production scaling needs, the architecture can be upgraded:
- **Staging:** Keep on Fly.io + Netlify (cost-effective)
- **Production:** Migrate backend to Azure App Service (enterprise-grade)
- **Frontend:** Continue on Netlify (optimized for static sites)

See [Post-MVP Scaling Plan](./docs/CICD-PIPELINE.md#post-mvp-scaling-plan) for detailed migration strategy.

### GitHub Flow Workflow

1. **Create a feature branch** from `main` (e.g., `feat/customer-search`)
2. **Make changes** following [Conventional Commits](./COMMIT_GUIDELINES.md)
3. **Open a pull request** → Automatically deploys backend to Fly.io staging and frontend to Netlify preview
4. **Test your changes** using the preview URLs (posted in PR comments)
5. **Get code review** and approval
6. **Merge to main** → Automatically deploys to production (backend to Fly.io, frontend to Netlify)
7. **Release Please** creates release PR when ready

### Setting Up Deployment

**Backend (Fly.io):**

Follow the [Deployment Setup Checklist](./docs/DEPLOYMENT-SETUP-CHECKLIST.md) for step-by-step instructions:

1. Create a Fly.io account at https://fly.io/app/sign-up
2. Install Fly.io CLI: `brew install flyctl` (or see checklist for other OS)
3. Create both apps:
   - Production: `flyctl launch --no-deploy --name gsc-tracking-api --config fly.toml`
   - Staging: `flyctl launch --no-deploy --name gsc-tracking-api-staging --config fly.staging.toml`
4. Generate an API token: `flyctl tokens create deploy --expiry 8760h`
5. Add the token as `FLY_API_TOKEN` in GitHub repository secrets
6. Create a PR to test staging deployment, or push to `main` for production

**Frontend (Netlify):**

1. Create a Netlify account at https://netlify.com
2. Connect your GitHub repository via Netlify dashboard
3. Configure build settings:
   - **Base directory:** `frontend`
   - **Build command:** `npm run build`
   - **Publish directory:** `frontend/dist`
4. Enable **Deploy Previews** for pull requests
5. Set environment variable: `VITE_API_URL` to your backend API URL

See [CICD-PIPELINE.md](./docs/CICD-PIPELINE.md#netlify-configuration) for detailed Netlify setup.

See the [complete deployment guide](./docs/FLYIO-DEPLOYMENT.md) for detailed instructions and troubleshooting.

## 🤝 Contributing

This is an open-source project. See [CONTRIBUTING.md](./CONTRIBUTING.md) for complete guidelines.

**⚠️ Important:** PR titles MUST follow [Conventional Commits](./COMMIT_GUIDELINES.md) format. This is required for automated releases.

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
# Test
