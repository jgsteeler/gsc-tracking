# GitHub Issues for GSC Small Engine App

This document contains all the GitHub issues that need to be created for the GSC Small Engine Repair Shop business management application. Issues are organized by category: MVP Features, Roadmap Features, and Infrastructure & Setup.

For full context, refer to [business-management-app-analysis.md](./business-management-app-analysis.md).

---

## MVP Features

### Issue 1: Customer Management (CRUD Operations)

**Title:** [MVP] Implement Customer Management with CRUD Operations

**Labels:** `mvp`, `enhancement`, `backend`, `frontend`

**Description:**

Implement complete customer management functionality including Create, Read, Update, and Delete operations for customer records.

**Acceptance Criteria:**
- [ ] Database schema/model for customers with fields: name, email, phone, address, notes
- [ ] Backend API endpoints for customer CRUD operations
- [ ] Frontend UI for listing all customers with search/filter
- [ ] Frontend form for creating new customers with validation
- [ ] Frontend form for editing existing customers
- [ ] Delete customer functionality with confirmation dialog
- [ ] Form validation on both frontend (Zod) and backend
- [ ] Error handling and user feedback for all operations
- [ ] Responsive design using shadcn/ui components

**Technical Notes:**
- Use Entity Framework Core for database models and migrations
- Implement REST API endpoints: GET /api/customers, POST /api/customers, PUT /api/customers/:id, DELETE /api/customers/:id
- Use React Hook Form with Zod validation (frontend)
- Use FluentValidation or Data Annotations (backend .NET validation)
- Use shadcn/ui components: Table, Dialog, Form, Input, Button
- Consider soft delete vs hard delete

**Priority:** Critical (blocking)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#core-entities)

---

### Issue 2: Job Management (CRUD Operations)

**Title:** [MVP] Implement Job Management with CRUD Operations

**Labels:** `mvp`, `enhancement`, `backend`, `frontend`

**Description:**

Implement complete job management functionality including Create, Read, Update, and Delete operations for job records. Jobs represent repair/service work for customers.

**Acceptance Criteria:**
- [ ] Database schema/model for jobs with fields: customer_id, equipment_type, equipment_model, description, status, date_received, date_completed, estimate_amount, actual_amount
- [ ] Job status enum: Quote, In Progress, Completed, Invoiced, Paid
- [ ] Backend API endpoints for job CRUD operations
- [ ] Frontend UI for listing all jobs with search/filter by customer, status, date
- [ ] Frontend form for creating new jobs with customer selection
- [ ] Frontend form for editing job details and updating status
- [ ] Delete job functionality with confirmation dialog
- [ ] Association with customer records (foreign key)
- [ ] Timeline tracking (created, updated timestamps)
- [ ] Form validation on both frontend and backend
- [ ] Responsive design using shadcn/ui components

**Technical Notes:**
- Use Entity Framework Core for database models with proper relations (navigation properties)
- Implement REST API endpoints: GET /api/jobs, POST /api/jobs, PUT /api/jobs/:id, DELETE /api/jobs/:id, GET /api/jobs/customer/:customerId
- Use React Hook Form with Zod validation (frontend)
- Use FluentValidation or Data Annotations (backend .NET validation)
- Use shadcn/ui components: Table, Dialog, Form, Input, Select, Badge for status
- Status field should use C# enum

**Priority:** Critical (blocking)

**Dependencies:** Depends on Customer Management (#1)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#core-entities)

---

### Issue 3: Cost Tracking Per Job

**Title:** [MVP] Implement Cost Tracking System for Jobs

**Labels:** `mvp`, `enhancement`, `backend`, `frontend`

**Description:**

Implement expense and cost tracking functionality to record all costs associated with each job (parts, labor, external services).

**Acceptance Criteria:**
- [ ] Database schema/model for expenses with fields: job_id, type (parts/labor/service), description, amount, date, receipt_reference
- [ ] Backend API endpoints for expense CRUD operations
- [ ] Frontend UI for viewing all expenses for a specific job
- [ ] Frontend form for adding expenses to a job
- [ ] Ability to edit and delete expenses
- [ ] Calculate total cost per job from all expenses
- [ ] Display profit margin (estimate/invoice amount - total costs)
- [ ] Form validation on both frontend and backend
- [ ] Responsive design using shadcn/ui components

**Technical Notes:**
- Use Entity Framework Core for database models with job relation (navigation properties)
- Implement REST API endpoints: GET /api/jobs/:jobId/expenses, POST /api/jobs/:jobId/expenses, PUT /api/expenses/:id, DELETE /api/expenses/:id
- Calculate totals on backend and cache/store for performance
- Use shadcn/ui components: Table, Dialog, Form, Input, Select
- Consider expense categories/types as C# enum

**Priority:** High

**Dependencies:** Depends on Job Management (#2)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#expenses)

---

### Issue 4: CSV Import/Export for Financial Data

**Title:** [MVP] Implement CSV Import/Export for Expenses, Estimates, Invoices, and Payment Status

**Labels:** `mvp`, `enhancement`, `backend`, `frontend`

**Description:**

Implement CSV import and export functionality for expenses, estimates, invoices, and payment status to enable data migration and reporting.

**Acceptance Criteria:**
- [ ] Backend endpoint to export expenses to CSV format
- [ ] Backend endpoint to export jobs with estimate/invoice data to CSV format
- [ ] Backend endpoint to import expenses from CSV file
- [ ] CSV format validation and error handling
- [ ] Frontend UI button/link to download CSV exports
- [ ] Frontend file upload component for CSV import
- [ ] Import preview before confirming
- [ ] Error reporting for invalid CSV data
- [ ] Support for common CSV formats (comma-separated, tab-separated)
- [ ] Documentation of CSV format/schema

**Technical Notes:**
- Use papaparse for frontend CSV parsing
- Use CsvHelper library for .NET backend CSV operations
- Implement endpoints: GET /api/export/expenses, GET /api/export/jobs, POST /api/import/expenses
- Include proper headers in CSV files
- Validate data types and required fields during import (FluentValidation)
- Consider batch processing for large files
- Use shadcn/ui components: Button, FileUpload, Table for preview

**Priority:** Medium

**Dependencies:** Depends on Cost Tracking (#3) and Job Management (#2)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#key-features)

---

### Issue 5: Basic UI with shadcn/ui Components

**Title:** [MVP] Build Basic Responsive UI with shadcn/ui Component Library

**Labels:** `mvp`, `enhancement`, `frontend`, `ui/ux`

**Description:**

Set up and implement the basic UI infrastructure using shadcn/ui components, including navigation, layout, and common UI patterns.

**Acceptance Criteria:**
- [ ] Install and configure shadcn/ui with Tailwind CSS
- [ ] Create main application layout with navigation
- [ ] Implement responsive navigation menu (desktop + mobile)
- [ ] Create reusable page layout components
- [ ] Set up routing between main pages (Dashboard, Customers, Jobs)
- [ ] Implement consistent styling and theme
- [ ] Add loading states and skeletons
- [ ] Add toast notifications for user feedback
- [ ] Ensure mobile responsiveness for all layouts
- [ ] Add dark mode support (optional for MVP)

**Technical Notes:**
- Use shadcn/ui CLI to add components as needed
- Components to install: Button, Input, Table, Dialog, Form, Select, Tabs, Card, Badge, Toast, Skeleton, DropdownMenu
- Use React Router or Next.js routing
- Implement layout component with sidebar/header
- Use Tailwind CSS for custom styling
- Follow shadcn/ui theming guidelines

**Priority:** High

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#frontend)

---

### Issue 6: Form Validation (Frontend and Backend)

**Title:** [MVP] Implement Comprehensive Form Validation with Zod

**Labels:** `mvp`, `enhancement`, `backend`, `frontend`

**Description:**

Implement robust form validation on both frontend and backend using Zod schemas to ensure data integrity and provide good user experience.

**Acceptance Criteria:**
- [ ] Define Zod schemas for all data models (Customer, Job, Expense)
- [ ] Implement frontend validation using React Hook Form + Zod
- [ ] Implement backend validation using Zod on all API endpoints
- [ ] Display validation errors inline on forms
- [ ] Prevent form submission when validation fails
- [ ] Show clear error messages for validation failures
- [ ] Validate data types, required fields, formats (email, phone)
- [ ] Add custom validation rules where needed
- [ ] Return proper HTTP status codes for validation errors (400)
- [ ] Document validation rules in API documentation

**Technical Notes:**
- Frontend: Use Zod schemas with React Hook Form and @hookform/resolvers/zod
- Backend: Use FluentValidation or Data Annotations for .NET API validation
- Consider sharing validation rules via TypeScript types generated from C# models (using tools like NSwag or OpenAPI Generator)
- Common validations: email format, phone format, required fields, min/max lengths, number ranges
- Use shadcn/ui Form components which integrate with React Hook Form
- Implement validation middleware in .NET API pipeline
- Consider creating custom validators for reusable validation logic

**Priority:** High

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#key-features)

---

## Roadmap Features

### Issue 7: Communication Logs

**Title:** [ROADMAP] Implement Communication Logs for Customer Interactions

**Labels:** `roadmap`, `enhancement`, `backend`, `frontend`

**Description:**

Add functionality to log and track all customer communications including phone calls, emails, text messages, and in-person interactions.

**Business Value:**
- Maintain complete communication history with customers
- Improve customer service by having context in all interactions
- Track follow-ups and service reminders
- Enable better team collaboration and handoffs

**Acceptance Criteria:**
- [ ] Database schema for communication logs with fields: customer_id, job_id (optional), type, date, subject, notes, created_by
- [ ] Backend API endpoints for communication CRUD operations
- [ ] Frontend UI to view communication history per customer
- [ ] Frontend form to add new communication log entries
- [ ] Filter and search communication logs
- [ ] Associate communications with specific jobs when relevant
- [ ] Track which user created each log entry
- [ ] Email integration to automatically log sent/received emails (optional)

**Technical Notes:**
- Communication types: phone, email, sms, in-person, other
- Consider integrations with email providers (Gmail, Outlook)
- Consider SMS integration with Twilio or similar
- Use timeline/activity feed UI pattern

**Priority:** High

**Phase:** Phase 2 (3-6 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#communication-management)

---

### Issue 8: Photo and Document Uploads

**Title:** [ROADMAP] Implement Photo and Document Upload System

**Labels:** `roadmap`, `enhancement`, `backend`, `frontend`, `storage`

**Description:**

Enable users to upload photos (before/after repair photos) and documents (receipts, manuals) associated with jobs and customers.

**Business Value:**
- Visual documentation of work performed
- Better customer communication with before/after photos
- Store important documents like receipts and warranties
- Improve record-keeping and dispute resolution

**Acceptance Criteria:**
- [ ] Configure cloud storage (Azure Blob, Tigris, or Upstash)
- [ ] Database schema for file metadata with fields: job_id/customer_id, file_type, file_name, file_url, uploaded_by, upload_date
- [ ] Backend API endpoint for file upload with validation
- [ ] Backend API endpoint to retrieve files
- [ ] Frontend file upload component with drag-and-drop
- [ ] Display uploaded images in gallery view
- [ ] Display uploaded documents as downloadable links
- [ ] Image preview and thumbnail generation
- [ ] File size and type restrictions
- [ ] Delete uploaded files functionality

**Technical Notes:**
- Use Azure Blob Storage, Tigris, or Upstash for file storage
- Implement multipart/form-data upload
- Generate thumbnails for images using sharp or similar
- Supported formats: JPG, PNG, PDF, DOC, XLSX
- File size limit: 10MB per file
- Use presigned URLs for secure access
- Consider image optimization for web

**Priority:** Medium

**Phase:** Phase 2 (3-6 months)

**Dependencies:** Requires storage infrastructure setup

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#document-management)

---

### Issue 9: Profitability Analysis and Reporting

**Title:** [ROADMAP] Implement Profitability Analysis and Financial Reporting

**Labels:** `roadmap`, `enhancement`, `backend`, `frontend`, `analytics`

**Description:**

Build comprehensive profitability analysis and reporting features to provide insights into job profitability, revenue trends, and business performance.

**Business Value:**
- Understand which types of jobs are most profitable
- Identify cost overruns and pricing issues
- Make data-driven business decisions
- Track revenue trends over time
- Compare performance across divisions

**Acceptance Criteria:**
- [ ] Calculate and display profit margin per job (revenue - costs)
- [ ] Dashboard with key metrics: total revenue, total costs, total profit, number of jobs
- [ ] Revenue trend chart over time (daily, weekly, monthly)
- [ ] Cost breakdown by category (parts, labor, services)
- [ ] Top profitable job types/categories
- [ ] Top customers by revenue
- [ ] Export reports to PDF or Excel
- [ ] Date range filters for all reports
- [ ] Division comparison for multi-tenant setup

**Technical Notes:**
- Use charting library: Recharts or Chart.js
- Pre-calculate metrics for performance (cron jobs or triggers)
- Implement caching for expensive queries
- Consider using data warehouse or analytics database for large datasets
- Use SQL aggregate functions efficiently
- Create materialized views for common reports

**Priority:** Medium

**Phase:** Phase 2 (3-6 months)

**Dependencies:** Requires cost tracking and job management

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#analytics--reporting)

---

### Issue 10: Inventory Management System

**Title:** [ROADMAP] Implement Parts Inventory Management

**Labels:** `roadmap`, `enhancement`, `backend`, `frontend`

**Description:**

Add inventory management functionality to track parts, stock levels, suppliers, and reorder needs.

**Business Value:**
- Know what parts are in stock before starting jobs
- Avoid ordering duplicate parts
- Get alerts when stock is low
- Track part costs and suppliers
- Improve job turnaround time

**Acceptance Criteria:**
- [ ] Database schema for inventory items with fields: part_number, name, description, quantity, unit_cost, reorder_level, supplier, location
- [ ] Backend API endpoints for inventory CRUD operations
- [ ] Frontend UI for viewing all inventory items
- [ ] Frontend forms for adding/editing inventory items
- [ ] Search and filter inventory by part number, name, supplier
- [ ] Track inventory usage per job (link expenses to inventory items)
- [ ] Automatic stock level updates when parts are used
- [ ] Low stock alerts/notifications
- [ ] Inventory value reporting
- [ ] Supplier management

**Technical Notes:**
- Implement inventory transactions for audit trail
- Use database triggers or application logic to maintain stock levels
- Consider barcode scanning integration
- Support for multiple locations/storage areas
- Implement stock adjustment functionality
- Add inventory reports: stock value, usage frequency, reorder list

**Priority:** Medium

**Phase:** Phase 2 (3-6 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#inventory-management)

---

### Issue 11: Multi-Tenant Architecture for Divisions

**Title:** [ROADMAP] Implement Multi-Tenant Architecture for Multiple Business Divisions

**Labels:** `roadmap`, `enhancement`, `backend`, `architecture`

**Description:**

Implement multi-tenant architecture to support multiple business divisions under GSC LLC with proper data isolation and tenant context.

**Business Value:**
- Support multiple divisions under GSC LLC umbrella
- Ensure data isolation between divisions
- Share common infrastructure and codebase
- Enable consolidated reporting across divisions
- Allow division-specific branding and configuration

**Acceptance Criteria:**
- [ ] Add tenant_id field to all relevant database tables
- [ ] Implement tenant context middleware for all API requests
- [ ] Ensure all database queries include tenant filter
- [ ] Create tenant management UI for admins
- [ ] Allow users to switch between divisions (if they have access)
- [ ] Implement division-specific branding (logo, colors)
- [ ] Create consolidated dashboard for GSC LLC management
- [ ] Cross-division reporting capabilities
- [ ] Tenant isolation in file storage
- [ ] Migration strategy for existing data

**Technical Notes:**
- Use tenant_id or organization_id column in all tables
- Implement Row Level Security (RLS) if using PostgreSQL
- Use middleware to inject tenant context into all queries
- Consider using separate schemas per tenant for strict isolation
- Implement tenant resolver from subdomain or path
- Store tenant configuration in separate table
- Add tenant validation to prevent cross-tenant access
- Create tenant seeding scripts for testing

**Priority:** High

**Phase:** Phase 2 (3-6 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#multi-tenant-design)

---

### Issue 12: Role-Based Access Control (RBAC)

**Title:** [ROADMAP] Implement Role-Based Access Control System

**Labels:** `roadmap`, `enhancement`, `backend`, `frontend`, `security`

**Description:**

Implement comprehensive role-based access control with defined roles (Admin, Manager, Technician, Customer) and granular permissions.

**Business Value:**
- Ensure users only access data they're authorized to see
- Protect sensitive financial information
- Enable customer self-service without security risks
- Support proper separation of duties
- Audit and track user actions

**Acceptance Criteria:**
- [ ] Define roles: Admin, Manager, Technician, Customer, GSC LLC Admin
- [ ] Define permissions for each role
- [ ] Database schema for roles and permissions
- [ ] Associate users with roles (many-to-many)
- [ ] Backend middleware for authorization checks
- [ ] Frontend UI shows/hides features based on permissions
- [ ] Restrict API endpoints based on user role
- [ ] Audit logging for sensitive operations
- [ ] Role management UI for admins
- [ ] Test coverage for authorization logic

**Technical Notes:**
- Integrate with Auth0 for user roles/permissions
- Use Auth0 Authorization extension or Actions
- Implement permission decorators/middleware for API endpoints
- Permission examples: read:customers, write:customers, read:financials, write:invoices, admin:all
- Frontend: conditionally render UI elements based on permissions
- Backend: verify permissions in middleware before controller logic
- Consider CASL or similar library for permission management
- Store user permissions in JWT or fetch from Auth0

**Priority:** High

**Phase:** Phase 2 (3-6 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#role-based-access-control)

---

### Issue 13: GSC LLC Consolidation Dashboard

**Title:** [ROADMAP] Build Consolidated Dashboard for GSC LLC Management

**Labels:** `roadmap`, `enhancement`, `frontend`, `analytics`

**Description:**

Create a high-level dashboard for GSC LLC management to view consolidated data across all divisions.

**Business Value:**
- Single view of all division performance
- Compare division metrics side-by-side
- Identify trends across the organization
- Make strategic decisions based on consolidated data
- Monitor overall business health

**Acceptance Criteria:**
- [ ] Dashboard showing key metrics across all divisions
- [ ] Division comparison charts (revenue, profit, jobs)
- [ ] Filter by date range
- [ ] Drill-down capability to view division details
- [ ] Export consolidated reports
- [ ] Top customers across all divisions
- [ ] Revenue trends for all divisions
- [ ] Cost analysis across divisions
- [ ] Access restricted to GSC LLC Admin role

**Technical Notes:**
- Query data across all tenants with proper authorization
- Use efficient aggregation queries
- Consider caching for performance
- Use data visualization library: Recharts or Chart.js
- Implement lazy loading for division details
- Create separate API endpoints for consolidated data
- Ensure proper tenant context handling

**Priority:** Medium

**Phase:** Phase 2 (3-6 months)

**Dependencies:** Requires multi-tenant architecture (#11) and RBAC (#12)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#advanced-multi-tenancy)

---

### Issue 14: Mobile-Responsive Optimizations

**Title:** [ROADMAP] Optimize Application for Mobile Devices

**Labels:** `roadmap`, `enhancement`, `frontend`, `ui/ux`, `mobile`

**Description:**

Enhance mobile experience with Progressive Web App capabilities, offline support, and mobile-optimized UI/UX.

**Business Value:**
- Enable field technicians to use the app on tablets/phones
- Access information while on job sites
- Work offline when internet is unavailable
- Improve user experience on mobile devices
- Increase adoption among mobile-first users

**Acceptance Criteria:**
- [ ] Implement Progressive Web App (PWA) capabilities
- [ ] Add service worker for offline support
- [ ] Optimize UI for touch interactions
- [ ] Implement mobile-specific navigation patterns
- [ ] Add app manifest for "Add to Home Screen"
- [ ] Cache critical data for offline access
- [ ] Optimize images and assets for mobile
- [ ] Test on various mobile devices and screen sizes
- [ ] Implement pull-to-refresh pattern
- [ ] Add mobile-optimized forms with appropriate input types

**Technical Notes:**
- Use Workbox for service worker generation
- Implement PWA with Next.js or Vite PWA plugin
- Use responsive design breakpoints: 640px, 768px, 1024px, 1280px
- Optimize bundle size for faster mobile loading
- Use lazy loading for images and components
- Implement IndexedDB for offline data storage
- Test with Chrome DevTools mobile emulation
- Consider touch-friendly button sizes (min 44x44px)
- Use mobile-appropriate input types: tel, email, date

**Priority:** Medium

**Phase:** Phase 2 (3-6 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#mobile-optimization)

---

### Issue 15: Wave API Integration

**Title:** [ROADMAP] Integrate Wave API for Invoicing and Payment Tracking

**Labels:** `roadmap`, `enhancement`, `backend`, `integration`

**Description:**

Integrate with Wave API to automatically create estimates, invoices, and sync payment information.

**Business Value:**
- Automate invoice creation from completed jobs
- Reduce manual data entry
- Sync payment status automatically
- Maintain single source of truth for financial data
- Leverage Wave's accounting features

**Acceptance Criteria:**
- [ ] Set up Wave API credentials and authentication
- [ ] Implement OAuth flow for Wave API
- [ ] Create invoices in Wave from completed jobs
- [ ] Sync invoice status (sent, paid) from Wave
- [ ] Create estimates in Wave
- [ ] Sync payment information
- [ ] Map GSC customers to Wave customers
- [ ] Handle API errors gracefully
- [ ] Add webhook handler for Wave events
- [ ] UI to manually trigger sync or view sync status

**Technical Notes:**
- Use Wave GraphQL API
- Store Wave customer IDs in database
- Implement background job queue for sync operations
- Handle rate limiting and retries
- Use webhooks for real-time updates when available
- Map job data to Wave invoice format
- Consider sync frequency and performance
- Add logging for all API interactions
- Implement sync conflict resolution

**Priority:** Low

**Phase:** Phase 3 (6-12 months)

**Dependencies:** Requires Wave account and API access

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#wave-api-integration)

---

### Issue 16: AI-Powered Features

**Title:** [ROADMAP] Implement AI-Powered Features for Job Estimation and Customer Communication

**Labels:** `roadmap`, `enhancement`, `backend`, `ai/ml`

**Description:**

Leverage AI/ML capabilities to provide intelligent job estimation, parts lookup recommendations, and customer communication assistance.

**Business Value:**
- Faster and more accurate job estimates
- Reduce time spent on quotes
- Suggest relevant parts based on job description
- Generate professional customer communications
- Learn from historical data to improve accuracy

**Acceptance Criteria:**
- [ ] AI-assisted job estimation based on historical data
- [ ] Parts recommendation based on equipment type and job description
- [ ] Customer communication template generation
- [ ] Sentiment analysis for customer communications (optional)
- [ ] Predictive maintenance scheduling suggestions
- [ ] Cost prediction for jobs based on similar past jobs
- [ ] UI integration for AI suggestions
- [ ] User feedback mechanism to improve AI accuracy
- [ ] Privacy and data usage compliance

**Technical Notes:**
- Use OpenAI API or similar for natural language processing
- Train models on historical job data
- Implement embeddings for similarity search
- Use vector database for part recommendations: Pinecone, Weaviate
- Consider using Langchain for LLM orchestration
- Implement prompt engineering for consistent results
- Add user feedback loop to improve recommendations
- Ensure proper data anonymization for AI training
- Consider costs and implement usage limits

**Priority:** Low

**Phase:** Phase 3 (6-12 months)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#ai-powered-features)

---

### Issue 17: Customer Portal

**Title:** [ROADMAP] Build Customer Self-Service Portal

**Labels:** `roadmap`, `enhancement`, `frontend`, `backend`

**Description:**

Create a customer-facing portal where customers can view their jobs, invoices, make payments, and communicate with the shop.

**Business Value:**
- Reduce inbound calls about job status
- Enable customers to self-serve for common needs
- Improve customer satisfaction and transparency
- Streamline payment collection
- Provide 24/7 access to information

**Acceptance Criteria:**
- [ ] Customer login/registration flow
- [ ] Dashboard showing customer's jobs and status
- [ ] View job details and timeline
- [ ] View and download invoices
- [ ] Online payment processing integration
- [ ] Message/communication with shop
- [ ] Email notifications for job status updates
- [ ] Mobile-responsive design
- [ ] Password reset functionality
- [ ] Customer profile management

**Technical Notes:**
- Create separate customer-facing route/subdomain
- Use Auth0 for customer authentication
- Implement different UI theme for customer portal
- Integrate payment gateway: Stripe, Square, or Wave
- Send email notifications using SendGrid or similar
- Filter data to only show customer's own records
- Implement public/private key for shareable job links
- Consider SMS notifications for important updates
- Add terms of service and privacy policy

**Priority:** Medium

**Phase:** Phase 3 (6-12 months)

**Dependencies:** Requires RBAC (#12)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#customer-portal)

---

## Infrastructure & Setup

### Issue 18: Azure App Service Setup

**Title:** [INFRA] Set Up Azure App Service for Application Hosting

**Labels:** `infrastructure`, `devops`, `hosting`

**Description:**

Provision and configure Azure App Service to host the GSC Small Engine business management application.

**Objectives:**
- Deploy application to production-ready environment
- Ensure scalability and reliability
- Implement proper security configurations
- Set up monitoring and logging

**Acceptance Criteria:**
- [ ] Provision Azure App Service (or alternatives: Fly.io, Railway)
- [ ] Configure custom domain and SSL certificate
- [ ] Set up staging and production environments
- [ ] Configure environment variables and app settings
- [ ] Enable application insights for monitoring
- [ ] Configure auto-scaling rules
- [ ] Set up health checks
- [ ] Configure CORS policies
- [ ] Enable compression and caching
- [ ] Document deployment process

**Technical Details:**
- App Service Plan: Choose appropriate tier (B1 for dev, P1V2+ for production)
- Runtime: Node.js 18+ or 20+
- Operating System: Linux preferred for cost
- Region: Choose based on user location
- Consider alternatives: Fly.io (lower cost), Railway (simpler setup), Render
- Enable Always On for production
- Configure deployment slots for blue-green deployments

**Category:** Hosting/Deployment

**Priority:** Critical (blocking)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#hosting--infrastructure)

---

### Issue 19: Auth0 Authentication Setup

**Title:** [INFRA] Configure Auth0 for User Authentication and Authorization

**Labels:** `infrastructure`, `devops`, `security`

**Description:**

Set up Auth0 for secure user authentication, SSO, MFA, and authorization for the application.

**Objectives:**
- Secure user authentication
- Enable single sign-on (SSO)
- Support multi-factor authentication (MFA)
- Implement role-based authorization
- Provide good user experience for login/logout

**Acceptance Criteria:**
- [ ] Create Auth0 tenant and application
- [ ] Configure OAuth 2.0 / OpenID Connect
- [ ] Set up Auth0 SDK in frontend application
- [ ] Configure callback URLs and allowed origins
- [ ] Enable social login providers (Google, Microsoft - optional)
- [ ] Enable MFA for admin users
- [ ] Configure Auth0 Rules or Actions for custom logic
- [ ] Set up roles and permissions in Auth0
- [ ] Implement token validation in backend
- [ ] Configure token expiration and refresh
- [ ] Add login/logout UI flows
- [ ] Test authentication flow end-to-end

**Technical Details:**
- Use Auth0 React SDK or Universal Login
- Implement JWT validation in backend middleware
- Store tokens securely (httpOnly cookies or secure storage)
- Configure proper CORS settings
- Set up Auth0 custom domain (optional)
- Use Auth0 Actions for custom authentication logic
- Configure token lifetime: access token (1hr), refresh token (7 days)
- Implement proper error handling for auth failures

**Category:** Authentication/Security

**Priority:** Critical (blocking)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#authentication)

---

### Issue 20: Database Setup

**Title:** [INFRA] Provision and Configure Production Database

**Labels:** `infrastructure`, `devops`, `database`

**Description:**

Set up production database (Azure SQL, PostgreSQL, or other) with proper configuration, security, and backup strategy.

**Objectives:**
- Provide reliable data persistence
- Ensure data security and privacy
- Enable disaster recovery
- Optimize for performance
- Support development and staging environments

**Acceptance Criteria:**
- [ ] Provision database (Azure SQL, Azure PostgreSQL, or alternative)
- [ ] Configure database connection strings
- [ ] Set up database users and permissions
- [ ] Enable SSL/TLS connections
- [ ] Configure automated backups
- [ ] Set up point-in-time recovery
- [ ] Configure firewall rules
- [ ] Enable query performance insights
- [ ] Set up database monitoring and alerts
- [ ] Document connection and administration procedures
- [ ] Create separate databases for dev, staging, production

**Technical Details:**
- Recommended: PostgreSQL for cost and flexibility
- Azure Database for PostgreSQL (Flexible Server)
- Alternatives: Supabase, Neon, Railway PostgreSQL
- Configure pg_bouncer for connection pooling
- Enable SSL mode: require or verify-full
- Set up backup retention: 7-30 days
- Configure maintenance window
- Monitor database size and performance metrics
- Use managed database service for automatic updates

**Category:** Database

**Priority:** Critical (blocking)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#database)

---

### Issue 21: Containerization with Docker

**Title:** [INFRA] Set Up Docker and Docker Compose for Development and Deployment

**Labels:** `infrastructure`, `devops`, `containerization`

**Description:**

Create Docker configuration for containerizing the application and Docker Compose setup for local development environment.

**Objectives:**
- Ensure consistent development environment
- Simplify deployment process
- Enable easy local setup for contributors
- Support microservices architecture if needed
- Facilitate CI/CD pipeline

**Acceptance Criteria:**
- [ ] Create Dockerfile for frontend application
- [ ] Create Dockerfile for backend application
- [ ] Create docker-compose.yml for local development
- [ ] Include database container in docker-compose
- [ ] Add volume mounts for development hot-reload
- [ ] Configure environment variables properly
- [ ] Add health checks to containers
- [ ] Optimize Docker images for size and security
- [ ] Create .dockerignore file
- [ ] Document Docker setup in README
- [ ] Test multi-stage builds for production
- [ ] Set up container registry (Azure Container Registry or Docker Hub)

**Technical Details:**
- Use multi-stage builds to reduce image size
- Base images: node:20-alpine for Node.js
- Use docker-compose version 3.8+
- Include services: frontend, backend, database, redis (if needed)
- Configure networks for service isolation
- Use environment-specific docker-compose files
- Implement build caching for faster builds
- Security: run as non-root user, scan for vulnerabilities

**Category:** Containerization

**Priority:** High

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#hosting--infrastructure)

---

### Issue 22: CI/CD Pipeline with GitHub Actions

**Title:** [INFRA] Implement CI/CD Pipeline using GitHub Actions

**Labels:** `infrastructure`, `devops`, `ci/cd`

**Description:**

Set up automated CI/CD pipeline using GitHub Actions following GitLab Flow for building, testing, and deploying the application.

**Objectives:**
- Automate build and test processes
- Enable continuous deployment to environments
- Implement code quality checks
- Ensure consistent and reliable releases
- Reduce manual deployment effort

**Acceptance Criteria:**
- [ ] Create GitHub Actions workflow for CI (build, lint, test)
- [ ] Create GitHub Actions workflow for CD (deploy)
- [ ] Run tests on every pull request
- [ ] Run linting and code quality checks
- [ ] Build Docker images in CI pipeline
- [ ] Push images to container registry
- [ ] Deploy to staging environment on main branch
- [ ] Deploy to production on release tags
- [ ] Implement GitLab Flow branching strategy
- [ ] Add status badges to README
- [ ] Configure secrets and environment variables
- [ ] Add deployment notifications (Slack, email)

**Technical Details:**
- Workflow triggers: push, pull_request, release
- Use GitHub Actions cache for dependencies
- Run parallel jobs for faster builds
- Implement matrix builds for multiple environments
- Use GitHub Secrets for sensitive data
- Implement blue-green or rolling deployments
- Add smoke tests after deployment
- Use GitHub Environments for deployment protection
- Consider using act for local workflow testing
- Implement rollback procedure

**Category:** CI/CD Pipeline

**Priority:** High

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#hosting--infrastructure)

---

### Issue 23: Open Source Repository Setup

**Title:** [INFRA] Configure Repository for Open Source Collaboration

**Labels:** `infrastructure`, `documentation`, `community`

**Description:**

Set up the repository with proper documentation, templates, and guidelines to enable open source collaboration.

**Objectives:**
- Enable community contributions
- Provide clear project documentation
- Establish contribution guidelines
- Create welcoming environment for contributors
- Ensure legal compliance with licenses

**Acceptance Criteria:**
- [ ] Write comprehensive README.md
- [ ] Add LICENSE file (MIT, Apache 2.0, or other)
- [ ] Create CONTRIBUTING.md with contribution guidelines
- [ ] Add CODE_OF_CONDUCT.md
- [ ] Create issue templates for bugs and features
- [ ] Create pull request template
- [ ] Set up GitHub Discussions or similar
- [ ] Add project documentation (architecture, setup)
- [ ] Create getting started guide for developers
- [ ] Add badges for build status, license, version
- [ ] Set up project board for issue tracking
- [ ] Configure branch protection rules
- [ ] Add CHANGELOG.md for release notes

**Technical Details:**
- Use GitHub issue templates (YAML format)
- Reference: GitHub's community standards
- Include setup instructions with prerequisites
- Document environment variables and configuration
- Add architecture diagrams (optional)
- Link to business-management-app-analysis.md
- Use conventional commit messages
- Add semantic versioning guidelines
- Consider adding Contributor License Agreement (CLA)

**Category:** Other

**Priority:** Medium

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#open-source-considerations)

---

### Issue 24: Hosting Alternatives Evaluation

**Title:** [INFRA] Evaluate and Document Alternative Hosting Options

**Labels:** `infrastructure`, `devops`, `research`

**Description:**

Research and document alternative hosting platforms to Azure App Service, including Fly.io, Railway, Render, and others.

**Objectives:**
- Identify cost-effective hosting alternatives
- Compare features and limitations
- Provide deployment flexibility
- Document pros and cons of each option
- Enable informed decision-making

**Acceptance Criteria:**
- [ ] Research Fly.io capabilities and pricing
- [ ] Research Railway capabilities and pricing
- [ ] Research Render capabilities and pricing
- [ ] Research other alternatives (Vercel, Netlify, DigitalOcean)
- [ ] Compare database hosting options
- [ ] Compare cost projections for each platform
- [ ] Document deployment process for each
- [ ] Evaluate performance and reliability
- [ ] Consider geographic distribution and latency
- [ ] Create comparison matrix document
- [ ] Make recommendation based on requirements

**Technical Details:**
- Consider factors: cost, scalability, ease of use, support, geographic regions
- Fly.io: Good for global distribution, Docker-based
- Railway: Simple setup, good for startups, built-in PostgreSQL
- Render: Similar to Heroku, free tier available
- Evaluate free tier limitations
- Consider egress costs and database costs separately
- Test deployment on 2-3 platforms
- Document required configurations for each
- Consider hybrid approach (frontend on Vercel, backend on Fly.io)

**Category:** Hosting/Deployment

**Priority:** Low

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#hosting--infrastructure)

---

### Issue 25: Storage Options Configuration

**Title:** [INFRA] Set Up Cloud Storage for Files and Assets

**Labels:** `infrastructure`, `devops`, `storage`

**Description:**

Configure cloud storage solution (Azure Blob, Tigris, Upstash, or other) for storing uploaded files, photos, and documents.

**Objectives:**
- Provide reliable file storage
- Enable scalable storage solution
- Ensure secure file access
- Optimize costs
- Support file operations (upload, download, delete)

**Acceptance Criteria:**
- [ ] Provision storage service (Azure Blob, Tigris, Upstash, or Supabase)
- [ ] Configure storage buckets/containers
- [ ] Set up access policies and permissions
- [ ] Enable CORS for web access
- [ ] Configure CDN for faster delivery (optional)
- [ ] Implement presigned URLs for secure access
- [ ] Set up lifecycle policies for old files
- [ ] Configure backup and redundancy
- [ ] Document storage integration
- [ ] Test upload/download functionality
- [ ] Implement storage usage monitoring

**Technical Details:**
- Azure Blob Storage: Mature, integrated with Azure ecosystem
- Tigris: S3-compatible, globally distributed, competitive pricing
- Upstash: Redis-based, good for small files
- Supabase Storage: Good integration if using Supabase
- Use S3 SDK or Azure SDK for integration
- Implement multipart upload for large files
- Generate thumbnails for images before storing
- Consider storage tiers: hot, cool, archive
- Set retention policies and auto-delete old files
- Monitor storage costs and usage

**Category:** Storage

**Priority:** Medium

**Dependencies:** Required for photo/document upload feature (#8)

**Reference:** [business-management-app-analysis.md](./business-management-app-analysis.md#storage)

---

## Labels Configuration

Create the following labels in the GitHub repository:

### Priority Labels
- `priority: critical` - Critical issues that block progress (color: #d73a4a)
- `priority: high` - High priority issues (color: #ff9800)
- `priority: medium` - Medium priority issues (color: #ffc107)
- `priority: low` - Low priority issues (color: #8bc34a)

### Category Labels
- `mvp` - Minimum Viable Product features (color: #0052cc)
- `roadmap` - Post-MVP roadmap features (color: #5319e7)
- `infrastructure` - Infrastructure and DevOps (color: #1d76db)
- `enhancement` - New features or improvements (color: #a2eeef)
- `bug` - Something isn't working (color: #d73a4a)
- `documentation` - Documentation improvements (color: #0075ca)
- `security` - Security-related issues (color: #ee0701)

### Technical Labels
- `backend` - Backend/API work (color: #c5def5)
- `frontend` - Frontend/UI work (color: #bfdadc)
- `database` - Database related (color: #d4c5f9)
- `devops` - DevOps and deployment (color: #fbca04)
- `testing` - Testing related (color: #7057ff)
- `ui/ux` - User interface and experience (color: #e99695)
- `api` - API design or changes (color: #0e8a16)
- `integration` - Third-party integrations (color: #f9d0c4)
- `mobile` - Mobile-specific (color: #d876e3)
- `analytics` - Analytics and reporting (color: #0075ca)
- `ai/ml` - AI/Machine learning (color: #006b75)

### Status Labels
- `status: blocked` - Blocked by another issue (color: #d73a4a)
- `status: in-progress` - Currently being worked on (color: #0e8a16)
- `status: needs-review` - Needs review (color: #fbca04)
- `status: needs-testing` - Needs testing (color: #ff9800)
- `good first issue` - Good for newcomers (color: #7057ff)
- `help wanted` - Extra attention needed (color: #008672)

---

## Issue Creation Instructions

To create these issues in the GitHub repository:

1. **Using GitHub Web Interface:**
   - Navigate to https://github.com/jgsteeler/gsc-tracking/issues
   - Click "New issue"
   - Select the appropriate template (MVP Feature, Roadmap Feature, or Infrastructure)
   - Fill in the details from each issue above
   - Add the appropriate labels
   - Click "Submit new issue"

2. **Using GitHub CLI (`gh`):**
   ```bash
   # Install GitHub CLI if not already installed
   # Create an issue using CLI
   gh issue create --title "Title" --body "Description" --label "label1,label2"
   ```

3. **Using GitHub API or Scripts:**
   - Consider creating a script to bulk-create issues from this document
   - Use GitHub REST API or GraphQL API

4. **Order of Creation:**
   - Start with infrastructure issues (#18-#25) as they're foundational
   - Then create MVP issues (#1-#6) as they're the highest priority
   - Finally create roadmap issues (#7-#17) for future planning

5. **Issue Numbering:**
   - The numbers in this document (Issue 1, Issue 2, etc.) are for reference
   - GitHub will assign actual issue numbers when created
   - Update cross-references after creation if needed

6. **Creating Milestones:**
   Consider creating milestones for better organization:
   - "MVP Release" (issues #1-#6)
   - "Phase 2 - Enhanced Features" (issues #7-#14)
   - "Phase 3 - Advanced Features" (issues #15-#17)
   - "Infrastructure Setup" (issues #18-#25)

---

## Next Steps

After creating the issues:

1. ✅ Add all labels to the repository
2. ✅ Create milestones for each phase
3. ✅ Assign issues to milestones
4. ✅ Create a project board for tracking progress
5. ✅ Prioritize and sequence the issues
6. ✅ Start with infrastructure setup issues
7. ✅ Begin MVP development
8. ✅ Set up regular review and planning sessions

---

**Document Version:** 1.0  
**Last Updated:** 2025-12-08  
**Total Issues:** 25 (6 MVP + 11 Roadmap + 8 Infrastructure)
