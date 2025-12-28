import { useAuth0 } from '@auth0/auth0-react';
import { jwtDecode } from 'jwt-decode';
import { useEffect, useState } from 'react';

/**
 * Custom hook to check if the current user has specific roles or permissions
 * 
 * Auth0 sends permissions or roles in the token under a custom namespace claim.
 * This hook checks for permissions/roles in the following order:
 * 1. "https://gsc-tracking.com/permissions" - Namespaced permissions claim (recommended)
 * 2. "permissions" - Standard permissions claim (for testing/fallback)
 * 3. "https://gsc-tracking.com/roles" - Namespaced roles claim (alternative)
 * 
 * Supported permission values: "admin", "write", "read"
 * Supported role values: "tracker-admin", "tracker-write", "tracker-read"
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
  const { isLoading, isAuthenticated, getAccessTokenSilently } = useAuth0();

  const [asyncRoles, setAsyncRoles] = useState<string[]>([]);
  const [foundPermissions, setFoundPermissions] = useState(false);

  useEffect(() => {
    if (!isAuthenticated || isLoading) {
      setFoundPermissions(true); // Ensure isLoading becomes false for unauthenticated state
      return;
    }

    const fetchAccessToken = async () => {
      try {
        const accessToken = await getAccessTokenSilently();
        const decodedToken = jwtDecode<{ 
          permissions?: string[];
          'https://gsc-tracking.com/permissions'?: string[];
          'https://gsc-tracking.com/roles'?: string[];
        }>(accessToken);
        
        // Check for permissions in order of preference:
        // 1. Namespaced permissions claim (standard Auth0 format)
        // 2. Standard permissions claim (for testing/fallback)
        // 3. Namespaced roles claim (alternative Auth0 format)
        const roles = decodedToken['https://gsc-tracking.com/permissions'] 
          || decodedToken.permissions 
          || decodedToken['https://gsc-tracking.com/roles']
          || [];
        
        setAsyncRoles(roles);
      } catch (error) {
        console.error('Error fetching or decoding access token:', error);
      } finally {
        setFoundPermissions(true); // Ensure isLoading becomes false even on error
      }
    };

    fetchAccessToken();
  }, [isAuthenticated, isLoading, getAccessTokenSilently]);

  return {
    isAdmin: asyncRoles.includes('admin') || asyncRoles.includes('tracker-admin'),
    canWrite: asyncRoles.includes('write') || asyncRoles.includes('tracker-write') || asyncRoles.includes('admin') || asyncRoles.includes('tracker-admin'), 
    canRead: asyncRoles.includes('read') || asyncRoles.includes('tracker-read') || asyncRoles.includes('write') || asyncRoles.includes('admin') || asyncRoles.includes('tracker-admin'),
    roles: asyncRoles,
    isLoading: isLoading || !foundPermissions,
  };
};
