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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the complete customer management feature. This involves creating the database model and migrations, building the backend API for full CRUD operations (GET, POST, PUT, DELETE), and developing the frontend UI with a customer list, search/filter capabilities, and forms for creating/editing customers. Ensure robust validation is implemented on both the client and server. Follow the technical notes for specific implementation details.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the job management feature. Your task is to create the database model and migrations for jobs, build the backend API for full CRUD operations, and develop the frontend UI to list, create, edit, and delete jobs. The UI should include search and filtering capabilities. Ensure jobs are correctly associated with customers. Follow the technical notes for specific implementation details.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the cost tracking feature for jobs. You will need to create the database model and migrations for expenses, build the backend API to manage these expenses per job, and develop the frontend UI to view, add, edit, and delete expenses for a specific job. The system should automatically calculate and display the total cost and profit margin for each job.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement CSV import/export functionality for financial data. Create backend endpoints to handle the export of expenses and job data, and an endpoint to import expenses from a CSV file. On the frontend, build the UI for downloading exports and uploading import files, including a preview and error reporting. Follow the technical notes for library and implementation guidance.

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

**GitHub Agent Prompt:**
As a senior frontend developer, build the basic responsive UI foundation using the shadcn/ui component library. Your task is to set up the main application layout, implement responsive navigation for desktop and mobile, create reusable page layouts, and configure routing for the main pages (Dashboard, Customers, Jobs). Implement a consistent theme and add loading states and toast notifications for user feedback.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement comprehensive form validation. Define Zod schemas for all data models (Customer, Job, Expense) and use them with React Hook Form on the frontend to provide inline validation. On the backend, implement validation for all API endpoints to ensure data integrity, returning appropriate error codes and messages for failures.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the communication logging feature. Create the necessary database schema and backend API to manage communication logs associated with customers and jobs. On the frontend, build the UI to display communication history for a customer and a form to add new log entries.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the photo and document upload system. This involves configuring a cloud storage solution, creating the database schema for file metadata, building backend API endpoints for secure file uploads and retrieval, and developing a frontend component for file uploads with a gallery view for images.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the profitability analysis and reporting feature. Your task is to build a reporting dashboard that displays key financial metrics like total revenue, costs, and profit. Create charts for revenue trends and cost breakdowns. The system should allow filtering by date ranges and provide options to export reports.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, implement the parts inventory management system. Create the database schema and backend API for managing inventory items, including stock levels and supplier information. Develop the frontend UI to view, add, edit, and search inventory. The system should automatically update stock levels when parts are used in jobs and provide low-stock alerts.

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

**GitHub Agent Prompt:**
As a senior backend architect, refactor the application to support a multi-tenant architecture. This involves adding a `tenant_id` to all relevant database tables, implementing middleware to enforce tenant isolation on all API requests, and ensuring all database queries are filtered by the current tenant. Create a tenant management UI for administrators.

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

**GitHub Agent Prompt:**
As a senior security engineer, implement a Role-Based Access Control (RBAC) system. Define roles and permissions, associate users with roles, and implement backend middleware to enforce authorization on API endpoints. On the frontend, conditionally render UI elements based on the current user's permissions. Integrate with Auth0 for role management.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, build a consolidated dashboard for GSC LLC management. This dashboard should display key metrics aggregated from all divisions, with charts for comparing division performance and filtering capabilities. Access must be restricted to users with the GSC LLC Admin role.

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

**GitHub Agent Prompt:**
As a senior frontend developer, optimize the application for mobile devices. Implement Progressive Web App (PWA) features, including a service worker for offline caching of critical data. Optimize the UI for touch interactions, ensure all layouts are responsive, and test thoroughly on various mobile screen sizes.

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

**GitHub Agent Prompt:**
As a senior backend developer, integrate the Wave API for financial automation. Implement the OAuth flow for authentication, create functions to generate invoices and estimates in Wave from job data, and set up a mechanism (webhooks or polling) to sync payment status back into the application. Handle API errors and rate limiting gracefully.

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

**GitHub Agent Prompt:**
As a senior AI/ML engineer, implement AI-powered features. Use an LLM like GPT to create an AI-assisted job estimation tool based on historical data. Develop a parts recommendation engine using embeddings and a vector database. Create a feature to generate templates for customer communications.

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

**GitHub Agent Prompt:**
As a senior full-stack developer, build a secure, self-service customer portal. Implement customer registration and login, a dashboard for customers to view their job history and status, and the ability to view and download invoices. Integrate a payment gateway to allow online payments.

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

**GitHub Agent Prompt:**
As a DevOps engineer, provision and configure the hosting environment for the application using Azure App Service (or a suitable alternative like Fly.io). Set up separate staging and production environments, configure custom domains, SSL, and all necessary environment variables. Implement monitoring, logging, and health checks.

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

**GitHub Agent Prompt:**
As a security engineer, configure Auth0 for user authentication. Create an Auth0 tenant and application, configure OAuth 2.0, and integrate the Auth0 SDK into the frontend. Implement token validation on the backend and set up roles and permissions to be used for RBAC.

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

**GitHub Agent Prompt:**
As a database administrator, provision and configure the production database. Choose a suitable managed database service (e.g., Azure Database for PostgreSQL), configure connection strings, users, and permissions, and set up automated backups, point-in-time recovery, and monitoring.

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

**GitHub Agent Prompt:**
As a DevOps engineer, create the containerization setup for the project. Write a `Dockerfile` for both the frontend and backend applications, using multi-stage builds to optimize for production. Create a `docker-compose.yml` file to orchestrate the local development environment, including services for the database and any other dependencies.

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

**GitHub Agent Prompt:**
As a DevOps engineer, implement the full CI/CD pipeline using GitHub Actions. Create workflows to automatically build, lint, and test the application on every pull request. Configure the pipeline to build and push Docker images to a container registry and then deploy the application to staging and production environments based on the branching strategy.

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

**GitHub Agent Prompt:**
As a project manager, configure the GitHub repository for open source collaboration. Create all necessary community health files, including a comprehensive `README.md`, `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, and issue/PR templates. Set up branch protection rules and a project board for issue tracking.

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

**GitHub Agent Prompt:**
As a cloud architect, research and evaluate alternative hosting platforms. Create a detailed comparison matrix of at least three platforms (e.g., Fly.io, Railway, Render), analyzing them based on cost, features, scalability, and ease of use. Provide a final recommendation based on the project's requirements.

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

**GitHub Agent Prompt:**
As a DevOps engineer, set up and configure a cloud storage solution for file uploads. Provision the service (e.g., Azure Blob Storage), configure storage containers, set up access policies and CORS, and document the integration process. Implement a mechanism for secure file access, such as presigned URLs.

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

## Issue 26: Manual Repository Configuration

**Title:** `[INFRA] Perform Manual Repository Configuration`

**Labels:** `infrastructure`, `setup`

**Description:**

### Description

Perform the manual configuration steps required for the repository as outlined in `MANUAL_CONFIGURATION_GUIDE.md`. These settings cannot be automated via code and must be configured in the GitHub repository settings.

**GitHub Agent Prompt:**
As the repository administrator, perform the manual configuration tasks outlined in `MANUAL_CONFIGURATION_GUIDE.md`. This includes setting up branch protection rules for the `main` branch, enabling and configuring GitHub Discussions, updating repository settings, and enabling all recommended security features like code scanning and secret scanning.

### Objectives

- Configure branch protection rules
- Enable and configure GitHub Discussions
- Configure repository settings and metadata
- Enable security features
- Create community labels and project board

### Acceptance Criteria

- [ ] Branch protection rules configured for `main` (Require PR, Require status checks, Require linear history)
- [ ] GitHub Discussions enabled and categories created (Announcements, General, Ideas, Q&A, Show and tell)
- [ ] Repository settings configured (Description, Topics, Default branch)
- [ ] Security features enabled (Code scanning, Secret scanning, Dependabot)
- [ ] Community labels created (good first issue, help wanted, etc.)
- [ ] Project board created ("GSC Tracking Development")

### Technical Notes

- Refer to `MANUAL_CONFIGURATION_GUIDE.md` for detailed steps for each item.
- Ensure you have Admin permissions on the repository to perform these actions.

### Priority

High

---

## Issue 27: Complete Fly.io Deployment Setup

**Title:** `[INFRA] Complete Fly.io Deployment Setup`

**Labels:** `infrastructure`, `devops`, `deployment`

**Description:**

### Description

Complete the initial setup for Fly.io deployment as outlined in `docs/DEPLOYMENT-SETUP-CHECKLIST.md`. This involves setting up the Fly.io account, CLI, and creating the initial applications.

**GitHub Agent Prompt:**
As a DevOps engineer, complete the initial Fly.io deployment setup. This requires you to create a Fly.io account, install the CLI, generate a deploy token, and add it as a `FLY_API_TOKEN` secret to the GitHub repository. You must then create the production and staging applications on Fly.io and configure all necessary secrets.

### Objectives

- Set up Fly.io account and CLI
- Create production and staging applications
- Configure secrets for deployment

### Acceptance Criteria

- [ ] Fly.io account created and credit card added (if required)
- [ ] Fly.io CLI installed locally and authenticated
- [ ] Deploy token generated (`fly tokens create deploy`)
- [ ] `FLY_API_TOKEN` added to GitHub Secrets
- [ ] Production app (`gsc-tracking-api`) created
- [ ] Staging app (`gsc-tracking-api-staging`) created
- [ ] Secrets configured for both apps (ConnectionStrings, Auth0 settings, etc.)
- [ ] Verify deployment pipeline runs successfully

### Technical Notes

- Refer to `docs/DEPLOYMENT-SETUP-CHECKLIST.md` for the detailed checklist.
- Ensure unique app names are used if `gsc-tracking-api` is taken.

### Priority

High

---

## Issue 28: Complete Netlify Frontend Deployment Setup

**Title:** `[INFRA] Complete Netlify Frontend Deployment Setup`

**Labels:** `infrastructure`, `devops`, `deployment`, `frontend`

**Description:**

### Description

Complete the initial setup for Netlify frontend deployment as outlined in `docs/NETLIFY-SETUP-CHECKLIST.md`. This ensures the frontend is automatically deployed on push.

**GitHub Agent Prompt:**
As a DevOps engineer, complete the initial Netlify frontend deployment setup. Connect the GitHub repository to a new Netlify site, configure the build settings (base directory, build command, publish directory), and set up all necessary environment variables. Ensure deploy previews are enabled for pull requests.

### Objectives

- Set up Netlify account and site
- Configure build settings and environment variables
- Verify deployment

### Acceptance Criteria

- [ ] Netlify account created and GitHub repository connected
- [ ] Build settings configured:
    - Base directory: `frontend`
    - Build command: `npm run build`
    - Publish directory: `frontend/dist`
- [ ] Environment variables configured (`VITE_API_URL`, etc.)
- [ ] Site name updated to `gsc-tracking` (or similar)
- [ ] Site deployed successfully
- [ ] Deploy previews enabled for Pull Requests

### Technical Notes

- Refer to `docs/NETLIFY-SETUP-CHECKLIST.md` for the detailed checklist.
- Ensure `netlify.toml` is present in the root (it is).

### Priority

High

---

## Issue 29: Database Setup Next Steps

**Title:** `[INFRA] Complete Database Setup Next Steps`

**Labels:** `infrastructure`, `database`

**Description:**

### Description

Complete the remaining database setup tasks as outlined in `docs/DATABASE-SETUP.md`. This includes monitoring, backup procedures, and production planning.

**GitHub Agent Prompt:**
As a database administrator, complete the remaining database setup tasks. This includes setting up monitoring and alerts for the database, documenting the backup and restore procedures, and finalizing the production database strategy (e.g., choosing between Azure SQL and Neon).

### Objectives

- Set up database monitoring
- Document backup procedures
- Plan production database strategy

### Acceptance Criteria

- [ ] Database monitoring and alerts configured (e.g., connection failures, performance)
- [ ] Backup and restore procedures documented in `docs/`
- [ ] Production database strategy finalized (Azure SQL vs PostgreSQL on Fly.io/Neon)
- [ ] Connection strings for production and staging verified and secured

### Technical Notes

- Refer to `docs/DATABASE-SETUP.md` for context.
- Consider using Neon for serverless PostgreSQL if Azure SQL is too expensive for MVP.

### Priority

Medium

---

## Issue 30: Implement Unit and Integration Tests Infrastructure

**Title:** `[INFRA] Implement Unit and Integration Tests Infrastructure`

**Labels:** `infrastructure`, `testing`, `backend`, `frontend`

**Description:**

### Description

Set up the testing infrastructure for both backend and frontend to enable CI testing and ensure code quality.

**GitHub Agent Prompt:**
As a quality assurance engineer, set up the testing infrastructure. For the backend, create the test project and add xUnit, FluentAssertions, and Moq. For the frontend, verify the Vitest configuration. Add a basic unit test to both projects and update the CI pipeline to run these tests on every pull request.

### Objectives

- Create backend test project
- Configure frontend testing
- Integrate tests into CI pipeline

### Acceptance Criteria

- [ ] Backend test project (`GscTracking.Api.Tests`) created and referenced in solution
- [ ] xUnit, FluentAssertions, and Moq installed for backend
- [ ] Basic unit test added for backend (e.g., testing a simple service or controller)
- [ ] Frontend test setup verified (Vitest/Jest)
- [ ] Basic unit test added for frontend
- [ ] CI workflow (`dotnet.yml` / `node.js.yml`) updated to run tests on PRs
- [ ] Verify tests pass in CI environment

### Technical Notes

- Refer to `docs/CICD-VALIDATION.md` for testing strategy.
- Ensure code coverage reporting is configured if possible.

### Priority

Medium

---

## Issue 31: Implement Pre-Auth0 CORS Configuration with Pattern Matching

**Title:** `[INFRA] Implement pre-Auth0 CORS configuration with pattern matching`

**Labels:** `infrastructure`, `backend`, `deployment`

**Description:**

### Description

Implement CORS configuration in the backend API to support Netlify deploy previews before Auth0 is implemented. Use pattern-based origin validation instead of wildcards to allow all deploy preview URLs dynamically.

**GitHub Agent Prompt:**
As a backend developer, implement a flexible CORS policy in the .NET API. The policy must use pattern matching to dynamically allow any Netlify deploy preview URL (`https://deploy-preview-*.netlify.app`), as well as the production, staging, and configurable localhost origins. Do not use a wildcard origin. Follow the technical details for the implementation.

### Objectives

- Configure CORS to allow production, staging, and all deploy preview URLs
- Use regex pattern matching for deploy preview validation
- Support configurable localhost ports via app settings
- Ensure security with specific pattern matching (no broad wildcards)

### Acceptance Criteria

- [ ] CORS policy implemented in `backend/GscTracking.Api/Program.cs`
- [ ] Pattern matching validates deploy preview URLs: `https://deploy-preview-\d+--gsc-tracking-ui\.netlify\.app`
- [ ] Production URL allowed: `https://gsc-tracking-ui.netlify.app`
- [ ] Staging URL allowed: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] Localhost ports configurable via comma-separated app setting (e.g., `"5173,5174,3000"`)
- [ ] CORS policy allows credentials, any method, any header
- [ ] Tested with deploy preview URLs from actual PRs
- [ ] Documentation updated in README or deployment docs

### Technical Details

**Configuration approach:**

```csharp
// In appsettings.json or appsettings.Development.json
{
  "AllowedLocalPorts": "5173,5174,3000"  // Comma-separated list of allowed localhost ports
}

// In Program.cs
using System.Text.RegularExpressions;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedLocalPorts = builder.Configuration["AllowedLocalPorts"]?.Split(',') ?? new[] { "5173" };
        
        policy.SetIsOriginAllowed(origin =>
        {
            // Allow configured localhost ports
            foreach (var port in allowedLocalPorts)
            {
                if (origin == $"http://localhost:{port.Trim()}")
                    return true;
            }
            
            // Allow production
            if (origin == "https://gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow staging
            if (origin == "https://staging--gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow Netlify deploy previews with pattern matching
            var netlifyPreviewPattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";
            if (Regex.IsMatch(origin, netlifyPreviewPattern))
                return true;
            
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Apply CORS
app.UseCors("AllowFrontend");
```

### Priority

High

---

## Issue 32: Create Staging Branch for Auth0 Testing

**Title:** `[INFRA] Create dedicated staging branch for Auth0 testing`

**Labels:** `infrastructure`, `deployment`, `authentication`

**Description:**

### Description

Create a dedicated `staging` branch that deploys to a persistent staging URL (`https://staging--gsc-tracking-ui.netlify.app`) for Auth0 testing. This avoids wildcard issues with Auth0 configuration.

**GitHub Agent Prompt:**
As a DevOps engineer, create and configure a dedicated `staging` branch. Create the branch from `main`, push it to GitHub, and configure the Netlify site to automatically deploy this branch to the `staging` deploy context. Document the workflow for keeping the `staging` branch updated from `main`.

### Objectives

- Create `staging` branch from `main`
- Configure Netlify to deploy `staging` branch
- Document staging workflow for team
- Keep staging in sync with main for testing

### Acceptance Criteria

- [ ] `staging` branch created and pushed to GitHub
- [ ] Netlify configured to deploy `staging` branch
- [ ] Staging deploys to: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] Backend staging points to: `https://gsc-tracking-api-staging.fly.dev`
- [ ] Staging workflow documented for team
- [ ] Process for keeping staging updated from main

### Technical Notes

**Create Staging Branch:**

```bash
# From main branch
git checkout main
git pull origin main

# Create staging branch
git checkout -b staging
git push origin staging
```

**Configure Netlify:**

1. Go to Netlify dashboard: `https://app.netlify.com/sites/gsc-tracking-ui`
2. Navigate to: **Site settings** → **Build & deploy** → **Continuous deployment**
3. Under **Deploy contexts** → **Branch deploys**
4. Select: **"Let me add individual branches"**
5. Add branch: `staging`
6. Save changes

### Priority

Medium (Do before Auth0 implementation)

---

## Issue 33: Implement Post-Auth0 CORS and Auth0 Configuration

**Title:** `[INFRA] Implement post-Auth0 CORS and Auth0 configuration with dedicated staging`

**Labels:** `infrastructure`, `backend`, `authentication`, `deployment`

**Description:**

### Description

Implement Auth0 authentication configuration and update CORS policy to work with Auth0. Create dedicated staging environment for Auth0 testing (since wildcards don't work reliably with Auth0).

**GitHub Agent Prompt:**
As a security engineer, implement the full Auth0 authentication setup. Configure the Auth0 application with the specific callback, logout, and web origin URLs for local, staging, and production environments (no wildcards). Configure the necessary environment variables in both the backend and frontend deployment contexts. Ensure the login/logout flow is fully testable on the dedicated `staging` environment.

### Objectives

- Configure Auth0 application settings with specific allowed URLs (no wildcards)
- Update CORS configuration to work with Auth0 requirements
- Create dedicated staging branch deployment for Auth0 testing
- Maintain pattern-based CORS for deploy previews (non-auth testing)
- Document Auth0 testing workflow for team

### Acceptance Criteria

- [ ] Auth0 application configured with allowed callback URLs
- [ ] Auth0 application configured with allowed web origins
- [ ] Auth0 application configured with allowed logout URLs
- [ ] Dedicated staging branch created: `staging`
- [ ] Staging branch deploys to: `https://staging--gsc-tracking-ui.netlify.app`
- [ ] CORS configuration updated for Auth0 compatibility
- [ ] Auth0 environment variables configured in backend
- [ ] Auth0 environment variables configured in Netlify (production and staging only)
- [ ] Login/logout tested on production
- [ ] Login/logout tested on staging
- [ ] Deploy previews documented as non-auth testing only
- [ ] Team documentation updated with Auth0 workflow

### Technical Details

**Auth0 Application Configuration:**

In Auth0 Dashboard → Applications → GSC Tracking → Settings:

1. **Allowed Callback URLs:**
   ```
   http://localhost:5173/callback,
   https://gsc-tracking-ui.netlify.app/callback,
   https://staging--gsc-tracking-ui.netlify.app/callback
   ```

2. **Allowed Logout URLs:**
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

3. **Allowed Web Origins:**
   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

**Note:** Do NOT use wildcards like `https://deploy-preview-*--gsc-tracking-ui.netlify.app` as they may not work reliably.

### Priority

High

---

## Issue 34: Activate Staging Database on Neon

**Title:** `[INFRA] Activate Staging Database on Neon`

**Labels:** `infrastructure`, `database`, `staging`

**Description:**

### Description

Activate the staging database on Neon.tech as per `docs/DATABASE-SETUP-SUMMARY.md`. This is the final step to make the staging environment fully functional.

**GitHub Agent Prompt:**
As a DevOps engineer, activate the staging database on Neon. Create a new project on Neon, obtain the connection string, and configure it as the `DATABASE_URL` secret in the `gsc-tracking-api-staging` application on Fly.io. Finally, apply the Entity Framework migrations to the new database to bring the schema up to date.

### Objectives

- Provision staging database on Neon
- Connect staging API to Neon database
- Apply migrations

### Acceptance Criteria

- [ ] Neon account created (free tier)
- [ ] Staging project `gsc-tracking-staging` created in US East (Ohio)
- [ ] Connection string obtained
- [ ] Fly.io staging app (`gsc-tracking-api-staging`) configured with `DATABASE_URL` secret
- [ ] Migrations applied to staging database (`dotnet ef database update`)
- [ ] Deployment verified (API connects to DB)

### Technical Notes

- Refer to `docs/NEON-QUICKSTART.md` for detailed steps.
- Use the connection string format: `postgresql://user:pass@host/db?sslmode=require`

### Priority

High

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
   - Start with infrastructure issues (#18-#34) as they're foundational
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
   - "Infrastructure Setup" (issues #18-#34)

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
**Total Issues:** 34 (6 MVP + 11 Roadmap + 17 Infrastructure)
