import { useAuth0 } from '@auth0/auth0-react';

/**
 * Custom hook to check if the current user has specific roles or permissions
 * 
 * Auth0 can send either permissions or roles in the token under a custom namespace claim.
 * This hook supports both approaches:
 * - Permissions: "admin", "write", "read" (recommended)
 * - Roles: "tracker-admin", "tracker-write", "tracker-read" (alternative)
 * 
 * @returns An object containing:
 * - isAdmin: Whether the user has admin access (full permissions)
 * - canWrite: Whether the user has write access (can add expenses and job updates)
 * - canRead: Whether the user has read access (can view data)
 * - roles: Array of role/permission strings the user has
 * - isLoading: Whether the user data is still loading
 * 
 * @example
 * ```tsx
 * const { isAdmin, canWrite, canRead, isLoading } = useUserRole();
 * 
 * if (isLoading) return <div>Loading...</div>;
 * 
 * return (
 *   <div>
 *     {isAdmin && <Button>Admin Action</Button>}
 *     {canWrite && <Button>Add Expense</Button>}
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
      canWrite: false,
      canRead: false,
      roles: [],
      isLoading,
    };
  }

  // Auth0 can store permissions/roles in different claim formats
  // Priority: Check for permissions first, then fall back to roles
  
  let roles: string[] = [];
  
  // First, check for permissions claim (recommended approach)
  const possiblePermissionNamespaces = [
    'https://gsc-tracking.com/permissions',
    'http://gsc-tracking.com/permissions',
    'permissions',
  ];
  
  let foundPermissions = false;
  for (const namespace of possiblePermissionNamespaces) {
    if (Array.isArray(user[namespace])) {
      roles = user[namespace] as string[];
      foundPermissions = true;
      break;
    }
  }
  
  // If no permissions found, check for roles (alternative approach)
  if (!foundPermissions) {
    // Check for roles in user object
    if (Array.isArray(user.roles)) {
      roles = user.roles as string[];
    } else {
      // Check for namespaced roles
      const possibleRoleNamespaces = [
        'https://gsc-tracking.com/roles',
        'http://gsc-tracking.com/roles',
        'roles',
      ];
      
      for (const namespace of possibleRoleNamespaces) {
        if (Array.isArray(user[namespace])) {
          roles = user[namespace] as string[];
          break;
        }
      }
    }
  }

  // Map permissions/roles to access levels
  // Supports both permission format (admin, write, read) and role format (tracker-admin, tracker-write, tracker-read)
  const isAdmin = roles.includes('admin') || roles.includes('tracker-admin');
  const canWrite = isAdmin || roles.includes('write') || roles.includes('tracker-write');
  const canRead = canWrite || roles.includes('read') || roles.includes('tracker-read');

  return {
    isAdmin,
    canWrite,
    canRead,
    roles,
    isLoading: false,
  };
};
