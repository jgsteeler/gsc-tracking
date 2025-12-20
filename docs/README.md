# GSC Tracking Documentation

This directory contains project documentation for the GSC Small Engine Repair Business Management Application.

## Getting Started

For a comprehensive guide to setting up the GSC Tracking application for local development, see the **[Local Development Getting Started Guide](../docs/GETTING-STARTED.md)**. This guide consolidates key documentation and provides step-by-step instructions for:

- Cloning the repository
- Building and running the app locally
- Setting up the app for development and local testing

Each section links to detailed documentation for further reference.

## Available Documents

### CI/CD and Deployment

#### [CICD-PIPELINE.md](./CICD-PIPELINE.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-10

Comprehensive CI/CD pipeline documentation covering the complete automated build, test, and deployment workflow.

**Includes:**

- CI/CD architecture and workflow diagrams
- GitHub Flow branching strategy (vs GitLab Flow comparison)
- All 4 GitHub Actions workflows explained in detail
- Environment variables and secrets configuration
- Deployment processes (staging and production)
- Rollback procedures and troubleshooting
- Test integration examples for future implementation
- Azure App Service deployment alternative

**Workflows Documented:**

- `validate-pr.yml` - PR title validation
- `docker-build.yml` - Docker image building
- `deploy-flyio.yml` - Fly.io deployment
- `release-please.yml` - Automated releases

#### [CICD-VALIDATION.md](./CICD-VALIDATION.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-10

Validation report for CI/CD pipeline implementation against original requirements.

**Includes:**

- Acceptance criteria validation
- Implementation details for each workflow
- Rationale for GitHub Flow vs GitLab Flow
- Rationale for Fly.io vs Azure App Service
- Security considerations
- Performance optimizations
- Gaps and future enhancements
- Approval recommendation

#### [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-08

Detailed guide for deploying the backend API to Fly.io.

**Includes:**

- Fly.io setup and configuration
- Deployment workflow explanation
- Manual deployment procedures
- Configuration files (fly.toml, fly.staging.toml)
- Troubleshooting guide

#### [DEPLOYMENT-SETUP-CHECKLIST.md](./DEPLOYMENT-SETUP-CHECKLIST.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-08

Step-by-step checklist for first-time Fly.io deployment setup.

**Includes:**

- Fly.io account and CLI setup
- App creation for staging and production
- GitHub secrets configuration
- Testing deployment workflow
- Verification steps

### Database Setup

#### [DATABASE-SETUP.md](./DATABASE-SETUP.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-11

Complete database setup guide for all environments (local, staging, production).

**Includes:**

- Database choice rationale (PostgreSQL vs SQLite)
- Neon PostgreSQL setup for staging (free tier)
- Production planning (Neon Pro, Azure PostgreSQL, Supabase)
- Connection configuration for all environments
- Database schema and migrations overview
- Backup and restore procedures
- Monitoring and alerting guidelines
- Security best practices
- Troubleshooting guide

### Authentication and Security

#### [AUTH0-SETUP.md](./AUTH0-SETUP.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-16

Complete Auth0 authentication setup guide for both backend and frontend.

**Includes:**

- Auth0 tenant and application configuration
- Backend JWT Bearer authentication setup
- Frontend React integration with Auth0 SDK
- Environment variable configuration (local, staging, production)
- Testing authentication flows
- Protecting API endpoints with [Authorize]
- Role-based and permission-based authorization
- Troubleshooting common issues
- Security best practices

#### [POSTGRESQL-CONNECTION-GUIDE.md](./POSTGRESQL-CONNECTION-GUIDE.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-14

Comprehensive guide for connecting Entity Framework Core to PostgreSQL databases, specifically Neon PostgreSQL.

**Includes:**

- Connection string formats (URL, standard, Docker)
- Environment variable configuration
- Common connection issues and solutions
- SSL/TLS certificate handling
- Migration workflow for PostgreSQL
- Neon-specific configuration (pooling, auto-suspend, branches)
- Testing connection procedures
- Best practices for security and performance

#### [TROUBLESHOOTING-NEON.md](./TROUBLESHOOTING-NEON.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-14

Quick reference guide for common Neon PostgreSQL connection issues.

**Includes:**

- Connection refused/timeout (auto-suspend)
- SSL/TLS certificate errors
- Authentication failures (password encoding)
- Database does not exist errors
- Migration type errors (SQLite vs PostgreSQL)
- Connection pool exhaustion
- Environment variable issues
- Quick testing commands
- Common CLI commands reference

#### [NEON-QUICKSTART.md](./NEON-QUICKSTART.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-11

5-minute quick start guide for setting up Neon PostgreSQL staging database.

**Includes:**

- Step-by-step setup (< 5 minutes)
- Fly.io integration
- Testing database connection
- Common tasks (branching, backup, restore)
- Troubleshooting

#### [DATABASE-MIGRATION-GUIDE.md](./DATABASE-MIGRATION-GUIDE.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-11

Entity Framework Core migrations reference and best practices.

**Includes:**

- EF Core migrations overview
- Creating and applying migrations
- Local, staging, and production deployment
- Migration best practices
- Common migration scenarios (add table, add column, indexes, etc.)
- Rollback procedures
- Troubleshooting migrations

### Infrastructure and Hosting

#### [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md)

**Status:** ✅ Complete  
**Last Updated:** 2025-12-08

Comprehensive evaluation of hosting alternatives for the GSC Tracking application, comparing five major platforms:

- **Azure App Service** - Enterprise PaaS with native .NET support
- **Fly.io** - Edge compute platform with global distribution
- **Railway** - Developer-focused PaaS with one-click databases
- **Netlify** - JAMstack/static site hosting (frontend only)
- **Cloudflare** - Edge platform with CDN and API gateway

**Includes:**

- Platform analysis with pros/cons and feature highlights
- Cost breakdowns for each environment (dev, staging, production, scaled)
- Environment-specific recommendations
- Migration strategies
- DNS and SSL setup guide
- Deployment configuration examples
- Testing approach

**Quick Summary:**

- **Development:** $0/month (Fly.io + Netlify free tiers)
- **Staging:** $40-50/month
- **Initial Production:** $21-65/month
- **Scaled Production:** $75-215/month

## Other Project Documentation

- [../business-management-app-analysis.md](../business-management-app-analysis.md) - Comprehensive business requirements and technical architecture
- [../SETUP-INSTRUCTIONS.md](../SETUP-INSTRUCTIONS.md) - GitHub project setup guide
- [../ISSUES.md](../ISSUES.md) - Detailed issue specifications
- [../README.md](../README.md) - Project README with overview and getting started

## Contributing

When adding new documentation:

1. Create the document in this `docs/` directory
2. Use clear, descriptive filenames (e.g., `DEPLOYMENT-GUIDE.md`, `API-DOCUMENTATION.md`)
3. Include a status, last updated date, and summary at the top
4. Update this README with a link and brief description
5. Use Markdown formatting consistently
6. Include code examples where appropriate

## Document Templates

### Standard Document Header

```markdown
# Document Title

**Document Version:** 1.0  
**Last Updated:** YYYY-MM-DD  
**Author:** GSC Development Team  
**Status:** Draft | In Review | Complete

---

## Summary

Brief description of what this document covers...

## Table of Contents

1. [Section 1](#section-1)
2. [Section 2](#section-2)
...
```

## Documentation Index

| Document | Category | Purpose |
|----------|----------|---------|
| [DATABASE-SETUP.md](./DATABASE-SETUP.md) | Database | Complete database setup (all environments) |
| [POSTGRESQL-CONNECTION-GUIDE.md](./POSTGRESQL-CONNECTION-GUIDE.md) | Database | **NEW!** PostgreSQL/Neon connection guide |
| [TROUBLESHOOTING-NEON.md](./TROUBLESHOOTING-NEON.md) | Database | **NEW!** Quick troubleshooting reference |
| [NEON-QUICKSTART.md](./NEON-QUICKSTART.md) | Database | 5-minute Neon PostgreSQL setup |
| [DATABASE-MIGRATION-GUIDE.md](./DATABASE-MIGRATION-GUIDE.md) | Database | EF Core migrations reference |
| [AUTH0-SETUP.md](./AUTH0-SETUP.md) | Security | **NEW!** Auth0 authentication setup guide |
| [CICD-PIPELINE.md](./CICD-PIPELINE.md) | CI/CD | Complete CI/CD workflow guide |
| [CICD-VALIDATION.md](./CICD-VALIDATION.md) | CI/CD | Pipeline validation report |
| [FLYIO-DEPLOYMENT.md](./FLYIO-DEPLOYMENT.md) | Deployment | Fly.io deployment guide |
| [DEPLOYMENT-SETUP-CHECKLIST.md](./DEPLOYMENT-SETUP-CHECKLIST.md) | Deployment | First-time setup checklist |
| [HOSTING-EVALUATION.md](./HOSTING-EVALUATION.md) | Infrastructure | Platform comparison |
| [COMMITLINT-SETUP.md](./COMMITLINT-SETUP.md) | Development | Commitlint configuration |
| [TESTING-COMMITLINT.md](./TESTING-COMMITLINT.md) | Development | Commitlint testing guide |

---

**Last Updated:** 2025-12-16  
**Maintained By:** GSC Development Team
