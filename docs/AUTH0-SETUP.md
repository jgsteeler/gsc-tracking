# Auth0 Authentication Setup Guide

This guide walks you through setting up Auth0 authentication for the GSC Tracking application.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Auth0 Tenant Setup](#auth0-tenant-setup)
3. [Auth0 Application Configuration](#auth0-application-configuration)
4. [Backend Configuration](#backend-configuration)
5. [Frontend Configuration](#frontend-configuration)
6. [Testing Authentication](#testing-authentication)
7. [Protecting API Endpoints](#protecting-api-endpoints)
8. [Troubleshooting](#troubleshooting)

## Prerequisites

- Auth0 account (free tier is sufficient for development)
- GSC Tracking backend deployed or running locally
- GSC Tracking frontend deployed or running locally

## Auth0 Tenant Setup

1. **Create or Access Auth0 Account**
   - Go to [https://auth0.com](https://auth0.com)
   - Sign up for a free account or log in to existing account
   - Navigate to your Auth0 Dashboard: [https://manage.auth0.com](https://manage.auth0.com)

2. **Create a New Tenant (if needed)**
   - Click on your profile picture in the top right
   - Select "Create tenant"
   - Choose a tenant name (e.g., `gsc-tracking-dev` or `gsc-tracking-prod`)
   - Select your region (closest to your users)
   - Click "Create"

## Auth0 Application Configuration

### Create API

1. **Navigate to APIs**
   - In the Auth0 Dashboard, go to **Applications** → **APIs**
   - Click **Create API**

2. **Configure API Settings**
   - **Name**: `GSC Tracking API`
   - **Identifier**: `https://api.gsc-tracking.com` (or your domain)
     - This is your **Audience** value
     - Must be a valid URL format (but doesn't need to resolve)
   - **Signing Algorithm**: RS256 (default)
   - Click **Create**

3. **API Permissions (Optional)**
   - Navigate to the **Permissions** tab
   - Add custom scopes if needed (e.g., `read:customers`, `write:jobs`)
   - For MVP, you can use the default scopes

### Create Application

1. **Navigate to Applications**
   - In the Auth0 Dashboard, go to **Applications** → **Applications**
   - Click **Create Application**

2. **Configure Application**
   - **Name**: `GSC Tracking UI`
   - **Application Type**: Single Page Application
   - Click **Create**

3. **Configure Application Settings**
   - Navigate to the **Settings** tab
   - Note your **Domain** and **Client ID** (you'll need these later)
      - Domain: your-tenant.auth0.com
      - Client ID: your-client-id-here
4. **Configure Allowed URLs**

   **Allowed Callback URLs** (comma-separated):

   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

   **Allowed Logout URLs** (comma-separated):

   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

   **Allowed Web Origins** (comma-separated):

   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

   **Allowed Origins (CORS)** (comma-separated):

   ```
   http://localhost:5173,
   https://gsc-tracking-ui.netlify.app,
   https://staging--gsc-tracking-ui.netlify.app
   ```

5. **Save Changes**
   - Scroll to the bottom and click **Save Changes**

### Configure Roles and Permissions

**IMPORTANT:** The GSC Tracking application requires the `tracker-admin` role to be configured in Auth0 for role-based access control (RBAC).

1. **Create Roles**
   - Go to **User Management** → **Roles**
   - Click **Create Role**
   - Create the following role:
     - **Name**: `tracker-admin`
     - **Description**: Full administrative access for GSC Tracking application

2. **Assign Permissions to Roles (Optional)**
   - Select a role
   - Navigate to **Permissions** tab
   - Click **Add Permissions**
   - Select your API and assign relevant permissions
   - For MVP, the application only checks for the `tracker-admin` role, not specific permissions

### Configuring Role Claims

**CRITICAL STEP:** Auth0 must be configured to include user roles in the JWT access token. By default, Auth0 does not add roles to tokens automatically.

1. **Create an Action to Add Roles to Token**
   
   Navigate to **Actions** → **Library** in your Auth0 Dashboard, then:
   
   a. Click **Build Custom**
   
   b. Name your action: `Add Roles to Access Token`
   
   c. Set the trigger: **Login / Post Login**
   
   d. Add the following code:
   
   ```javascript
   /**
    * Handler that will be called during the execution of a PostLogin flow.
    *
    * @param {Event} event - Details about the user and the context in which they are logging in.
    * @param {PostLoginAPI} api - Interface whose methods can be used to change the behavior of the login.
    */
   exports.onExecutePostLogin = async (event, api) => {
     const namespace = 'https://gsc-tracking.com';
     
     if (event.authorization) {
       // Add roles to the access token
       api.accessToken.setCustomClaim(`${namespace}/roles`, event.authorization.roles);
       
       // Optionally add roles to the ID token as well
       api.idToken.setCustomClaim(`${namespace}/roles`, event.authorization.roles);
     }
   };
   ```
   
   e. Click **Deploy**

2. **Add the Action to Your Login Flow**
   
   Navigate to **Actions** → **Flows** → **Login**:
   
   a. Drag and drop your **Add Roles to Access Token** action from the right sidebar to the flow
   
   b. Place it after the **Start** node
   
   c. Click **Apply**

3. **Verify Role Claims**
   
   After configuration, the JWT access token will contain a custom claim like:
   
   ```json
   {
     "https://gsc-tracking.com/roles": ["tracker-admin"]
   }
   ```
   
   The backend API is configured to automatically map this custom claim to the standard .NET role claim for authorization.

> **Note:** The namespace `https://gsc-tracking.com` is used to avoid claim collision with standard JWT claims. This is a best practice recommended by Auth0. The backend automatically handles this namespace and maps it to standard .NET role claims.

4. **Alternative Namespace Configuration**
   
   If you prefer to use a different namespace, you can update:
   - The Auth0 Action code above
   - The backend configuration in `Program.cs` (look for `possibleRoleClaims` array)
   - The frontend configuration in `useUserRole.ts` (look for `possibleNamespaces` array)

## Backend Configuration

**IMPORTANT: Auth0 configuration is mandatory.** The application will not start without proper Auth0 configuration. This is a security measure to prevent the application from running in an insecure state.

The backend is configured to read Auth0 settings primarily from environment variables. This is the most secure method and is recommended for all environments.

### How It Works

In `Program.cs`, the application attempts to load configuration in this order:

1. **Environment Variables**: `AUTH0_DOMAIN` and `AUTH0_AUDIENCE`. This is the primary method for production and staging.
2. **`.env` File**: For local development, the `DotNetEnv` NuGet package is used to load environment variables from a `.env` file automatically.
3. **`appsettings.json`**: `Auth0:Domain` and `Auth0:Audience`. This is a fallback, useful for shared development settings.

**If both `AUTH0_DOMAIN` and `AUTH0_AUDIENCE` are not set (either via environment variables or `.env`), the application will throw an `InvalidOperationException` and fail to start.**

### 1. Local Development (`.env` file)

Create a file named `.env` in the `backend/GscTracking.Api/` directory. This file is in the `.gitignore` and will not be committed.

```bash
# backend/GscTracking.Api/.env

# Auth0 Configuration
AUTH0_DOMAIN="your-dev-tenant.us.auth0.com"
AUTH0_AUDIENCE="https://gsc-tracking-api"
```

### 2. Staging and Production

For staging and production environments, the backend uses environment variables set on the hosting platform. This ensures sensitive information is securely managed and not exposed in the repository.

1. **Netlify**

   Set the following environment variables in the Netlify dashboard:

   - `AUTH0_DOMAIN`: `your-production-domain`
   - `AUTH0_AUDIENCE`: `https://gsc-tracking-api`

2. **Kubernetes**

   Use Kubernetes secrets to store and manage environment variables. For example:

   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: auth0-secrets
   type: Opaque
   data:
     AUTH0_DOMAIN: <base64-encoded-domain>
     AUTH0_AUDIENCE: <base64-encoded-audience>
   ```

   Then, mount these secrets as environment variables in your deployment:

   ```yaml
   env:
     - name: AUTH0_DOMAIN
       valueFrom:
         secretKeyRef:
           name: auth0-secrets
           key: AUTH0_DOMAIN
     - name: AUTH0_AUDIENCE
       valueFrom:
         secretKeyRef:
           name: auth0-secrets
           key: AUTH0_AUDIENCE
   ```

3. **Fly.io**

   Use `flyctl secrets` to securely set environment variables:

   ```bash
   flyctl secrets set AUTH0_DOMAIN="your-production-domain"
   flyctl secrets set AUTH0_AUDIENCE="https://gsc-tracking-api"
   ```

---

## Frontend Configuration

### Environment Variables

1. **Local Development (.env.local)**

   Create `.env.local` in the frontend directory:

   ```bash
   # frontend/.env.local

   # Auth0 Configuration
   VITE_AUTH0_DOMAIN=your-tenant.auth0.com
   VITE_AUTH0_CLIENT_ID=your-client-id
   VITE_AUTH0_AUDIENCE=https://api.gsc-tracking.com

   # API URL
   VITE_API_URL=http://localhost:5091/api
   ```

2. **Staging and Production**

For staging and production environments, the frontend uses environment variables set on the hosting platform (e.g., Netlify).

1. **Netlify**

   Set the following environment variables in the Netlify dashboard:

   - `VITE_AUTH0_DOMAIN`: `your-production-domain`
   - `VITE_AUTH0_CLIENT_ID`: `your-production-client-id`
   - `VITE_AUTH0_AUDIENCE`: `https://api.gsctracking.com`
   - `VITE_API_URL`: `https://api.your-production-domain.com/api`

2. **Kubernetes**

   Use Kubernetes secrets to store and manage environment variables, similar to the backend configuration.

## Testing Authentication

### Local Testing

1. **Start Backend**

   ```bash
   cd backend/GscTracking.Api
   dotnet run
   ```

2. **Start Frontend**

   ```bash
   cd frontend
   npm run dev
   ```

3. **Test Login Flow**
   - Navigate to `http://localhost:5173`
   - If Auth0 is configured, you will be redirected to `/landing`
   - Click the "Log In to Get Started" button on the landing page
   - You should be redirected to Auth0's Universal Login page
   - Sign in or create a new account
   - You should be redirected back to the Dashboard
   - Your profile picture and name should appear in the sidebar
   - If Auth0 is not configured, you will have direct access to all pages

4. **Test Logout Flow**
   - Click the "Log Out" button
   - You should be logged out and the "Log In" button should reappear

### Testing Protected Endpoints

To test protected API endpoints, you'll need to add the `[Authorize]` attribute to a controller:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protect entire controller
public class CustomersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        // This endpoint now requires authentication
        // ...
    }
}
```

## Protecting Frontend Routes

### Landing Page and Protected Routes

The application now implements a **landing page** that is shown to unauthenticated users when Auth0 is configured. The landing page provides an overview of the application features and a prominent "Log In" button.

**Current Implementation**: The `ProtectedRoute` component (`frontend/src/components/ProtectedRoute.tsx`) is **actively used** in the application to protect all main routes (Dashboard, Customers, Jobs).

**Features**:

- Checks if the user is authenticated using Auth0
- Shows a loading spinner while authentication status is being determined
- Redirects unauthenticated users to the landing page (`/landing`)
- From the landing page, users can click "Log In" to authenticate via Auth0
- Gracefully handles cases where Auth0 is not configured (allows access for development)
- Displays appropriate loading states during redirects

**Authentication Flow**:

1. User navigates to any protected route (`/`, `/customers`, `/jobs`, etc.)
2. If Auth0 is configured and user is not authenticated:
   - User is redirected to `/landing`
   - Landing page displays application features and "Log In" button
   - User clicks "Log In" and is redirected to Auth0 Universal Login
   - After successful authentication, user is redirected back to the application
3. If Auth0 is not configured (development mode):
   - User is granted access to all routes without authentication
   - Useful for local development without Auth0 setup

**Current Route Structure**:

```tsx
import { ProtectedRoute } from './components/ProtectedRoute';

<Routes>
  {/* Public landing page */}
  <Route path="/landing" element={<Landing />} />
  
  {/* Protected routes - require authentication when Auth0 is configured */}
  <Route path="/" element={
    <ProtectedRoute>
      <MainLayout />
    </ProtectedRoute>
  }>
    <Route index element={<Dashboard />} />
    <Route path="customers" element={<Customers />} />
    <Route path="jobs" element={<Jobs />} />
    <Route path="jobs/:id" element={<JobDetails />} />
  </Route>
</Routes>
```

**Behavior Summary**:

| Scenario | Auth0 Configured | User Authenticated | Behavior |
|----------|------------------|--------------------|------------|
| Production | ✅ | ✅ | Access granted to all protected routes |
| Production | ✅ | ❌ | Redirected to landing page, must log in |
| Development | ❌ | N/A | Access granted (graceful degradation) |


## Protecting API Endpoints

### Controller-Level Protection

Protect all endpoints in a controller:

```csharp
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all actions
public class CustomersController : ControllerBase
{
    // All endpoints require authentication
}
```

### Action-Level Protection

Protect specific endpoints:

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        // Public endpoint - no authentication required
    }

    [HttpPost]
    [Authorize] // Requires authentication
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerRequestDto dto)
    {
        // Protected endpoint - authentication required
    }
}
```

### Role-Based Authorization

The GSC Tracking API uses role-based authorization to control access to modification endpoints.

**Current Roles:**
- `tracker-admin` - Full administrative access; required for all POST, PUT, and DELETE operations

**Authorization Rules:**
- All API endpoints require authentication (except `/api/hello`)
- GET endpoints require authentication only
- POST, PUT, DELETE endpoints require authentication AND the `tracker-admin` role

**Implementation:**

The API uses an authorization policy named `AdminOnly` that checks for the `tracker-admin` role:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("tracker-admin"));
});
```

Controllers are protected at the class level with `[Authorize]` for authentication, and modification endpoints use `[Authorize(Policy = "AdminOnly")]` for role-based access.

**Assigning Roles to Users:**

1. In Auth0 Dashboard, go to **User Management** → **Roles**
2. Select the `tracker-admin` role (already created)
3. Click **Users** tab
4. Click **Add Users** and search for users to assign the role

### Role-Based Authorization (Deprecated Examples)

The following examples show alternative ways to implement role-based authorization that are not currently used in this application, but are provided for reference:

Protect endpoints with specific roles:

```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "admin")] // Only admins can delete
public async Task<IActionResult> DeleteCustomer(int id)
{
    // Only users with 'admin' role can access
}
```

### Permission-Based Authorization

Protect endpoints with specific permissions:

```csharp
[HttpPost]
[Authorize(Policy = "write:customers")] // Requires specific permission
public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerRequestDto dto)
{
    // Only users with 'write:customers' permission can access
}
```

## Troubleshooting

### Common Issues

1. **Application fails to start with "Auth0 configuration is required" error**
   - This is expected behavior when Auth0 is not configured
   - Set `AUTH0_DOMAIN` and `AUTH0_AUDIENCE` environment variables
   - Or configure `Auth0:Domain` and `Auth0:Audience` in appsettings.json
   - See the Backend Configuration section for detailed instructions

2. **"Invalid state" error on callback**
   - Clear browser cookies and local storage
   - Verify the redirect URI matches exactly in Auth0 settings
   - Check that HTTPS is used in production URLs

3. **"Audience is required" error**
   - Verify `VITE_AUTH0_AUDIENCE` is set in frontend environment
   - Verify the audience matches your API identifier in Auth0

4. **401 Unauthorized on API calls**
   - Verify the access token is being sent with requests
   - Check that the API audience matches between frontend and backend
   - Verify Auth0 domain and audience are correctly set in backend

5. **CORS errors**
   - Verify your origin is listed in Auth0's Allowed Web Origins
   - Check backend CORS configuration includes your frontend URL

6. **"Login required" error on deploy previews**
   - Deploy preview URLs are not registered in Auth0 by default
   - Either register each preview URL manually (not recommended)
   - Or use the staging branch for Auth0 testing
   - Deploy previews can be used for non-authenticated testing

### Debugging Tips

1. **Check Auth0 Logs**
   - Go to **Monitoring** → **Logs** in Auth0 Dashboard
   - Look for failed login attempts or authorization errors

2. **Check Browser Console**
   - Look for Auth0-related errors in the browser console
   - Check Network tab for failed API requests

3. **Check Backend Logs**
   - Look for authentication/authorization errors
   - Verify JWT token validation is working

4. **Verify Environment Variables**
   - Frontend: Check browser console for `import.meta.env` values
   - Backend: Check startup logs for configuration warnings

## Security Best Practices

1. **Never commit secrets**
   - Use `.env.local` for local development (not tracked by git)
   - Use environment variables in production (Fly.io secrets, Netlify env vars)

2. **Use different Auth0 applications for environments**
   - Development: One client ID for localhost
   - Staging: One client ID for staging URL
   - Production: One client ID for production URL

3. **Regularly rotate secrets**
   - Rotate client secrets periodically
   - Update in all environments

4. **Enable MFA**
   - Go to **Security** → **Multi-factor Auth** in Auth0
   - Enable MFA for production users

5. **Monitor Auth0 logs**
   - Regularly check for suspicious login attempts
   - Set up alerts for security events

## Additional Resources

- [Auth0 Documentation](https://auth0.com/docs)
- [Auth0 React SDK](https://github.com/auth0/auth0-react)
- [Auth0 .NET SDK](https://github.com/auth0/auth0-aspnetcore-authentication)
- [JWT.io](https://jwt.io) - Decode and verify JWT tokens

## Support

If you encounter issues:

1. Check the [Auth0 Community](https://community.auth0.com/)
2. Review the [Auth0 Documentation](https://auth0.com/docs)
3. Contact your development team

---

**Last Updated**: 2025-12-16
