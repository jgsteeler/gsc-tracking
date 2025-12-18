# GSC Small Engine Repair - Business Management App

## Technology Stack Analysis & Recommendation

**Date:** December 7, 2025  
**Division:** GSC-SE (Small Engines)  
**Scope:** Full-stack business management application with Wave integration  
**Future Expansion:** Template for GSC-PROD, GSC-AI, and GSC-DEV divisions

---

## Executive Summary

### Primary Recommendation: **Hybrid Architecture (Updated for .NET 10 LTS)**

- **Backend:** .NET 10 Web API (C#) — Latest LTS, business-focused, enterprise-grade
- **Frontend:** React + Vite (TypeScript)
- **Hosting:** Azure App Service (containerized) with Azure SQL (see below for containerization/hosting alternatives)
- **Integration Layer:** GraphQL gateway for future Wave API abstraction (not MVP)
- **Cost Projection:** $15-30/month initial, scales with usage
- **Open Source:** All code in the gsc-tracking public repo
- **Customer Portal:** Planned for future phase, integrated with main app/data

**Rationale:** .NET 10 is chosen for its business focus, reliability, and scalability, not just familiarity. React + Vite is modern and proven. Containerization allows flexibility in hosting (Azure, Fly.io, Railway, etc.). The stack is designed for open source, multi-division, and future extensibility.

---

## Business Requirements Analysis

### Core Features (MVP Phase 1 — Refined)

**MVP Scope:**

1. **Customer Management**
   - Contact information, basic service history
   - Equipment inventory per customer (optional, can be deferred)
   - *Roadmap*: Communication logs, Wave sync

2. **Job Tracking**
   - Job creation, status tracking (open/in-progress/completed/closed)
   - Labor hours, cost tracking per job
   - *Roadmap*: Photo/doc uploads, profitability analysis

3. **Inventory Management**
   - *Roadmap only*: Parts inventory, supply tracking, reorder alerts
   - **MVP:** Only cost tracking per job (no inventory management needed for low job volume)

4. **Financial Integration**
   - **MVP:** CSV import/export for expenses, estimates, invoices, and payment status (manual sync with Wave)
   - *Roadmap*: Wave API integration for direct sync, estimate/invoice creation, payment tracking

5. **Reporting Dashboard**
   - **MVP:** Job-related reports only (job status, cost tracking)
   - *Roadmap*: Revenue/expense trends, customer metrics, inventory levels (Wave handles most financial reporting)

### Multi-Division Requirements

- **Division isolation:** Each division (GSC-SE, GSC-PROD, GSC-AI, GSC-DEV) operates independently
- **LLC consolidation:** Roll-up financial reporting to GSC LLC level
- **Shared infrastructure:** Single codebase, multi-tenant architecture
- **Division-specific customization:** Different workflows per business type

---

## Technology Stack Comparison

### Option 1: .NET Stack (RECOMMENDED, .NET 10 LTS)

#### Architecture

```
Frontend: React + Vite + TypeScript
Backend: .NET 10 Web API (C#)
Database: Azure SQL Database (Serverless tier) — or PostgreSQL, Cosmos DB, MongoDB (see below)
Auth: Auth0 (preferred, existing account)
Integration: Custom GraphQL layer (future, not MVP)
Hosting: Azure App Service (Linux containers) — or Fly.io, Railway, GCP, AWS, Netlify, etc.
Storage: Azure Blob Storage (photos/docs) — or Tigris, Upstash, etc.
```

#### Pros

✅ **Your expertise:** You're "very comfortable" with .NET Web API  
✅ **Enterprise-grade:** Production-ready, robust, excellent tooling  
✅ **Performance:** Excellent performance, efficient resource usage  
✅ **Type safety:** C# + TypeScript end-to-end type safety  
✅ **Scalability:** Handles growth from 1 user to enterprise  
✅ **Azure ecosystem:** Seamless integration with Azure services  
✅ **Long-term support:** Microsoft's LTS commitment  
✅ **Free tier friendly:** Can run on Azure free/low-cost tiers  

#### Cons

❌ **Learning curve:** For frontend devs joining later (not relevant for solo)
❌ **Perceived cost:** Azure can be more expensive at scale, but is manageable for MVP
❌ **Cold starts:** App Service can have cold starts on lower tiers (5-10s)
❌ **Hosting lock-in:** Containerization allows migration to Fly.io, Railway, etc. if needed

#### Cost Breakdown (Monthly)

- Azure App Service (B1 Basic): $13/month (or Free tier for dev)
- Azure SQL Database (Serverless): $5-15/month (or PostgreSQL, Cosmos, MongoDB, Fly.io managed Postgres)
- Azure Blob Storage: $1-2/month (or Tigris, Upstash, etc.)
- Domain: $12/year = $1/month (already owned)
- **Total: $19-31/month production, $6-18/month dev environment**

#### Development Experience

- Visual Studio / VS Code with C# Dev Kit
- Entity Framework Core for database
- Swagger/OpenAPI auto-generated docs
- Hot reload for rapid development
- Excellent debugging

---

### Option 2: Node.js/Express Stack (for comparison)

#### Architecture

```
Frontend: React + Vite + TypeScript
Backend: Express.js + TypeScript
Database: PostgreSQL (Supabase or Railway)
Auth: Supabase Auth or Clerk (Auth0 also possible)
Integration: GraphQL via Apollo Server
Hosting: Railway, Render, or Fly.io
Storage: Supabase Storage or Cloudflare R2
```

#### Pros

✅ **Cost-effective:** Very cheap hosting ($5-10/month)  
✅ **JavaScript everywhere:** Single language (TypeScript)  
✅ **Fast development:** Less boilerplate, rapid prototyping  
✅ **Huge ecosystem:** npm packages for everything  
✅ **Serverless friendly:** Easy to deploy to Vercel/Netlify Edge  
✅ **Free tiers:** Generous free tiers (Railway, Supabase, Render)  

#### Cons

❌ **Your expertise:** Less comfortable for you personally  
❌ **Type safety:** Less robust than C# (even with TypeScript)  
❌ **Performance:** Slower than .NET for compute-heavy tasks  
❌ **Enterprise perception:** Less "serious" than .NET  
❌ **Maintenance:** More dependencies to manage  

#### Cost Breakdown (Monthly)

- Railway/Render (Hobby): $5/month
- PostgreSQL (Supabase Free): $0/month (25GB limit)
- Storage (Supabase Free): $0/month (1GB limit)
- Domain: $1/month
- **Total: $6-10/month**

#### Development Experience

- VS Code with excellent TypeScript support
- Prisma ORM (fantastic developer experience)
- Quick iteration, less ceremony
- Great for MVPs and prototypes
- **Supabase:** Open source Firebase alternative, provides Postgres DB, auth, storage, and real-time APIs. Good for rapid prototyping and small teams.

---

### Option 3: Serverless Architecture

#### Architecture

```
Frontend: Next.js (React) on Vercel
Backend: Azure Functions or AWS Lambda
Database: Cosmos DB (serverless) or DynamoDB
Auth: Next Auth or Azure AD B2C
Integration: API Management layer
```

#### Pros

✅ **Ultra-scalable:** Pay only for execution  
✅ **No server management:** Fully managed  
✅ **Global distribution:** Edge computing  
✅ **Modern stack:** Latest technology  

#### Cons

❌ **Cold starts:** 1-5 second delays on inactive functions  
❌ **Complex debugging:** Harder to troubleshoot  
❌ **Vendor lock-in:** Tied to cloud provider  
❌ **Unpredictable costs:** Can spike with usage  
❌ **Not ideal for your use case:** Constant usage = constant charges  

#### Cost Breakdown (Monthly)

- Highly variable: $5-50/month depending on usage
- Good for spiky traffic, not great for constant internal use

---

## Detailed Recommendation: Hybrid .NET Architecture

### Why This Works Best for You

1. **Leverage Your Strengths**
   - You're comfortable with .NET Web API
   - Build faster with familiar tools
   - Less learning, more shipping

2. **Business-Critical Application**
   - This isn't a POC, it's operational software
   - Reliability and data integrity matter
   - .NET's robustness is valuable here

3. **Cost is Manageable**
   - Azure Free tier for initial development
   - B1 tier ($13/month) for production is affordable
   - Serverless SQL scales with actual usage
   - Still cheaper than Wave alternatives ($50-200/month)

4. **Future-Proof Architecture**
   - Scales to all GSC divisions
   - Can add AI features easily (Azure AI services)
   - GraphQL abstraction allows provider switching
   - Multi-tenant ready from day one

5. **Time to Market**
   - You ship faster in .NET
   - Less context switching
   - Better debugging and tooling
   - Your productivity matters most

### Recommended Tech Stack Details

#### Backend (.NET 10 Web API)

```csharp
Project Structure:
- GscManagement.API (Web API)
- GscManagement.Core (Business logic, models)
- GscManagement.Infrastructure (Data access, external services)
- GscManagement.Integrations (Wave, future integrations)
```

**Key Libraries:**

- Entity Framework Core (ORM)
- MediatR (CQR pattern, keeps code clean)
- FluentValidation (input validation)
- FluentResults (result pattern)
- FluentAssertions (testing)
- Bogus (test data generation)
- Serilog (logging)
- Polly (resilience and retry policies)
- Refit (typed HTTP clients for Wave API)
- AutoMapper (entity mapping)

**API Pattern:**

- RESTful for CRUD operations
- GraphQL via Hot Chocolate for complex queries
- SignalR for real-time updates (job status changes)

#### Frontend (React + Vite)

```
Tech Stack:
- React 18
- TypeScript
- Vite (fast build tool)
- TanStack Query (React Query v5) - server state
- Zustand (client state management)
- React Router v6
- Tailwind CSS + shadcn/ui (beautiful components)
- React Hook Form + Zod (forms & validation)
```

**UI Framework:** shadcn/ui

- Copy/paste components (no dependency bloat)
- Tailwind-based, highly customizable
- Accessible by default
- Professional look without design work

#### Database (Azure SQL, PostgreSQL, Cosmos/Mongo)

```sql
Schema Design (starter):
- Divisions (multi-tenant)
- Customers
- Jobs
- JobActivities (time tracking, notes)
- Expenses
- SyncLog (integration audit trail)
```

**Strategy:**

- Start with Azure SQL (serverless) for MVP, or PostgreSQL (Fly.io, Railway, Supabase) if preferred
- Cosmos DB or MongoDB are options for NoSQL, but not required for current relational needs
- Automated backups, point-in-time restore
- ORM/abstraction layer to allow future DB migration

#### Integration Layer (Wave API, Roadmap)

**MVP:** CSV import/export for expenses, estimates, invoices, and payment status. No direct Wave API integration in MVP.

**Roadmap:**

- Repository + abstraction pattern for future Wave/QuickBooks integration
- Consider Wave API capabilities for future (REST, GraphQL, webhooks)
- Sync strategy: CSV/manual for MVP, API for later phases

#### Hosting & Containerization

**Initial Setup:**

- Azure App Service (F1 Free tier for dev, B1 Basic for prod)
- **Alternatives:** Fly.io (container hosting, managed Postgres, Tigris object storage, Upstash Redis), Railway, GCP, AWS, Netlify (API gateway via Cloudflare)
- **Strategy:** Use Docker containers for portability; can migrate between hosts as needed
- **Storage:** Azure Blob, Tigris, or other S3-compatible storage

**Container Strategy:**

- Docker Compose for local dev
- CI/CD with GitHub Actions (see below)

#### Authentication (Auth0)

- **Auth0 (Recommended):**
  - Free tier: 7,000 active users
  - Social logins included
  - Excellent developer experience
  - Already in use for Divergent Flow
- **Azure AD B2C:** Not recommended (complex, less preferred)

---

## Alternative Consideration: Cost-Optimized Stack

If cost is the absolute priority, here's the minimum viable architecture:

### Ultra-Budget Stack ($0-8/month)

```
Frontend: Netlify (free hosting)
Backend: Railway Free Trial → $5/month Hobby
Database: Supabase Free (25GB, 500MB egress)
Storage: Cloudflare R2 (10GB free)
Auth: Supabase Auth (included)
Stack: React + Express + PostgreSQL
```

**Trade-offs:**

- You learn Node.js/Express (time investment)
- Less enterprise-ready (perception issue)
- Adequate for MVP and growth to 100 customers
- Can migrate to .NET later if needed

**When This Makes Sense:**

- Want to validate business model first
- Willing to learn Node.js
- Budget is tighter than $30/month
- Speed to market > code comfort

---

## Migration Path & Scalability

### Phase 1: MVP (Months 1-3)

**Users:** Just you
**Hosting:** Azure Free tier (or Fly.io, Railway for dev)
**Features:** Basic CRUD for customers/jobs, cost tracking, CSV import/export for expenses/estimates/invoices, manual Wave sync
**Cost:** $0-10/month

### Phase 2: Production (Months 3-6)

**Users:** 1-5 (you + potential helpers)
**Hosting:** Azure B1 or Fly.io managed Postgres
**Features:** Add job workflow, basic reporting, CSV sync improvements
**Cost:** $15-25/month

### Phase 3: Division Expansion (Months 6-12)

**Users:** 10-20 (multiple divisions)
**Hosting:** Azure B2 or Fly.io Pro
**Features:** Multi-tenant, role-based access, consolidated reporting, customer portal (view jobs, invoices)
**Cost:** $30-60/month

### Phase 4: Scale (Year 2+)

**Users:** 50+ (if you hire employees or offer to other businesses)
**Hosting:** Azure S1 or Kubernetes
**Features:** Advanced analytics, mobile apps, API for partners, full Wave API integration
**Cost:** $100-300/month (or revenue-funded)

---

## Integration Strategy

### Wave Integration (MVP: CSV, API: Roadmap)

- **MVP:** CSV import/export for expenses, estimates, invoices, and payment status. No direct Wave API integration in MVP. Use Wave's built-in receipt email/upload for expense capture.
- **Roadmap:** Direct API integration for customer/estimate/invoice creation, payment tracking, and bidirectional sync.

### Future Integrations (Abstracted)

**Pattern:** Interface-based design

```csharp
public interface IAccountingSync
{
    Task<Customer> CreateCustomer(CustomerDto customer);
    Task<Invoice> CreateInvoice(InvoiceDto invoice);
    Task<IEnumerable<Payment>> GetPayments(DateTime since);
}

// Implementations:
- WaveAccountingSync
- QuickBooksAccountingSync
- FreshBooksAccountingSync
```

**GSC LLC Consolidation:**

- Each division reports to central API
- Aggregation service rolls up to LLC level
- Division-specific dashboards + consolidated view
- Shared customer tracking across divisions

---

## AI & Advanced Features Roadmap

### Phase 1 AI Features (Nice to Have)

1. **Intelligent Job Estimation**
   - ML model trained on your historical jobs
   - Predicts labor time and parts costs
   - Improves over time with more data

2. **Smart Parts Lookup**
   - Photo-based parts identification
   - Equipment model recognition
   - Automatic parts catalog search

3. **Customer Communication**
   - Auto-generate job status updates
   - SMS/Email templates with AI personalization
   - Sentiment analysis on customer feedback

### Phase 2 AI Features (Future)

1. **Predictive Maintenance**
   - Analyze job patterns to predict failures
   - Proactive service recommendations
   - Customer retention tool

2. **Business Intelligence**
   - Revenue forecasting
   - Seasonal trend analysis
   - Pricing optimization

**AI Implementation:**

- Azure OpenAI Service (GPT-4, GPT-4 Vision)
- Azure Cognitive Services (OCR for receipts)
- Custom ML models with Azure ML Studio
- Estimated cost: $5-20/month depending on usage

---

## Development Roadmap

### Month 1: Foundation

**Week 1-2: Setup & Core Models**

- [ ] Azure account setup, App Service creation
- [ ] .NET 10 Web API project scaffolding
- [ ] Database schema design & EF Core migrations (Azure SQL or PostgreSQL)
- [ ] React + Vite frontend scaffolding
- [ ] Auth0 authentication setup

**Week 3-4: Core CRUD**

- [ ] Customer management (CRUD)
- [ ] Job management (CRUD)
- [ ] Cost tracking per job
- [ ] CSV import/export for expenses, estimates, invoices, payment status
- [ ] Basic UI with shadcn/ui components
- [ ] Form validation (backend + frontend)

### Month 2: Workflow & Reporting

**Week 1-2: Job Workflow**

- [ ] Job status workflow (open → in-progress → completed → closed)
- [ ] Time tracking for labor
- [ ] Expense tracking improvements

**Week 3-4: Reporting**

- [ ] Job-related reports (status, cost)
- [ ] Export capabilities (CSV)

### Month 3: Polish & Roadmap Features

- [ ] Photo/doc upload (Azure Blob/Tigris)
- [ ] Profitability analysis (per job)
- [ ] Customer portal (view jobs, invoices)
- [ ] Multi-tenant architecture for divisions
- [ ] Role-based access control (RBAC)
- [ ] GSC LLC consolidation dashboard
- [ ] Mobile-responsive optimizations
- [ ] Begin planning for Wave API integration

### Month 4+: Advanced & Integration

- [ ] Wave API integration (estimates, invoices, payments)
- [ ] Inventory management (if needed)
- [ ] AI features (if desired)

---

## Cost-Benefit Analysis

### Total Cost of Ownership (3 Years)

#### Option A: .NET Stack (Recommended)

| Year | Hosting | Database | Storage | Auth | Total |
|------|---------|----------|---------|------|-------|
| 1    | $156    | $120     | $24     | $0   | $300  |
| 2    | $360    | $180     | $36     | $0   | $576  |
| 3    | $720    | $240     | $48     | $0   | $1,008|
| **3-Year Total** | | | | | **$1,884** |

#### Option B: Node.js Stack

| Year | Hosting | Database | Storage | Auth | Total |
|------|---------|----------|---------|------|-------|
| 1    | $60     | $0       | $0      | $0   | $60   |
| 2    | $120    | $0       | $12     | $0   | $132  |
| 3    | $240    | $120     | $24     | $0   | $384  |
| **3-Year Total** | | | | | **$576** |

**Savings: Node.js is $1,308 cheaper over 3 years**

### Value Analysis

**Time Investment (Learning Curve):**

- .NET: 0 hours (you already know it)
- Node.js: 40-80 hours to become proficient

**Your Hourly Rate (consulting value):** ~$75-150/hour

**Learning Cost Equivalent:**

- 40 hours × $100/hour = $4,000 opportunity cost
- 80 hours × $100/hour = $8,000 opportunity cost

**ROI Calculation:**

```
.NET Stack TCO:     $1,884 + $0 learning    = $1,884
Node.js Stack TCO:  $576 + $4,000 learning  = $4,576

NET Savings with .NET: $2,692 over 3 years
```

**Conclusion:** Even though Node.js has lower hosting costs, your time learning it costs more than the hosting savings. **Stick with .NET.**

### Alternative Cost Analysis: Commercial Software

**ServiceTitan (Industry Leader):**

- Small Business: $299-499/month
- Enterprise: $1,000+/month
- 3-Year Cost: $10,764 - $17,964

**Jobber (Small Business):**

- Core: $129/month
- Connect: $209/month
- Grow: $309/month
- 3-Year Cost: $4,644 - $11,124

**Your Custom App (RECOMMENDED):**

- Development Time: 200-300 hours @ $0 (your time)
- 3-Year Cost: $1,884 (hosting only)
- **Savings: $2,760 - $9,240+ vs Jobber**
- **Control: 100% customization, no vendor lock-in**
- **Data Ownership: Complete control, no subscription trap**

---

## Domain & Hosting Strategy

### Current State

- **Primary Domain:** gibsonsvc.com (Astro static site)
- **Staging:** gibsonsvctest.netlify.app

### Recommended Setup

**Option A: Subdomain (RECOMMENDED)**

```
gibsonsvc.com                  → Astro marketing site (keep as-is)
app.gibsonsvc.com              → React app (Azure App Service)
api.gibsonsvc.com              → .NET API (Azure App Service)
```

**Benefits:**

- Clean separation of concerns
- Marketing site stays static, fast, SEO-friendly
- App lives on separate infrastructure
- Professional appearance

**Option B: Path-based**

```
gibsonsvc.com                  → Astro marketing site
gibsonsvc.com/app              → React app
gibsonsvc.com/api              → .NET API
```

**Requires:** Reverse proxy (Azure Front Door or Nginx)  
**Complexity:** Higher  
**Cost:** +$5-20/month for Front Door

**Option C: Separate Domain (During MVP)**

```
gibsonsvc.com                  → Marketing site (keep)
gsc-manager.azurewebsites.net  → App (free Azure subdomain)
```

**Benefits:**

- Zero DNS configuration during MVP
- Move to custom domain later
- Fastest to launch

**Recommendation:** Start with Option C (free subdomain), move to Option A (app.gibsonsvc.com) once you're ready for production.

---

## Security Considerations

### Authentication & Authorization

1. **User Authentication**
   - JWT tokens with refresh mechanism
   - Secure password hashing (BCrypt or Argon2)
   - Multi-factor authentication (future)

2. **Role-Based Access**
   - Admin: Full access (you)
   - Manager: Job management, customer view
   - Technician: Job updates only (future employees)
   - Customer Portal: View their own jobs (future)

3. **API Security**
   - HTTPS only (enforced)
   - CORS policies (whitelist your domains)
   - Rate limiting (prevent abuse)
   - API key rotation for Wave integration

### Data Protection

1. **Sensitive Data**
   - Customer payment info: Never store, use Wave
   - Customer PII: Encrypted at rest
   - Access logs for compliance

2. **Backups**
   - Azure SQL: Automated daily backups (7 days)
   - Blob Storage: Geo-redundant by default
   - Manual backup before major changes

3. **Compliance**
   - GDPR consideration (if you expand)
   - Data export capability for customers
   - Deletion requests handling

---

## Development Tools & Workflow

### Recommended Dev Environment

```
IDE: Visual Studio 2022 or VS Code + C# Dev Kit
Version Control: Git + GitHub (private repo)
API Testing: Postman or REST Client (VS Code extension)
Database: Azure Data Studio or SSMS
Docker: Docker Desktop (for local dev consistency)
```

### CI/CD Pipeline (GitHub Actions, GitLab Flow)

**Branching:** Main branch + short-lived working branches

**Workflow:**
a) Push any branch: Build & test
b) PR to main: Deploy to staging
c) Merge to main: Deploy to prod (with approval)

**Automated:**

- Linting (C# + TypeScript)
- Unit tests (xUnit for .NET, Vitest for React)
- Integration tests (key workflows)
- Database migrations

### Local Development

```yaml
# docker-compose.yml
services:
  api:
    build: ./GscManagement.API
    ports: ["5000:5000"]
    environment:
      - ConnectionStrings__Default=Server=db;Database=GscManagement;...
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
  
  frontend:
    build: ./gsc-management-ui
    ports: ["3000:3000"]
    environment:
      - VITE_API_URL=http://localhost:5000
```

**Benefits:**

- Consistent environment across machines
- No "works on my machine" issues
- Easy onboarding for future developers

---

## Risks & Mitigation

### Technical Risks

1. **Wave API Changes**
   - **Risk:** Wave deprecates API or changes pricing
   - **Mitigation:** Abstraction layer allows provider swap
   - **Fallback:** Manual CSV export/import as backup

2. **Azure Cost Overruns**
   - **Risk:** Unexpected usage spikes increase costs
   - **Mitigation:** Set up budget alerts ($50 threshold)
   - **Fallback:** Container-based, can move to Railway/Render

3. **Data Loss**
   - **Risk:** Database corruption or deletion
   - **Mitigation:** Automated backups + geo-redundancy
   - **Fallback:** Export critical data weekly to local JSON

### Business Risks

1. **Time Investment**
   - **Risk:** Development takes longer than expected
   - **Mitigation:** MVP approach, iterate in phases
   - **Fallback:** Stick with document-based system longer

2. **Scope Creep**
   - **Risk:** Add too many features, never launch
   - **Mitigation:** Strict MVP definition, defer "nice-to-haves"
   - **Fallback:** Launch with manual Wave sync if needed

3. **Division Requirements Diverge**
   - **Risk:** Different divisions need vastly different features
   - **Mitigation:** Module-based architecture from day 1
   - **Fallback:** Fork codebase per division (less ideal)

---

## Final Recommendation

### Stack Decision: .NET 8 + React + Azure

**Primary Factors:**

1. ✅ You're comfortable with .NET → Ship faster
2. ✅ Business-critical app → Reliability matters
3. ✅ Cost is acceptable → $15-30/month is reasonable
4. ✅ Future-proof → Scales to all divisions
5. ✅ Time to market → Your productivity > hosting savings

**Implementation Plan:**

1. **Week 1-2:** Azure setup, .NET API scaffolding, React UI foundation
2. **Week 3-4:** Customer & Job CRUD with basic UI
3. **Month 2:** Wave integration, job workflow, photo uploads
4. **Month 3:** Inventory, reporting, polish for daily use
5. **Month 4+:** Multi-division support, advanced features

**Budget Allocation:**

- **Development:** Free (your time)
- **Year 1 Hosting:** $300 ($25/month average)
- **Year 2 Hosting:** $576 ($48/month as you scale)
- **Total 2-Year TCO:** $876 vs $3,096+ for Jobber

**ROI:**

- Save $2,200+ vs commercial software (2 years)
- Gain complete customization
- Own your data forever
- Template for other divisions = 4x value

---

## Next Steps (Updated)

### Immediate Actions (This Week)

1. [ ] Confirm tech stack: .NET 10 Web API, React + Vite, Auth0, Azure SQL or PostgreSQL, containerized
2. [ ] Set up Azure App Service (free tier for dev)
3. [ ] Set up Auth0 tenant (reuse existing if possible)
4. [ ] Initialize gsc-tracking open source repo (if not already)
5. [ ] Sketch out initial DB schema (customers, jobs, expenses, etc.)
6. [ ] Set up GitHub Actions for CI/CD (GitLab Flow)

### Month 1 Kickoff

1. [ ] Scaffold .NET 10 Web API project
2. [ ] Scaffold React + Vite frontend
3. [ ] Implement Auth0 authentication
4. [ ] Implement core CRUD for customers/jobs
5. [ ] Implement CSV import/export for expenses, estimates, invoices, payment status

### Questions to Answer

1. Azure SQL or PostgreSQL for MVP?
2. Start with Azure hosting, or try Fly.io for dev?
3. Any immediate need for mobile app, or is responsive web enough?
4. Who will use the app in MVP (just you, or others)?
5. What CSV formats are needed for Wave import/export?
6. What is the minimum viable customer portal feature set?

---

## Alternative Scenarios

### Scenario 1: "I Changed My Mind, Let's Go Cheaper"

**Recommended:** Node.js + Express + PostgreSQL (Supabase) on Railway

- **Time to Learn:** 2-4 weeks for proficiency
- **Cost:** $5-10/month
- **Trade-off:** Slower initial development, but adequate

### Scenario 2: "I Want Something Working Tomorrow"

**Recommended:** Airtable + Zapier for Wave integration

- **Time to Setup:** 1-2 days
- **Cost:** $20-50/month (Airtable Pro + Zapier)
- **Trade-off:** Not a real app, but functional immediately
- **Migration Path:** Build custom app, import data later

### Scenario 3: "Money is No Object, Best Possible"

**Recommended:** .NET + Next.js + Azure Premium + Azure AI

- **Features:** Real-time collaboration, advanced AI, mobile apps
- **Cost:** $200-500/month
- **Team:** Hire a junior dev to help ($20-40/hour remote)
- **Timeline:** 3-6 months to full feature set

---

## Conclusion

**For your situation (solo business, .NET expertise, multi-division future), the hybrid .NET + React + Azure stack is the optimal choice.** It balances cost, development speed, scalability, and your personal productivity.

The $1,884 three-year cost is negligible compared to:

- Commercial software savings: $2,700-9,000+
- Your time value: Priceless (you ship 2-3x faster in .NET)
- Business control: Own your data, customize infinitely
- Division expansion: Reuse for GSC-PROD, GSC-AI, GSC-DEV

**Ready to build?** Let's start with the Azure setup and project scaffolding.

---

**Document Version:** 1.0  
**Author:** AI Analysis for Jack Gibson  
**Next Review:** After MVP completion (Month 3)
