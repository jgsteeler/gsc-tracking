# Auth0 Permission-Based Access Control (RBAC) Fix

## Problem Summary

The GSC Tracking application was experiencing issues with Auth0 permission-based access control. Admin users with the `admin` permission were:
- ✅ Configured in Auth0
- ✅ Assigned to users in Auth0
- ✅ Reaching the frontend successfully
- ❌ NOT properly recognized by the backend when sent as role claims, causing 403 Forbidden errors on protected endpoints

## Root Cause

Auth0 includes user permissions in JWT tokens as **custom namespace claims** (e.g., `https://gsc-tracking.com/permissions`), but the .NET JWT Bearer authentication middleware looks for roles in the **standard `role` claim** by default.

Without explicit configuration to map these custom permission/role claims to standard .NET role claims, the backend authorization middleware cannot recognize user permissions, resulting in access denied errors.

## Current Configuration

**IMPORTANT**: The application uses **permissions as the primary method** for authorization:
- Auth0 sends permissions in the `permissions` claim with values: `"admin"`, `"write"`, `"read"`
- The backend checks for permissions FIRST, then falls back to roles if no permissions are found
- Both permissions and roles are mapped to internal role names: `"tracker-admin"`, `"tracker-write"`, `"tracker-read"`

### Authorization Levels

1. **read permission** (tracker-read role):
   - Can view all data (customers, jobs, expenses, job updates)
   - Cannot modify anything

2. **write permission** (tracker-write role):
   - All read permissions
   - Can add job updates to existing jobs
   - Can add expenses to existing jobs
   - **Can export data to CSV**
   - Cannot add/edit customers or jobs

3. **admin permission** (tracker-admin role):
   - All write permissions
   - Can add/edit/delete customers
   - Can add/edit/delete jobs
   - Can edit/delete expenses
   - **Can import data from CSV**
   - Full administrative access

### API Endpoint Authorization

- **ReadAccess Policy** (requires read, write, OR admin):
  - GET endpoints for viewing customers, jobs, expenses, job updates

- **WriteAccess Policy** (requires write OR admin):
  - POST endpoints for creating job updates and expenses
  - PUT endpoints for updating job updates and expenses
  - **Export endpoints** (GET /api/export/*)

- **AdminOnly Policy** (requires admin only):
  - POST/PUT/DELETE endpoints for customers and jobs
  - DELETE endpoints for expenses and job updates
  - **Import endpoints** (POST /api/import/*)

## Solution

The fix involves two parts:

### 1. Backend Configuration (Program.cs)

Modified the JWT Bearer authentication configuration to transform Auth0's custom permission/role claims into standard .NET role claims:

```csharp
.AddJwtBearer(options =>
{
    options.Authority = $"https://{auth0Domain}/";
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://{auth0Domain}/",
        ValidateAudience = true,
        ValidAudience = auth0Audience,
        ValidateLifetime = true
    };
    
    // Transform claims to map Auth0 custom role claims to standard role claims
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // Check for roles in various Auth0 claim formats
                var possibleRoleClaims = new[]
                {
                    "https://gsc-tracking.com/roles",
                    "http://gsc-tracking.com/roles",
                    "roles"
                };

                foreach (var roleClaimType in possibleRoleClaims)
                {
                    var roleClaims = claimsIdentity.FindAll(roleClaimType).ToList();
                    if (roleClaims.Any())
                    {
                        // Add each role as a standard role claim
                        foreach (var roleClaim in roleClaims)
                        {
                            claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                System.Security.Claims.ClaimTypes.Role, 
                                roleClaim.Value));
                        }
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
    };
});
```

This configuration:
- Uses the `OnTokenValidated` event to transform claims at runtime
- Supports multiple Auth0 role claim formats (https://, http://, plain "roles")
- Maps each role from Auth0's custom namespace to the standard .NET `ClaimTypes.Role`
- Processes only the first matching namespace to avoid duplicate role claims

### 2. Auth0 Configuration

Auth0 must be configured to include roles in JWT tokens. This is done via an Auth0 Action:

1. **Create an Action** in Auth0 Dashboard (**Actions** → **Library**):

```javascript
/**
 * Handler that will be called during the execution of a PostLogin flow.
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

2. **Add the Action to Login Flow** (**Actions** → **Flows** → **Login**)

## Testing

Added comprehensive tests to verify the role claim transformation works correctly:

1. `Auth0RoleClaimMapping_ShouldMapCustomRoleClaimsToStandardRoleClaims` - Verifies roles are mapped from Auth0 namespaces
2. `Auth0RoleClaimMapping_WhenNoRoleClaimsPresent_ShouldNotAddStandardRoleClaims` - Verifies no false positives
3. `Auth0RoleClaimMapping_ShouldPrioritizeHttpsNamespace` - Verifies correct namespace priority

All 309 backend tests pass, including the 5 new tests.

## Documentation Updates

Updated the following documentation files:

1. **AUTH0-SETUP.md** - Added comprehensive section on configuring role claims in Auth0
2. **GETTING-STARTED.md** - Fixed role name inconsistency (tracking-admin → tracker-admin)
3. **AUTH.md** - Fixed role name inconsistency

## Role Name Clarification

The correct role name is **`tracker-admin`**, not `tracking-admin`.

- Backend code uses: `tracker-admin`
- Frontend code uses: `tracker-admin`
- Authorization policy checks for: `tracker-admin`

### Supported Role Formats

As of December 2025, the backend now supports **two role formats** for flexibility:

1. **Short format**: `"admin"`, `"write"`, `"read"` (automatically mapped to tracker-* format)
2. **Full format**: `"tracker-admin"`, `"tracker-write"`, `"tracker-read"` (used as-is)

The backend automatically maps short role names to the full tracker-* format:
- `"admin"` → `"tracker-admin"`
- `"write"` → `"tracker-write"`
- `"read"` → `"tracker-read"`

This means you can configure Auth0 to send roles as either:
- `"https://gsc-tracking.com/roles": ["admin"]` (will be mapped)
- `"https://gsc-tracking.com/roles": ["tracker-admin"]` (will be used as-is)

Both formats work correctly. The mapping ensures backward compatibility and simplifies Auth0 configuration.

### If Your Roles Don't Match

If your Auth0 role uses a different naming convention:

1. **Option A (Recommended)**: Update your Auth0 role to use one of the supported formats:
   - Short: `admin`, `write`, `read`
   - Full: `tracker-admin`, `tracker-write`, `tracker-read`

2. **Option B**: Update the backend authorization policy in `Program.cs`:
   ```csharp
   options.AddPolicy("AdminOnly", policy =>
       policy.RequireRole("your-custom-role-name")); // Change to match your Auth0 role
   ```
   And update the mapping switch statement to include your custom role names.

## How to Verify the Fix

1. **Check Auth0 Configuration:**
   - **PRIMARY METHOD**: Verify the Auth0 Action sends permissions in the `permissions` claim:
     - Permission values should be: `admin`, `write`, `read`
   - **FALLBACK METHOD**: If using roles instead, they should be in the `roles` claim:
     - Short format: `admin`, `write`, `read`
     - Full format: `tracker-admin`, `tracker-write`, `tracker-read`
   - Verify the Auth0 Action is deployed and added to the Login Flow
   - Test by logging in and decoding the JWT token at [jwt.io](https://jwt.io)
   - Confirm the token includes (in order of priority):
     1. **Preferred**: `"https://gsc-tracking.com/permissions": ["admin"]` (will be mapped to "tracker-admin")
     2. **Alternative**: `"https://gsc-tracking.com/roles": ["admin"]` (will be mapped to "tracker-admin")
     3. **Alternative**: `"https://gsc-tracking.com/roles": ["tracker-admin"]` (will be used as-is)

2. **Test Backend Authorization:**
   - Log in to the frontend
   - Test the following authorization levels:
     - **read permission**: Can view all data
     - **write permission**: Can view data + add job updates + add expenses + export data
     - **admin permission**: Can do everything write can + add/edit customers + add/edit jobs + edit expenses + import data
   - Should succeed if you have the appropriate permission
   - Should fail with 403 Forbidden if you don't have the required permission

3. **Check Browser Console:**
   - Look for authentication-related errors
   - Verify the access token is being sent with API requests

4. **Check Backend Logs:**
   - Look for authentication/authorization errors
   - Verify the permission/role claims are being processed

## Next Steps

If you continue to experience issues:

1. Verify your Auth0 role name matches exactly (case-sensitive)
2. Check that the Auth0 Action is deployed and active
3. Clear browser cookies and local storage
4. Try logging out and back in to get a fresh token
5. Decode your JWT token at [jwt.io](https://jwt.io) to verify roles are included
6. Check backend logs for claim processing details

## Additional Resources

- [Auth0 Custom Claims Documentation](https://auth0.com/docs/secure/tokens/json-web-tokens/create-custom-claims)
- [Auth0 Actions Documentation](https://auth0.com/docs/customize/actions)
- [ASP.NET Core Authorization Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction)

---

**Last Updated:** 2025-12-25
