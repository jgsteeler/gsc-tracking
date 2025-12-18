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
      - Domain: <dev-axm38gs1176ibx7p.us.auth0.com>
      - Client ID: <arNDmNtUSmJm1O1V1UYOWNcKnEEnXgrI>
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

### Configure Roles and Permissions (Optional)

1. **Create Roles**
   - Go to **User Management** → **Roles**
   - Click **Create Role**
   - Create roles like:
     - `admin` - Full access to all features
     - `user` - Standard user access
     - `viewer` - Read-only access

2. **Assign Permissions to Roles**
   - Select a role
   - Navigate to **Permissions** tab
   - Click **Add Permissions**
   - Select your API and assign relevant permissions

## Backend Configuration

The backend is configured to read Auth0 settings primarily from environment variables. This is the most secure method and is recommended for all environments.

### How It Works

In `Program.cs`, the application attempts to load configuration in this order:

1. **Environment Variables**: `AUTH0_DOMAIN` and `AUTH0_AUDIENCE`. This is the primary method for production and staging.
2. **`appsettings.json`**: `Auth0:Domain` and `Auth0:Audience`. This is a fallback, useful for shared development settings.

For local development, the `DotNetEnv` NuGet package is used to automatically load a `.env` file if it exists.

### 1. Local Development (`.env` file)

Create a file named `.env` in the `backend/GscTracking.Api/` directory. This file is in the `.gitignore` and will not be committed.

```bash
# backend/GscTracking.Api/.env

# Auth0 Configuration
AUTH0_DOMAIN="your-dev-tenant.us.auth0.com"
AUTH0_AUDIENCE="https://gsc-tracking-api"
```

### 2. Fly.io Configuration (Staging & Production)

For deployed environments like Fly.io, you should use secrets to store your environment variables securely.

**Option A: Use `flyctl secrets set` (Recommended)**

This is the most secure method. Run these commands from your terminal:

```bash
# Staging Environment
flyctl secrets set AUTH0_DOMAIN="your-staging-domain" --app gsc-tracking-api-staging
flyctl secrets set AUTH0_AUDIENCE="your-staging-audience" --app gsc-tracking-api-staging

# Production Environment
flyctl secrets set AUTH0_DOMAIN="your-production-domain" --app gsc-tracking-api
flyctl secrets set AUTH0_AUDIENCE="your-production-audience" --app gsc-tracking-api
```

**Option B: Use `fly.toml` (Less Secure)**

You can also place environment variables in your `fly.toml` file. This is less secure as the file is committed to version control, but can be useful if the values are not highly sensitive.

Example for `fly.staging.toml`:

```toml
[env]
  ASPNETCORE_ENVIRONMENT = 'Staging'
  ASPNETCORE_URLS = 'http://+:8080'
  AUTH0_DOMAIN = 'dev-axm38gs1176ibx7p.us.auth0.com'
  AUTH0_AUDIENCE = 'https://gsc-tracking-api'
```

**Note:** Secrets set with `flyctl secrets set` will override any values in `fly.toml`. The recommended practice is to use secrets for all sensitive data.

## Frontend Configuration

### Environment Variables

1. **Local Development (.env.local)**

   Create `.env.local` in the frontend directory:

   ```bash
   # Auth0 Configuration
   VITE_AUTH0_DOMAIN=your-tenant.auth0.com
   VITE_AUTH0_CLIENT_ID=your-client-id
   VITE_AUTH0_AUDIENCE=https://api.gsc-tracking.com

   # API URL
   VITE_API_URL=http://localhost:5091/api
   ```

2. **Netlify Configuration**

   **Option A: Using Netlify UI**
   - Go to your Netlify site dashboard
   - Navigate to **Site settings** → **Environment variables**
   - Add the following variables:
     - `VITE_AUTH0_DOMAIN`: `your-tenant.auth0.com`
     - `VITE_AUTH0_CLIENT_ID`: `your-client-id`
     - `VITE_AUTH0_AUDIENCE`: `https://api.gsc-tracking.com`
     - `VITE_API_URL`: `https://gsc-tracking-api.fly.dev/api`

   **Option B: Using netlify.toml**

   Update `netlify.toml`:

   ```toml
   [context.production.environment]
     VITE_API_URL = "https://gsc-tracking-api.fly.dev/api"
     VITE_AUTH0_DOMAIN = "your-tenant.auth0.com"
     VITE_AUTH0_CLIENT_ID = "your-production-client-id"
     VITE_AUTH0_AUDIENCE = "https://api.gsc-tracking.com"

   [context.staging.environment]
     VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
     VITE_AUTH0_DOMAIN = "your-tenant.auth0.com"
     VITE_AUTH0_CLIENT_ID = "your-staging-client-id"
     VITE_AUTH0_AUDIENCE = "https://api.gsc-tracking.com"

   [context.deploy-preview.environment]
     VITE_API_URL = "https://gsc-tracking-api-staging.fly.dev/api"
     # Note: Deploy previews may not have Auth0 configured
     # as their URLs are not registered in Auth0
   ```

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
   - Click the "Log In" button in the sidebar
   - You should be redirected to Auth0's Universal Login page
   - Sign in or create a new account
   - You should be redirected back to the application
   - Your profile picture and name should appear in the sidebar

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

### ProtectedRoute Component

The application includes a `ProtectedRoute` component (`frontend/src/components/ProtectedRoute.tsx`) that can be used to protect specific routes in the frontend. This component ensures that only authenticated users can access certain pages.

**Current Status**: The `ProtectedRoute` component is **currently not integrated** into the application routing. It has been implemented as **preparatory work for future use** when route-level authentication is required.

**Features**:
- Checks if the user is authenticated using Auth0
- Shows a loading spinner while authentication status is being determined
- Redirects unauthenticated users to the Auth0 login page
- Gracefully handles cases where Auth0 is not configured (allows access)
- Displays appropriate loading states during redirects

**Future Usage Example**:

When you need to protect specific routes, you can wrap them with the `ProtectedRoute` component:

```tsx
import { ProtectedRoute } from './components/ProtectedRoute';

// In your routing configuration
<Routes>
  <Route path="/" element={<MainLayout />}>
    <Route index element={<Dashboard />} />
    
    {/* Public routes */}
    <Route path="about" element={<About />} />
    
    {/* Protected routes - require authentication */}
    <Route 
      path="customers" 
      element={
        <ProtectedRoute>
          <Customers />
        </ProtectedRoute>
      } 
    />
    
    <Route 
      path="jobs" 
      element={
        <ProtectedRoute>
          <Jobs />
        </ProtectedRoute>
      } 
    />
    
    <Route 
      path="jobs/:id" 
      element={
        <ProtectedRoute>
          <JobDetails />
        </ProtectedRoute>
      } 
    />
  </Route>
</Routes>
```

**Why Not Enabled Yet?**

The application currently uses optional authentication - users can use the app without logging in, but certain features (like creating/editing) may be restricted based on authentication status. When the business requirements change to require authentication for accessing the application, the `ProtectedRoute` component can be integrated following the example above.

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

1. **"Invalid state" error on callback**
   - Clear browser cookies and local storage
   - Verify the redirect URI matches exactly in Auth0 settings
   - Check that HTTPS is used in production URLs

2. **"Audience is required" error**
   - Verify `VITE_AUTH0_AUDIENCE` is set in frontend environment
   - Verify the audience matches your API identifier in Auth0

3. **401 Unauthorized on API calls**
   - Verify the access token is being sent with requests
   - Check that the API audience matches between frontend and backend
   - Verify Auth0 domain and audience are correctly set in backend

4. **CORS errors**
   - Verify your origin is listed in Auth0's Allowed Web Origins
   - Check backend CORS configuration includes your frontend URL

5. **"Login required" error on deploy previews**
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
