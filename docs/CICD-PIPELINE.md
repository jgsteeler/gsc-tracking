# CI/CD Pipeline Documentation

This document provides comprehensive documentation for the GSC Tracking CI/CD pipeline, including workflows, branching strategy, deployment processes, and configuration.

## Table of Contents

- [Overview](#overview)
- [Branching Strategy (GitHub Flow)](#branching-strategy-github-flow)
- [GitHub Actions Workflows](#github-actions-workflows)
- [Environment Variables and Secrets](#environment-variables-and-secrets)
- [Deployment Process](#deployment-process)
- [Testing and Quality Checks](#testing-and-quality-checks)
- [Troubleshooting](#troubleshooting)

## Overview

The GSC Tracking project uses GitHub Actions for continuous integration and continuous deployment (CI/CD). The pipeline automates:

- **Build Validation**: Ensuring code compiles successfully
- **Code Quality**: Linting, formatting, and validation checks
- **Testing**: Running unit and integration tests (when available)
- **Docker Image Building**: Creating containerized applications
- **Deployment**: Automatic deployment to staging and production environments
- **Release Management**: Automated versioning and changelog generation

### Current Deployment Platform

**Fly.io** is the current deployment platform for both staging and production environments. The original plan included Azure App Service, but after evaluation (see [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md)), Fly.io was chosen for:
- Cost-effectiveness (generous free tier)
- Simplicity of deployment
- Docker-first approach
- Global edge network
- Built-in PostgreSQL support

**Note:** Azure App Service deployment workflows can be added if needed in the future. The Docker images are already built and pushed to GitHub Container Registry, making it straightforward to deploy to any container platform.

### CI/CD Architecture

```
┌─────────────────┐
│  Feature Branch │
│   Development   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Pull Request   │◄─── PR Title Validation (Conventional Commits)
│    to Main      │◄─── Docker Build (test)
└────────┬────────┘◄─── Code Quality Checks (when configured)
         │
         │ (Deploy to Staging on PR)
         ▼
┌─────────────────┐
│ Staging Env     │
│ (Fly.io)        │◄─── Automatic deployment
│ PR Preview      │◄─── Shared staging environment
└─────────────────┘
         │
         │ (After Review & Approval)
         ▼
┌─────────────────┐
│  Merge to Main  │
└────────┬────────┘
         │
         ├─► Docker Build & Push (main branch)
         ├─► Deploy to Production (Fly.io)
         └─► Release Please (create release PR)
         │
         ▼
┌─────────────────┐
│  Production     │
│   (Fly.io)      │
└─────────────────┘
```

## Branching Strategy (GitHub Flow)

GSC Tracking follows **GitHub Flow**, a simplified workflow that emphasizes:

- **Single main branch**: `main` is always production-ready
- **Short-lived feature branches**: Work happens in feature branches
- **Pull requests**: All changes go through PRs for review
- **Continuous deployment**: Merging to `main` deploys to production

### Why GitHub Flow Instead of GitLab Flow?

The original issue mentioned GitLab Flow, but the project uses **GitHub Flow** because:

1. **Simplicity**: Single main branch reduces complexity
2. **Continuous Deployment**: Direct deployment from main to production
3. **Staging via PRs**: PR-based staging deployments provide testing without extra branches
4. **Small Team**: Better suited for small teams and rapid iteration
5. **Native GitHub Integration**: Works seamlessly with GitHub Actions

**GitLab Flow** would add environment branches (e.g., `staging`, `production`) but isn't necessary given:
- Fly.io handles staging/production via configuration files (`fly.staging.toml` vs `fly.toml`)
- PR-based staging deployments achieve the same testing goals
- Release Please manages versioning without needing release branches

**Comparison:**

| Feature | GitHub Flow | GitLab Flow |
|---------|-------------|-------------|
| **Main Branch** | ✅ Single `main` | ✅ Single `main` |
| **Environment Branches** | ❌ No | ✅ `staging`, `production` |
| **Deployment** | From `main` | From env branches |
| **Complexity** | Low | Medium |
| **Best For** | Small teams, rapid deployment | Larger teams, controlled releases |

If more granular control is needed in the future, the project can migrate to GitLab Flow by adding environment branches.

### Workflow Steps

1. **Create a feature branch** from `main`:
   ```bash
   git checkout -b feat/customer-search
   # or
   git checkout -b fix/api-validation
   ```

2. **Make changes** following [Conventional Commits](../COMMIT_GUIDELINES.md):
   ```bash
   git commit -m "feat(customer): add search functionality"
   ```

3. **Push and create a Pull Request**:
   ```bash
   git push origin feat/customer-search
   ```
   - PR title MUST follow Conventional Commits format
   - Automatic deployment to staging environment
   - Review and testing on staging URL

4. **Code Review**: Get PR reviewed and approved

5. **Merge to main**: 
   - Triggers production deployment
   - Release Please creates/updates release PR
   - Docker images are built and pushed

6. **Release** (when ready):
   - Merge Release Please PR
   - Creates GitHub release with changelog
   - Tags version (e.g., `frontend-v1.0.0`, `backend-v0.2.0`)

### Branch Naming Conventions

Follow these patterns for branch names:

- **Features**: `feat/short-description` or `feature/short-description`
- **Bug fixes**: `fix/short-description` or `bugfix/short-description`
- **Documentation**: `docs/short-description`
- **Infrastructure**: `infra/short-description` or `chore/short-description`

**Examples:**
```bash
feat/customer-management
fix/null-reference-error
docs/api-documentation
chore/update-dependencies
```

## GitHub Actions Workflows

The project includes four main workflows located in `.github/workflows/`:

### 1. PR Validation (`validate-pr.yml`)

**Triggers:**
- Pull request opened, edited, synchronized, or reopened

**Purpose:**
- Validates PR title follows Conventional Commits format

**Jobs:**
- `validate-pr-title`: Ensures PR title matches format `<type>(<scope>): <description>`

**Allowed types:**
- `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`

**Configuration:**
```yaml
requireScope: false  # Scope is optional
subjectPattern: ^[a-z].+$  # Subject must start with lowercase
```

### 2. Docker Build (`docker-build.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main`

**Purpose:**
- Build and push Docker images for backend and frontend
- Publish images to GitHub Container Registry (ghcr.io)

**Jobs:**

#### `changes`
- Detects which paths have changed (backend or frontend)
- Uses path filters to avoid unnecessary builds

#### `build-backend`
- Runs only if backend files changed
- Builds Docker image from `backend/Dockerfile`
- Tags image with branch name, PR number, or SHA
- Pushes to `ghcr.io/jgsteeler/gsc-tracking-backend`
- Uses layer caching for faster builds

#### `build-frontend`
- Runs only if frontend files changed
- Builds Docker image from `frontend/Dockerfile`
- Tags image with branch name, PR number, or SHA
- Pushes to `ghcr.io/jgsteeler/gsc-tracking-frontend`
- Uses layer caching for faster builds

**Image Tagging Strategy:**
```
type=ref,event=branch     # Branch name (e.g., main, develop)
type=ref,event=pr         # PR number (e.g., pr-42)
type=semver,pattern={{version}}
type=semver,pattern={{major}}.{{minor}}
type=sha                  # Commit SHA
```

### 3. Fly.io Deployment (`deploy-flyio.yml`)

**Triggers:**
- Push to `main` (backend changes)
- Pull requests to `main` (backend changes)
- Manual workflow dispatch

**Purpose:**
- Deploy backend API to Fly.io staging and production environments

**Jobs:**

#### `deploy-staging`
- Runs on pull requests
- Deploys to staging: `gsc-tracking-api-staging.fly.dev`
- Comments on PR with deployment URL
- Uses `STAGING_FLY_API_TOKEN` secret

#### `deploy-production`
- Runs on push to `main`
- Deploys to production: `gsc-tracking-api.fly.dev`
- Uses `FLY_API_TOKEN` secret
- Creates deployment summary

**Environment URLs:**
- **Staging**: https://gsc-tracking-api-staging.fly.dev
- **Production**: https://gsc-tracking-api.fly.dev

**Health Check Endpoints:**
- **Staging**: https://gsc-tracking-api-staging.fly.dev/api/hello
- **Production**: https://gsc-tracking-api.fly.dev/api/hello

### 4. Release Please (`release-please.yml`)

**Triggers:**
- Push to `main` branch

**Purpose:**
- Automates semantic versioning
- Generates changelogs
- Creates GitHub releases

**Configuration Files:**
- `.github/release-please-config.json`: Package configuration
- `.github/.release-please-manifest.json`: Current versions

**How It Works:**
1. Analyzes commits since last release
2. Determines version bump based on commit types:
   - `feat:` → Minor version (0.X.0)
   - `fix:` → Patch version (0.0.X)
   - `BREAKING CHANGE:` or `!` → Major version (X.0.0)
3. Creates/updates a Release PR with:
   - Updated version numbers
   - Generated CHANGELOG.md entries
4. When Release PR is merged:
   - Creates GitHub release
   - Tags the release
   - Publishes changelog

**Separate Versioning:**
- Frontend and backend are versioned independently
- Each has its own CHANGELOG.md

See [RELEASE.md](../RELEASE.md) for detailed release workflow.

## Environment Variables and Secrets

### GitHub Secrets

Configure these secrets in GitHub repository settings (`Settings > Secrets and variables > Actions`):

#### Required Secrets

| Secret Name | Description | Used By |
|-------------|-------------|---------|
| `FLY_API_TOKEN` | Fly.io production API token | `deploy-flyio.yml` (production) |
| `STAGING_FLY_API_TOKEN` | Fly.io staging API token | `deploy-flyio.yml` (staging) |
| `GITHUB_TOKEN` | Automatic GitHub token | All workflows (auto-provided) |

#### How to Generate Fly.io Tokens

```bash
# Install Fly.io CLI
brew install flyctl  # macOS
# or
curl -L https://fly.io/install.sh | sh  # Linux/WSL

# Authenticate
flyctl auth login

# Create long-lived deploy token
flyctl tokens create deploy --expiry 8760h

# Copy the token and add to GitHub secrets
```

### Environment Variables in Workflows

Workflows use these environment variables:

```yaml
# docker-build.yml
env:
  REGISTRY: ghcr.io
  IMAGE_PREFIX: ${{ github.repository }}

# deploy-flyio.yml
env:
  FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}
```

### Application Environment Variables

For local development and Docker, configure in `.env` file:

```bash
# Copy example file
cp .env.example .env

# Edit with your values
```

See [.env.example](../.env.example) for all available variables.

For production deployment, environment variables are configured in:
- **Fly.io**: `backend/fly.toml` and `backend/fly.staging.toml`
- **Azure**: Azure Portal > App Service > Configuration (if using Azure)

## Deployment Process

### Automatic Deployments

#### Staging Deployment (Pull Requests)

1. **Create/update a PR** to `main` branch
2. **Backend changes** trigger:
   - Docker build (test only, no push)
   - Deployment to Fly.io staging
   - Comment on PR with staging URL
3. **Test your changes** at: https://gsc-tracking-api-staging.fly.dev
4. **Staging environment** is shared across all PRs

**Staging Deployment Flow:**
```
PR to main (backend changes)
    ↓
Build Docker image (test)
    ↓
Deploy to Fly.io staging
    ↓
Comment on PR with URL
    ↓
Test changes
    ↓
Merge when ready
```

#### Production Deployment (Main Branch)

1. **Merge PR to main** (after approval)
2. **Automatic deployment** triggers:
   - Docker images built and pushed to ghcr.io
   - Backend deployed to Fly.io production
   - Release Please creates/updates release PR
3. **Production URL**: https://gsc-tracking-api.fly.dev
4. **Health check** automatically runs

**Production Deployment Flow:**
```
Merge to main
    ↓
Build & Push Docker Images
    ↓
Deploy to Fly.io Production
    ↓
Health Check
    ↓
Release Please PR
```

### Manual Deployments

#### Manual Fly.io Deployment

If you need to deploy manually (not recommended for production):

```bash
# Navigate to backend
cd backend

# Deploy to production
flyctl deploy --config fly.toml --remote-only

# Deploy to staging
flyctl deploy --config fly.staging.toml --remote-only
```

#### Manual Workflow Trigger

You can manually trigger the Fly.io deployment workflow:

1. Go to **Actions** tab in GitHub
2. Select **Deploy Backend to Fly.io** workflow
3. Click **Run workflow**
4. Select branch and click **Run workflow**

### Deployment Verification

After deployment, verify the application is running:

**Production:**
```bash
curl https://gsc-tracking-api.fly.dev/api/hello
```

**Staging:**
```bash
curl https://gsc-tracking-api-staging.fly.dev/api/hello
```

Expected response:
```json
{
  "message": "Hello from GSC Tracking API!",
  "version": "1.0.0",
  "timestamp": "2025-12-10T03:00:00Z"
}
```

### Rollback Procedure

If a deployment causes issues:

#### Option 1: Revert the Commit

```bash
# Find the commit to revert
git log --oneline

# Revert the problematic commit
git revert <commit-sha>

# Push to main
git push origin main
```

This creates a new commit that undoes the changes and triggers a new deployment.

#### Option 2: Deploy Previous Version (Fly.io)

```bash
# List previous releases
flyctl releases --app gsc-tracking-api

# Rollback to previous release
flyctl releases rollback --app gsc-tracking-api
```

#### Option 3: Manual Fix

1. Create a hotfix branch from main
2. Apply the fix
3. Create PR and deploy to staging
4. Fast-track merge to production

## Testing and Quality Checks

### Current Status

**Implemented:**
- ✅ PR title validation (Conventional Commits)
- ✅ Docker build testing
- ✅ Automatic deployment to staging/production

**To Be Implemented:**
- ⏳ Backend unit tests (.NET)
- ⏳ Frontend unit tests (Vitest/Jest)
- ⏳ Integration tests
- ⏳ End-to-end tests (Playwright/Cypress)
- ⏳ Code coverage reports
- ⏳ Linting checks in CI
- ⏳ Security scanning

### Adding Tests to CI Pipeline

When tests are implemented, add to workflows:

#### Backend Tests

Add to `docker-build.yml` before building:

```yaml
- name: Run Backend Tests
  working-directory: ./backend/GscTracking.Api
  run: |
    dotnet restore
    dotnet test --no-restore --verbosity normal
```

Or create a separate `test.yml` workflow:

```yaml
name: Run Tests

on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main, develop ]

jobs:
  test-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Run Tests
        working-directory: ./backend/GscTracking.Api
        run: |
          dotnet restore
          dotnet test --no-restore --verbosity normal --logger "trx;LogFileName=test-results.trx"
      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: test-results
          path: backend/**/TestResults/*.trx
```

#### Frontend Tests

```yaml
  test-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json
      - name: Install Dependencies
        working-directory: ./frontend
        run: npm ci
      - name: Run Tests
        working-directory: ./frontend
        run: npm test -- --run --coverage
      - name: Upload Coverage
        uses: codecov/codecov-action@v4
        with:
          directory: ./frontend/coverage
```

### Code Quality Checks

Add linting and formatting checks:

```yaml
  lint-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Run Linter
        working-directory: ./backend/GscTracking.Api
        run: dotnet format --verify-no-changes

  lint-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json
      - name: Install Dependencies
        working-directory: ./frontend
        run: npm ci
      - name: Run Linter
        working-directory: ./frontend
        run: npm run lint
```

## Troubleshooting

### Common Issues

#### 1. PR Title Validation Fails

**Error:** "PR title doesn't match conventional commits format"

**Solution:**
- Ensure PR title follows format: `<type>(<scope>): <description>`
- Valid types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`
- Description must start with lowercase letter
- Examples:
  - ✅ `feat(customer): add search functionality`
  - ✅ `fix: resolve null reference error`
  - ❌ `Add customer search`
  - ❌ `Fix bug`

#### 2. Docker Build Fails

**Error:** "failed to solve: process... exited with: 1"

**Possible causes:**
- Dockerfile syntax error
- Missing dependencies in Dockerfile
- Build context issues

**Solution:**
1. Test Docker build locally:
   ```bash
   cd backend  # or frontend
   docker build -t test-image .
   ```
2. Check Dockerfile for errors
3. Review build logs in GitHub Actions

#### 3. Fly.io Deployment Fails

**Error:** "Error: failed to fetch an image or build from source"

**Possible causes:**
- Invalid Fly.io API token
- App doesn't exist on Fly.io
- Configuration error in fly.toml

**Solution:**
1. Verify secret is set correctly: `Settings > Secrets > FLY_API_TOKEN`
2. Check app exists:
   ```bash
   flyctl apps list
   ```
3. Validate fly.toml configuration:
   ```bash
   flyctl config validate
   ```

#### 4. Release Please Not Creating PR

**Error:** Release PR not created after merging to main

**Possible causes:**
- No conventional commit messages since last release
- Commits in non-package directories
- Only `chore:` or `docs:` commits (no version bump)

**Solution:**
1. Check commit messages follow Conventional Commits
2. Ensure commits are in `frontend/` or `backend/` directories
3. Use `feat:` or `fix:` for version bumps
4. Review workflow logs: `Actions > Release Please`

#### 5. Workflow Permissions Error

**Error:** "Resource not accessible by integration"

**Solution:**
1. Go to `Settings > Actions > General`
2. Scroll to "Workflow permissions"
3. Select "Read and write permissions"
4. Check "Allow GitHub Actions to create and approve pull requests"
5. Click "Save"

### Getting Help

If you encounter issues not covered here:

1. **Check workflow logs**: Go to `Actions` tab and review failed run
2. **Review documentation**:
   - [GitHub Actions docs](https://docs.github.com/en/actions)
   - [Fly.io docs](https://fly.io/docs/)
   - [Release Please docs](https://github.com/googleapis/release-please)
3. **Search existing issues**: Check GitHub issues for similar problems
4. **Open an issue**: Create a new issue with workflow logs and error details

## Azure App Service Deployment (Alternative)

While the current deployment uses Fly.io, you can add Azure App Service deployment to the CI/CD pipeline. Here's how:

### Prerequisites

1. Azure subscription with App Service created
2. Azure credentials configured as GitHub secrets

### Adding Azure Deployment Workflow

Create `.github/workflows/deploy-azure.yml`:

```yaml
name: Deploy to Azure App Service

on:
  push:
    branches: [ main ]
    paths:
      - 'backend/**'
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: gsc-tracking-api    # Replace with your app name
  AZURE_WEBAPP_PACKAGE_PATH: './backend'
  DOTNET_VERSION: '10.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Build with dotnet
        working-directory: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
        run: dotnet build --configuration Release
      
      - name: Publish with dotnet
        working-directory: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
        run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp
      
      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ${{env.DOTNET_ROOT}}/myapp
```

### Alternative: Deploy Docker Container to Azure

```yaml
name: Deploy Docker to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: gsc-tracking-api
          images: 'ghcr.io/jgsteeler/gsc-tracking-backend:main'
```

### Required Secrets for Azure

Add these secrets to GitHub repository settings:

| Secret Name | How to Get It |
|-------------|---------------|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Download from Azure Portal: App Service > Deployment Center > Manage publish profile |
| `AZURE_CREDENTIALS` | Create using Azure CLI: `az ad sp create-for-rbac --name "gsc-tracking" --sdk-auth --role contributor` |

### Azure-Specific Configuration

Update `appsettings.json` for Azure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://your-vault.vault.azure.net/secrets/DbConnectionString/)"
  },
  "AllowedOrigins": [
    "https://gsc-tracking-frontend.azurewebsites.net"
  ]
}
```

### Comparison: Fly.io vs Azure App Service

| Feature | Fly.io | Azure App Service |
|---------|--------|-------------------|
| **Free Tier** | ✅ Generous | ✅ Limited (F1 plan) |
| **Deployment** | Docker-based | App code or Docker |
| **Database** | Built-in PostgreSQL | Separate service needed |
| **Global Edge** | ✅ Built-in | Requires Front Door |
| **Complexity** | Low | Medium |
| **Azure Integration** | ❌ | ✅ Native |
| **Scaling** | Automatic | Manual/Auto-scale rules |

See [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md) for detailed comparison.

## Additional Resources

### Internal Documentation
- [RELEASE.md](../RELEASE.md) - Release process and versioning
- [COMMIT_GUIDELINES.md](../COMMIT_GUIDELINES.md) - Commit message format
- [DOCKER.md](../DOCKER.md) - Docker and container documentation
- [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md) - Detailed Fly.io deployment guide
- [DEPLOYMENT-SETUP-CHECKLIST.md](./DEPLOYMENT-SETUP-CHECKLIST.md) - Initial setup steps
- [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md) - Platform comparison and evaluation

### External Resources
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Flow Guide](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Fly.io Documentation](https://fly.io/docs/)
- [Azure App Service Documentation](https://learn.microsoft.com/en-us/azure/app-service/)

---

**Last Updated:** 2025-12-10  
**Maintained By:** GSC Development Team
