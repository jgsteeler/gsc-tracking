# Authentication Guide

This document provides an overview of the authentication setup for the GSC Tracking application, including Auth0 configuration and mock authentication for local development.

---

## Auth0 Setup

The GSC Tracking application uses Auth0 for authentication and authorization. Auth0 provides secure login, role-based access control (RBAC), and token-based authentication for the backend API and frontend application.

To configure Auth0, follow the detailed instructions in the [Auth0 Setup Guide](./AUTH0-SETUP.md).

### Key Steps

1. Set up an Auth0 tenant and application.
2. Configure environment variables for the backend and frontend.
3. Create the required roles for RBAC:
   - `tracking-admin`
   - `tracking-read`
   - `tracking-write`
4. Assign roles to users as needed.
5. Test the login, logout, and role-based access flows.

For more details, see the [Auth0 Setup Guide](./AUTH0-SETUP.md).

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

- Mock authentication does not support RBAC testing. All mock users will default to the `tracking-admin` role.
- Developers must test role-based access control (RBAC) with proper Auth0 configuration before submitting a merge request.

> **⚠️ Important:** All new features must be tested locally with proper RBAC and authorization configured. Merge requests will not be accepted unless RBAC functionality has been verified.

### Next Steps

A new feature will be implemented to automatically enable mock authentication when the `MOCK_AUTH` or `VITE_MOCK_AUTH` variable is set. This will allow developers to use mock authentication without modifying the codebase.

---

## Other OAuth Providers

If you prefer to use another OAuth provider, you are welcome to submit a merge request (MR) with the implementation for your preferred provider. Alternatively, you can open an issue request to suggest support for your provider.

> **Note:** Auth0 is the only officially supported authentication provider at this time. Support for additional providers may be considered in the future.
