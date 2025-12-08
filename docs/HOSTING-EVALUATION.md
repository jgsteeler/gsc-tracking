# Hosting Alternatives Evaluation for GSC Tracking

**Document Version:** 1.0  
**Last Updated:** 2025-12-08  
**Author:** GSC Development Team  
**Status:** Evaluation Complete

---

## Executive Summary

This document evaluates hosting alternatives for the GSC Tracking business management application, comparing five major platforms: **Azure App Service**, **Fly.io**, **Railway**, **Netlify**, and **Cloudflare**. The application stack consists of:

- **Backend:** .NET 10 Web API (containerized)
- **Frontend:** React 19 + Vite (static site)
- **Database:** SQL/PostgreSQL (managed service needed)
- **Storage:** Blob storage for photos/documents

### Quick Recommendations

| Environment | Frontend | Backend | Database | Rationale |
|-------------|----------|---------|----------|-----------|
| **Dev** | Netlify (Free) | Fly.io (Free tier) | Fly.io Postgres (Free) | Zero cost, quick setup, dev-to-stage path |
| **Staging** | Netlify ($19/mo) | Fly.io ($10-20/mo) | Fly.io Postgres ($10/mo) | Same as prod setup but smaller resources |
| **Initial Prod** | Cloudflare Pages ($0-20/mo) | Railway ($5-20/mo) | Railway Postgres ($5-10/mo) | Simple, cost-effective, auto-scaling |
| **Scaled Prod** | Cloudflare Pages ($20/mo) | Azure App Service ($50-100/mo) | Azure SQL ($5-50/mo) | Enterprise-grade, best for scale |

**Total Cost Estimates:**
- **Development:** $0/month
- **Staging:** $40-50/month
- **Initial Production:** $10-50/month
- **Scaled Production:** $75-170/month

---

## Table of Contents

1. [Platform Overview](#platform-overview)
2. [Detailed Platform Analysis](#detailed-platform-analysis)
3. [Feature Comparison Matrix](#feature-comparison-matrix)
4. [Cost Analysis](#cost-analysis)
5. [Recommendations by Environment](#recommendations-by-environment)
6. [Migration Strategies](#migration-strategies)
7. [DNS and SSL Setup](#dns-and-ssl-setup)
8. [Testing and Deployment](#testing-and-deployment)
9. [Appendix: Configuration Examples](#appendix-configuration-examples)

---

## Platform Overview

### 1. Azure App Service (Current Recommendation in Business Analysis)

**Type:** Traditional PaaS  
**Best For:** Enterprise applications, .NET workloads, full Azure ecosystem integration  
**Container Support:** Yes (Linux and Windows containers)

**Highlight Features:**
- Deep .NET integration and optimization
- Comprehensive Azure ecosystem (SQL, Blob Storage, Key Vault, etc.)
- Enterprise-grade SLA (99.95% uptime)
- Advanced monitoring with Application Insights
- Easy scaling (vertical and horizontal)
- Managed SSL certificates (Let's Encrypt or custom)
- Deployment slots for blue-green deployments
- Built-in authentication providers (Auth0, AAD, etc.)

### 2. Fly.io

**Type:** Edge compute platform with global distribution  
**Best For:** Low-latency global apps, containers, PostgreSQL  
**Container Support:** Docker-native platform

**Highlight Features:**
- Deploy containers globally in seconds
- Built-in PostgreSQL (Fly Postgres) with clustering
- WireGuard VPN for secure private networking
- Automatic HTTPS with custom domains
- Run containers close to users (edge computing)
- Free tier includes 3 shared-cpu VMs + 3GB storage
- Simple CLI (`flyctl`) for deployment
- Zero cold starts (always-on containers)

### 3. Railway

**Type:** Developer-focused PaaS  
**Best For:** Rapid prototyping, simple deployments, built-in databases  
**Container Support:** Dockerfile auto-detection or Nixpacks

**Highlight Features:**
- One-click PostgreSQL, MySQL, MongoDB, Redis
- GitHub integration with auto-deploy on push
- Built-in observability (logs, metrics, traces)
- Usage-based pricing (pay for what you use)
- Instant preview environments for PRs
- Simple dashboard and CLI
- Free tier includes $5/month credit
- Native support for monorepos and multiple services

### 4. Netlify

**Type:** JAMstack/Static site platform with serverless functions  
**Best For:** Frontend hosting, static sites, serverless APIs  
**Container Support:** No (serverless functions only)

**Highlight Features:**
- Optimized for React, Vite, and modern frontend frameworks
- Global CDN with instant cache invalidation
- Atomic deploys with instant rollback
- Branch-based preview deployments
- Built-in forms, identity, and analytics
- Serverless functions (AWS Lambda backend)
- Free tier includes 100GB bandwidth/month
- Excellent DX with Netlify CLI

**Note:** Not suitable for .NET backend hosting (serverless functions only support Node, Go, etc.)

### 5. Cloudflare

**Type:** Edge platform (Workers + Pages)  
**Best For:** Global CDN, edge computing, DDoS protection  
**Container Support:** Limited (Workers are V8 isolates, not full containers)

**Highlight Features:**
- Cloudflare Pages for static frontend hosting
- Workers for edge serverless functions
- Unmetered DDoS protection (all plans)
- Global CDN with 300+ locations
- Workers KV, R2 (S3-compatible storage), D1 (SQLite edge database)
- Free tier includes unlimited requests
- API Gateway capabilities (reverse proxy, caching, security)
- Sub-second cold starts on Workers

**Note:** .NET backend would need to be hosted elsewhere; Cloudflare can proxy/cache requests

---

## Detailed Platform Analysis

### 1. Azure App Service

#### Pros
✅ Native .NET support - First-class support for .NET 10, optimized runtime  
✅ Integrated ecosystem - SQL, Storage, Key Vault, Application Insights work seamlessly  
✅ Enterprise SLA - 99.95% uptime guarantee, enterprise support available  
✅ Scaling - Easy vertical and horizontal scaling, autoscale rules  
✅ Security - Managed identities, Key Vault integration, network isolation (VNet)  
✅ CI/CD - Native GitHub Actions integration, deployment slots  
✅ Monitoring - Application Insights provides deep telemetry  

#### Cons
❌ Cost - More expensive than alternatives, especially at scale  
❌ Complexity - More services to configure (SQL, Storage, Key Vault, etc.)  
❌ Cold starts - App Service on Basic/Free tiers can have 5-10s cold starts  
❌ Regional limitations - Not as globally distributed as Fly.io or Cloudflare  

#### Cost (Monthly)
| Tier | App Service | SQL Database | Blob Storage | Total |
|------|-------------|--------------|--------------|-------|
| Dev | Free (F1) | Free (local) | Free (5GB) | $0 |
| Staging | B1 Basic ($13) | Serverless ($5) | $0.20 | ~$18 |
| Production | P1v3 ($100) | Gen Purpose ($15-50) | $1-5 | $116-155 |
| Scaled | P2v3+ ($200+) | Business ($50-200) | $10+ | $260-410+ |

---

### 2. Fly.io

#### Pros
✅ Docker-native - Deploy any Dockerfile, no platform lock-in  
✅ Global edge - Deploy to 30+ regions worldwide, run close to users  
✅ Fast deployments - Sub-minute deployments globally  
✅ Free tier - 3 shared-cpu VMs (256MB RAM each), 3GB storage, 160GB transfer  
✅ Built-in Postgres - Streaming replication, zero-config clustering  
✅ Private networking - WireGuard VPN between services  
✅ Zero cold starts - Containers always running (not serverless)  
✅ Cost-effective - ~$10-20/mo for small production apps  

#### Cons
❌ Smaller ecosystem - Not as mature as AWS/Azure/GCP  
❌ No managed SQL - Postgres is on Fly VMs (you handle backups)  
❌ Storage limitations - No native blob storage (use Tigris or external S3)  
❌ Support - Community support only (paid enterprise support available)  

#### Cost (Monthly)
| Tier | App Instances | Postgres | Storage | Total |
|------|--------------|----------|---------|-------|
| Dev | Free (3 shared VMs) | Free (shared) | Free (3GB) | $0 |
| Staging | 1x shared ($3) | 1x shared ($3) | 10GB SSD ($1) | ~$7 |
| Production | 2x dedicated ($15) | 2x HA ($20) | 50GB SSD ($5) | ~$40 |
| Scaled | 4-6x dedicated ($60) | 3x HA ($30) | 100GB ($10) | ~$100 |

---

### 3. Railway

#### Pros
✅ Developer-first UX - Beautiful dashboard, intuitive workflow  
✅ One-click databases - PostgreSQL, MySQL, MongoDB, Redis in seconds  
✅ GitHub integration - Auto-deploy on push, PR previews  
✅ Usage-based pricing - Pay only for what you use (CPU, RAM, bandwidth)  
✅ Built-in observability - Logs, metrics, traces out-of-the-box  
✅ Monorepo support - Deploy multiple services from one repo  
✅ Instant rollbacks - One-click rollback to previous deployment  
✅ Free tier - $5/month credit, no credit card required  

#### Cons
❌ Cost unpredictability - Usage-based can be hard to forecast  
❌ No managed backups - Database backups require external tools (pg_dump)  
❌ Regional limitations - Fewer regions than Fly.io or Cloudflare  
❌ Storage limitations - Volumes are expensive ($0.25/GB/month)  
❌ New platform - Less mature than Azure/AWS, smaller community  

#### Cost (Monthly)
| Tier | App Services | Database | Storage | Total |
|------|-------------|----------|---------|-------|
| Dev | $2-5 (usage) | $1-2 (usage) | Free | ~$3-7 |
| Staging | $8-12 (usage) | $3-5 (usage) | $5 (20GB) | ~$16-22 |
| Production | $15-25 (usage) | $5-10 (usage) | $10 (40GB) | ~$30-45 |
| Scaled | $50-100 (usage) | $15-30 (usage) | $20 (80GB) | ~$85-150 |

---

### 4. Netlify

#### Pros
✅ Frontend excellence - Best-in-class React/Vite/static site hosting  
✅ Global CDN - Instant cache invalidation, fast worldwide  
✅ Atomic deploys - Zero-downtime deploys with instant rollback  
✅ Branch previews - Every PR gets a unique preview URL  
✅ Free tier - 100GB bandwidth, unlimited sites  
✅ Build optimization - Automatic asset optimization  
✅ Forms/Identity - Built-in form handling and authentication  

#### Cons
❌ Backend limitations - Cannot host .NET backend (serverless only)  
❌ No container support - Not suitable for Docker-based apps  
❌ Serverless only - Functions limited to Node, Go, Rust (no .NET)  
❌ Cost at scale - Bandwidth overages can be expensive ($55/TB)  

#### Cost (Monthly)
- **Dev/Staging:** Free (100GB bandwidth, 300 build mins)
- **Production:** $19/site (400GB bandwidth, 1M function invocations)
- **Bandwidth overages:** $55/TB

**Note:** Backend must be hosted separately (Fly.io, Railway, Azure)

---

### 5. Cloudflare

#### Pros
✅ Unmetered DDoS protection - Best-in-class security on all plans  
✅ Global CDN - 300+ locations, fastest CDN globally  
✅ Free tier - Unlimited bandwidth, unlimited requests  
✅ API Gateway - Reverse proxy, caching, rate limiting for backend  
✅ R2 Storage - S3-compatible with zero egress fees  
✅ Workers - Edge functions for API routing, caching  
✅ Pages - Optimized for React, Vite, instant deploys  
✅ Zero cold starts - Workers run in <1ms  

#### Cons
❌ Backend limitations - Cannot run .NET backend (Workers are JS/WASM only)  
❌ No container support - Not suitable for Docker-based apps  
❌ D1 not production-ready - Edge database still in beta  
❌ Learning curve - Workers have unique constraints  

#### Cost (Monthly)
| Tier | Pages | Workers | R2 Storage | Total |
|------|-------|---------|------------|-------|
| Dev | Free | Free | Free (10GB) | $0 |
| Staging | Free | $5 (paid) | $0.50 (50GB) | $5.50 |
| Production | $20 (paid) | $5 (paid) | $1-5 (100GB+) | $26-30 |

**Note:** Backend must be hosted separately (Fly.io, Railway, Azure)

---

## Feature Comparison Matrix

| Feature | Azure | Fly.io | Railway | Netlify | Cloudflare |
|---------|-------|--------|---------|---------|------------|
| Container Support | ✅ | ✅ | ✅ | ❌ | ❌ |
| .NET 10 Hosting | ✅ | ✅ | ✅ | ❌ | ❌ |
| Managed DB | ✅ | ⚠️ | ✅ | ❌ | ⚠️ |
| Blob Storage | ✅ | ⚠️ | ⚠️ | ⚠️ | ✅ |
| Global Edge/CDN | ⚠️ | ✅ | ❌ | ✅ | ✅ |
| Auto-scaling | ✅ | ⚠️ | ⚠️ | ✅ | ✅ |
| Free Tier | ✅ | ✅ | ⚠️ | ✅ | ✅ |
| SSL Certificates | ✅ | ✅ | ✅ | ✅ | ✅ |
| CI/CD | ✅ | ✅ | ✅ | ✅ | ✅ |
| API Gateway | ⚠️ | ❌ | ❌ | ❌ | ✅ |
| DDoS Protection | ⚠️ | ⚠️ | ⚠️ | ✅ | ✅ |
| SLA/Uptime | 99.95% | 99.9% | 99.9% | 99.9% | 100% |

✅ = Full support | ⚠️ = Partial support | ❌ = Not supported

---

## Cost Analysis

### Total Cost Comparison (Monthly)

| Environment | Azure | Fly.io | Railway | Netlify + Fly.io | Cloudflare + Railway |
|-------------|-------|--------|---------|------------------|----------------------|
| Development | $0 | $0 | $3-7 | $0 | $0 |
| Staging | $18-25 | $7-10 | $16-22 | $7-10 | $11-16 |
| Initial Prod | $116-155 | $40-60 | $30-45 | $59-79 | $56-75 |
| Scaled Prod | $260-410 | $100-150 | $85-150 | $119-169 | $111-180 |

**Key Insights:**
- **Azure:** Higher cost but enterprise-grade features
- **Fly.io:** Best balance of features and cost
- **Railway:** Simplest setup, usage-based pricing
- **Netlify + Fly.io:** Best frontend + affordable backend
- **Cloudflare + Railway:** Best security/CDN + simple backend


---

## Recommendations by Environment

### Development Environment (Dev → Staging Path)

**Recommended Stack:**
```
Frontend: Netlify (Free tier)
Backend: Fly.io (Free tier: 3 shared VMs)
Database: Fly Postgres (Free tier)
Storage: Development only (local or Tigris free tier)
Auth: Auth0 (Free tier)
```

**Total Cost:** $0/month

**Rationale:**
- Zero cost during development
- Smooth transition to staging (same platforms)
- Learn deployment workflows without financial risk
- Fly.io free tier is generous (3 VMs, 3GB storage)
- Netlify branch previews for frontend testing
- Auth0 free tier supports 7,000 active users

**Setup Time:** 1-2 hours

---

### Staging Environment

**Recommended Stack:**
```
Frontend: Netlify (Pro plan: $19/mo)
Backend: Fly.io (1-2 dedicated VMs: $15-30/mo)
Database: Fly Postgres (HA setup: $10-20/mo)
Storage: Tigris or Azure Blob
Auth: Auth0 (Free tier)
Monitoring: Sentry (Free tier)
```

**Total Cost:** $44-69/month

**Rationale:**
- Mirrors production setup but with smaller resources
- Netlify Pro includes team features and preview environments
- Fly.io HA Postgres for testing failover/backup strategies
- Same deployment workflows as production

**Alternative (Azure-Native for Staging):**
```
Frontend: Azure Static Web Apps (Free)
Backend: Azure App Service (B1 Basic: $13/mo)
Database: Azure SQL (Serverless: $5/mo)
Storage: Azure Blob Storage ($1/mo)
Total: $19/month
```

---

### Initial Production (0-100 users)

**Recommended Stack (Cost-Optimized):**
```
Frontend: Cloudflare Pages ($0-20/mo)
Backend: Railway (Usage-based: $15-30/mo)
Database: Railway Postgres ($5-10/mo)
Storage: Cloudflare R2 ($1-5/mo)
Auth: Auth0 (Free tier, upgrade at 7k users)
Monitoring: Railway built-in + Sentry (Free)
```

**Total Cost:** $21-65/month

**Rationale:**
- Simple setup: Railway handles backend + database in one dashboard
- Cost-effective: Usage-based pricing scales with actual load
- Great UX: Railway's developer experience is best-in-class
- Security: Cloudflare DDoS protection and CDN included
- Storage: R2 has zero egress fees

**Alternative (Performance-Optimized):**
```
Frontend: Netlify (Pro: $19/mo)
Backend: Fly.io (2x dedicated VMs: $30/mo)
Database: Fly Postgres (HA: $20/mo)
Storage: Tigris ($1-5/mo)
Total: $70-74/month
```

**Alternative (Azure-Native):**
```
Frontend: Azure Static Web Apps (Free)
Backend: Azure App Service (B1 Basic: $13/mo)
Database: Azure SQL (Serverless: $10/mo)
Storage: Azure Blob Storage ($1/mo)
Total: $24/month
```

---

### Scaled Production (100-1000+ users)

**Recommended Stack (Enterprise-Grade):**
```
Frontend: Cloudflare Pages (Paid: $20/mo)
Backend: Azure App Service (P1v3: $100/mo)
Database: Azure SQL (General Purpose: $30-50/mo)
Storage: Azure Blob Storage ($5-10/mo)
CDN: Cloudflare (Free) as reverse proxy/cache
Auth: Auth0 (Essential: $35/mo for 500+ MAU)
Monitoring: Azure Application Insights (included)
```

**Total Cost:** $190-215/month

**Rationale:**
- Enterprise SLA: Azure 99.95% uptime
- Scalability: Easy horizontal scaling
- Monitoring: Application Insights provides deep diagnostics
- Security: Azure Key Vault, Managed Identity, VNet integration
- Compliance: SOC2, ISO certifications if needed
- Cloudflare CDN: Offload static assets, API caching, DDoS protection

**Alternative (Cost-Optimized at Scale):**
```
Frontend: Cloudflare Pages ($20/mo)
Backend: Fly.io (4-6 VMs: $60-90/mo)
Database: Fly Postgres (3x HA: $30-40/mo)
Storage: Cloudflare R2 ($10-20/mo)
Total: $120-170/month
```

---

## Migration Strategies

### Hybrid Approach (Recommended)

**Strategy:** Use multiple platforms for different components.

**Example Setup:**
```
Frontend: Cloudflare Pages (global CDN, DDoS)
Backend: Fly.io or Railway (containers, databases)
Storage: Cloudflare R2 (S3-compatible, zero egress)
CDN/Gateway: Cloudflare (cache API responses)
Auth: Auth0 (existing setup)
```

**Benefits:**
- Best-of-breed: Use each platform's strengths
- Avoid vendor lock-in: Containerization keeps backend portable
- Cost optimization: Cloudflare's free CDN + affordable backend
- Flexibility: Easy to swap backend provider (Fly.io ↔ Railway ↔ Azure)

**Migration Path:**
1. Start with Azure for backend (as planned in business analysis)
2. Add Cloudflare for CDN/security (no code changes)
3. Migrate frontend to Cloudflare Pages or Netlify (separate deployment)
4. Evaluate Fly.io/Railway after 3-6 months of production

### Containerization First

**Key Requirement:** Create Dockerfile for .NET backend

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GscTracking.Api.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GscTracking.Api.dll"]
```

This ensures portability between all container platforms (Fly.io, Railway, Azure, AWS, GCP).

---

## DNS and SSL Setup

### General DNS Configuration

All platforms require similar DNS setup:

1. **Add Custom Domain** in hosting platform dashboard
2. **Update DNS Records** at your domain registrar:

```dns
# For subdomain (app.example.com)
CNAME app   <platform-provided-hostname>

# For API subdomain (api.example.com)
CNAME api   <backend-platform-hostname>
```

3. **Verify Domain Ownership** (TXT record)
4. **Wait for SSL Provisioning** (5-30 minutes)

### SSL Certificates

All platforms provide **free Let's Encrypt SSL certificates** with auto-renewal:

| Platform | Certificate | Renewal | Custom CA |
|----------|------------|---------|-----------|
| Azure | Let's Encrypt | Auto (60d) | Yes |
| Fly.io | Let's Encrypt | Auto (60d) | No |
| Railway | Let's Encrypt | Auto (60d) | No |
| Netlify | Let's Encrypt | Auto (60d) | Yes (paid) |
| Cloudflare | Cloudflare | Auto (60d) | Yes (paid) |

**Best Practice:** Use Cloudflare in front of any backend for:
- Free SSL (Flexible, Full, Full Strict modes)
- DDoS protection
- CDN caching
- Rate limiting
- Firewall rules

---

## Testing and Deployment

### Testing Approach

**Phase 1: Local Development**
```bash
# Backend
cd backend/GscTracking.Api
dotnet run

# Frontend
cd frontend
npm run dev
```

**Phase 2: Deploy to Fly.io (Test)**

1. Create Dockerfile for backend (see example above)
2. Deploy to Fly.io:
   ```bash
   flyctl launch --name gsc-tracking-api-test
   flyctl postgres create --name gsc-tracking-db
   flyctl postgres attach gsc-tracking-db
   flyctl deploy
   ```
3. Test API:
   ```bash
   curl https://gsc-tracking-api-test.fly.dev/api/health
   ```

**Phase 3: Deploy Frontend to Netlify**

1. Connect GitHub repo to Netlify dashboard
2. Configure build settings:
   - Base directory: `frontend`
   - Build command: `npm run build`
   - Publish directory: `frontend/dist`
3. Add environment variables:
   - `VITE_API_URL=https://gsc-tracking-api-test.fly.dev`
   - `VITE_AUTH0_DOMAIN=<your-auth0-domain>`
   - `VITE_AUTH0_CLIENT_ID=<your-client-id>`
4. Netlify auto-deploys on push to `main`

**Phase 4: Performance Testing**

Use tools like:
- **k6** for load testing
- **Lighthouse** for frontend performance
- **Apache JMeter** for API stress testing

---

## Appendix: Configuration Examples

### A. Dockerfile for .NET 10 Backend

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GscTracking.Api/GscTracking.Api.csproj", "GscTracking.Api/"]
RUN dotnet restore "GscTracking.Api/GscTracking.Api.csproj"
COPY . .
WORKDIR "/src/GscTracking.Api"
RUN dotnet build "GscTracking.Api.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "GscTracking.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GscTracking.Api.dll"]
```

### B. fly.toml (Fly.io Configuration)

```toml
app = "gsc-tracking-api"
primary_region = "iad"

[build]
  dockerfile = "Dockerfile"

[env]
  ASPNETCORE_ENVIRONMENT = "Production"
  ASPNETCORE_URLS = "http://+:8080"

[[services]]
  internal_port = 8080
  protocol = "tcp"

  [[services.ports]]
    handlers = ["http"]
    port = 80
    force_https = true

  [[services.ports]]
    handlers = ["tls", "http"]
    port = 443

  [[services.http_checks]]
    interval = "30s"
    timeout = "10s"
    method = "GET"
    path = "/health"
```

### C. netlify.toml (Netlify Configuration)

```toml
[build]
  base = "frontend"
  command = "npm run build"
  publish = "dist"

[[redirects]]
  from = "/api/*"
  to = "https://gsc-tracking-api.fly.dev/api/:splat"
  status = 200
  force = true

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200

[[headers]]
  for = "/assets/*"
  [headers.values]
    Cache-Control = "public, max-age=31536000, immutable"
```

### D. Docker Compose (Local Development)

```yaml
version: '3.8'

services:
  api:
    build:
      context: ./backend
      dockerfile: GscTracking.Api/Dockerfile
    ports:
      - "5091:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=gsc_tracking;User=postgres;Password=postgres;
    depends_on:
      - db

  db:
    image: postgres:16-alpine
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=gsc_tracking
    volumes:
      - postgres_data:/var/lib/postgresql/data

  frontend:
    build:
      context: ./frontend
    ports:
      - "5173:5173"
    environment:
      - VITE_API_URL=http://localhost:5091
    volumes:
      - ./frontend:/app
      - /app/node_modules

volumes:
  postgres_data:
```

---

## Summary and Action Items

### Key Takeaways

1. **No single "best" platform** – Choose based on:
   - Team expertise and preferences
   - Budget constraints
   - Scalability requirements
   - Geographic distribution needs

2. **Hybrid approach recommended** for GSC Tracking:
   - Frontend: Cloudflare Pages or Netlify (global CDN, free tier)
   - Backend: Fly.io or Railway (cost-effective, container-native)
   - Database: Co-located with backend (Fly Postgres or Railway PG)
   - Storage: Cloudflare R2 or Azure Blob

3. **Start simple, scale later:**
   - Dev: Free tiers on Fly.io + Netlify ($0/mo)
   - Staging: Upgrade to paid tiers ($40-50/mo)
   - Prod: Start with Railway or Fly.io ($30-70/mo)
   - Scale: Migrate to Azure if enterprise features needed ($150-250/mo)

4. **Containerization is key:**
   - Write Dockerfile for .NET backend
   - Ensures portability between all platforms

### Immediate Action Items

- [ ] Create Dockerfile for .NET 10 backend (see Appendix A)
- [ ] Test deployment to Fly.io (free tier, no cost)
- [ ] Deploy frontend to Netlify (connect GitHub repo)
- [ ] Document deployment process for team
- [ ] Set up monitoring (Sentry or platform-native)
- [ ] Configure Auth0 for dev/staging/prod environments
- [ ] Test performance with k6 (see Testing section)
- [ ] Estimate real costs after 1 month of staging usage

### Long-Term Considerations

- Multi-region deployment (Fly.io or Cloudflare for global users)
- Database replication (read replicas for reporting queries)
- CDN optimization (Cloudflare for API caching, image optimization)
- Observability stack (logs, metrics, traces with OpenTelemetry)
- Cost optimization (reserved instances, usage monitoring)

---

## Conclusion

The GSC Tracking application has multiple viable hosting options. The **recommended approach** is:

1. **Start with Fly.io + Netlify** for development and staging (low cost, great DX)
2. **Launch production with Railway or Fly.io** (simple, cost-effective)
3. **Migrate to Azure** if enterprise features or scale requirements emerge
4. **Use Cloudflare** as CDN/security layer regardless of backend host

This strategy provides **flexibility**, **cost optimization**, and a clear **path to scale** as the business grows.

---

**Document Status:** ✅ Complete  
**Next Review Date:** 2026-03-08 (or after 3 months of production usage)  
**Feedback:** Open an issue in the repository to suggest improvements

---

**References:**
- [Fly.io Documentation](https://fly.io/docs/)
- [Railway Documentation](https://docs.railway.app/)
- [Netlify Documentation](https://docs.netlify.com/)
- [Cloudflare Pages Documentation](https://developers.cloudflare.com/pages/)
- [Azure App Service Documentation](https://learn.microsoft.com/en-us/azure/app-service/)
- [GSC Tracking Business Analysis](../business-management-app-analysis.md)
- [Setup Instructions](../SETUP-INSTRUCTIONS.md)
