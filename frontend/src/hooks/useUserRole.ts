import { useAuth0 } from '@auth0/auth0-react';

/**
 * Custom hook to check if the current user has specific roles
 * 
 * Auth0 roles are typically included in the token under a custom namespace claim.
 * By default, Auth0 adds roles to the token as:
 * - `<namespace>/roles` (array of role strings)
 * 
 * @returns An object containing:
 * - isAdmin: Whether the user has the tracker-admin role
 * - roles: Array of role strings the user has
 * - isLoading: Whether the user data is still loading
 * 
 * @example
 * ```tsx
 * const { isAdmin, isLoading } = useUserRole();
 * 
 * if (isLoading) return <div>Loading...</div>;
 * 
 * return (
 *   <div>
 *     {isAdmin && <Button>Admin Action</Button>}
 *   </div>
 * );
 * ```
 */
export const useUserRole = () => {
  const { user, isLoading, isAuthenticated } = useAuth0();

  // If not authenticated or loading, return defaults
  if (!isAuthenticated || isLoading || !user) {
    return {
      isAdmin: false,
      roles: [],
      isLoading,
    };
  }

  // Auth0 can store roles in different claim formats depending on configuration
  // Common locations:
  // 1. Direct in user object (less common): user.roles
  // 2. In a namespaced claim: user['https://your-domain.com/roles']
  // 3. In the access token claims (requires getIdTokenClaims)
  
  // Try to extract roles from common locations
  let roles: string[] = [];
  
  // Check for roles in user object
  if (Array.isArray(user.roles)) {
    roles = user.roles as string[];
  }
  
  // Check for namespaced roles (common Auth0 pattern)
  // Auth0 typically uses a namespace like https://your-app.com/roles
  const possibleNamespaces = [
    'https://gsc-tracking.com/roles',
    'http://gsc-tracking.com/roles',
    'roles',
  ];
  
  for (const namespace of possibleNamespaces) {
    if (Array.isArray(user[namespace])) {
      roles = user[namespace] as string[];
      break;
    }
  }

  // Check if user has the tracker-admin role
  const isAdmin = roles.includes('tracker-admin');

  return {
    isAdmin,
    roles,
    isLoading: false,
  };
};
