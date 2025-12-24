# Auth0 Role-Based Access Control (RBAC) Fix

## Problem Summary

The GSC Tracking application was experiencing issues with Auth0 role-based access control. The `tracker-admin` role was:
- ✅ Configured in Auth0
- ✅ Assigned to users in Auth0
- ✅ Reaching the frontend successfully
- ❌ NOT reaching the backend, causing 403 Forbidden errors on protected endpoints

## Root Cause

Auth0 includes user roles in JWT tokens as **custom namespace claims** (e.g., `https://gsc-tracking.com/roles`), but the .NET JWT Bearer authentication middleware looks for roles in the **standard `role` claim** by default.

Without explicit configuration to map these custom claims to standard .NET role claims, the backend authorization middleware cannot recognize user roles, resulting in access denied errors.

## Solution

The fix involves two parts:

### 1. Backend Configuration (Program.cs)

Modified the JWT Bearer authentication configuration to transform Auth0's custom role claims into standard .NET role claims:

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
        ValidateLifetime = true,
        // Map Auth0's role claim to the standard role claim
        RoleClaimType = "https://gsc-tracking.com/roles"
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
- Sets the `RoleClaimType` to match Auth0's namespace
- Uses the `OnTokenValidated` event to transform claims
- Supports multiple Auth0 role claim formats (https://, http://, plain "roles")
- Maps each role to the standard .NET `ClaimTypes.Role`

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

If your Auth0 role is named `tracking-admin`, you need to either:
1. Rename it to `tracker-admin` in Auth0, OR
2. Update the backend authorization policy in `Program.cs`:
   ```csharp
   options.AddPolicy("AdminOnly", policy =>
       policy.RequireRole("tracking-admin")); // Change to match your Auth0 role
   ```

## How to Verify the Fix

1. **Check Auth0 Configuration:**
   - Verify the role name in Auth0 matches the backend expectation
   - Verify the Auth0 Action is deployed and added to the Login Flow
   - Test by logging in and decoding the JWT token at [jwt.io](https://jwt.io)
   - Confirm the token includes: `"https://gsc-tracking.com/roles": ["tracker-admin"]`

2. **Test Backend Authorization:**
   - Log in to the frontend
   - Attempt to create, update, or delete a customer/job/expense
   - Should succeed if you have the `tracker-admin` role
   - Should fail with 403 Forbidden if you don't have the role

3. **Check Browser Console:**
   - Look for authentication-related errors
   - Verify the access token is being sent with API requests

4. **Check Backend Logs:**
   - Look for authentication/authorization errors
   - Verify the role claims are being processed

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

**Last Updated:** 2025-12-24
