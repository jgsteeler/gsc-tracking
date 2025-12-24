# Authentication Guide

This document provides an overview of the authentication setup for the GSC Tracking application, including Auth0 configuration and mock authentication for local development.

---

## Auth0 Setup

The GSC Tracking application uses Auth0 for authentication and authorization. Auth0 provides secure login, role-based access control (RBAC), and token-based authentication for the backend API and frontend application.

To configure Auth0, follow the detailed instructions in the [Auth0 Setup Guide](./AUTH0-SETUP.md).

### Key Steps

1. Set up an Auth0 tenant and application.
2. Configure environment variables for the backend and frontend.
3. Configure Auth0 permissions (or roles) for role-based access control (RBAC).
4. Assign appropriate permissions to users based on their access needs.
5. Test the login, logout, and role-based access flows.

For more details, see the [Auth0 Setup Guide](./AUTH0-SETUP.md).

---

## Role-Based Access Control (RBAC)

The GSC Tracking application implements three access levels using Auth0 permissions or roles.

### Access Levels

The backend recognizes both **permissions** (recommended) and **roles** (alternative):

**Using Permissions (Recommended):**
- Auth0 Permission: `admin` → Internal Role: `tracker-admin`
- Auth0 Permission: `write` → Internal Role: `tracker-write`
- Auth0 Permission: `read` → Internal Role: `tracker-read`

**Using Roles (Alternative):**
- Auth0 Role: `tracker-admin` → Internal Role: `tracker-admin`
- Auth0 Role: `tracker-write` → Internal Role: `tracker-write`
- Auth0 Role: `tracker-read` → Internal Role: `tracker-read`

### Role Definitions

1. **`tracker-admin`** (or permission: `admin`) - Full administrative access
   - Can create, read, update, and delete all resources (customers, jobs, expenses, job updates)
   - Can import data via CSV
   - Can export data to CSV
   - Required for creating and managing customers and jobs

2. **`tracker-write`** (or permission: `write`) - Write access for field operations
   - Can read all data (customers, jobs, expenses, job updates)
   - Can create and update expenses
   - Can create job updates
   - Can export data to CSV
   - Cannot create/modify/delete customers or jobs
   - Cannot delete expenses or job updates

3. **`tracker-read`** (or permission: `read`) - Read-only access
   - Can view all data (customers, jobs, expenses, job updates)
   - Can export data to CSV
   - Cannot create, update, or delete any resources

### Endpoint Authorization

| Endpoint Type | Admin | Write | Read |
|--------------|-------|-------|------|
| GET (view data) | ✅ | ✅ | ✅ |
| POST/PUT Customer | ✅ | ❌ | ❌ |
| POST/PUT Job | ✅ | ❌ | ❌ |
| POST/PUT Expense | ✅ | ✅ | ❌ |
| POST Job Update | ✅ | ✅ | ❌ |
| DELETE Customer | ✅ | ❌ | ❌ |
| DELETE Job | ✅ | ❌ | ❌ |
| DELETE Expense | ✅ | ❌ | ❌ |
| DELETE Job Update | ✅ | ❌ | ❌ |
| Import CSV | ✅ | ❌ | ❌ |
| Export CSV | ✅ | ✅ | ✅ |

---

## Mock Authentication for Local Development

To simplify local development, you can enable mock authentication by setting environment variables. This allows developers to bypass the Auth0 login flow and use a predefined mock user.

### Enabling Mock Authentication

1. Add the following environment variables:

   - For the backend, add to your `.env` file:

     ```env
     MOCK_AUTH=true
     ```

   - For the frontend, add to your `.env.local` file:

     ```env
     VITE_MOCK_AUTH=true
     ```

2. Ensure these variables are only set in local development environments. They should never be included in staging or production configurations.

### Limitations of Mock Authentication

- Mock authentication does not support RBAC testing. All mock users will default to the `tracker-admin` role.
- Developers must test role-based access control (RBAC) with proper Auth0 configuration before submitting a merge request.

> **⚠️ Important:** All new features must be tested locally with proper RBAC and authorization configured. Merge requests will not be accepted unless RBAC functionality has been verified.

### Next Steps

A new feature will be implemented to automatically enable mock authentication when the `MOCK_AUTH` or `VITE_MOCK_AUTH` variable is set. This will allow developers to use mock authentication without modifying the codebase.

---

## Other OAuth Providers

If you prefer to use another OAuth provider, you are welcome to submit a merge request (MR) with the implementation for your preferred provider. Alternatively, you can open an issue request to suggest support for your provider.

> **Note:** Auth0 is the only officially supported authentication provider at this time. Support for additional providers may be considered in the future.
