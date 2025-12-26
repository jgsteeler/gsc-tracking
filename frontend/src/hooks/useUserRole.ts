import { useAuth0 } from '@auth0/auth0-react';
import { jwtDecode } from 'jwt-decode';
import { useEffect, useState } from 'react';

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
        const decodedToken = jwtDecode<{ permissions?: string[] }>(accessToken);
        if (decodedToken.permissions) {
          setAsyncRoles(decodedToken.permissions);
        }
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
    canWrite: asyncRoles.includes('write') || asyncRoles.includes('tracker-write') || asyncRoles.includes('admin')|| asyncRoles.includes('tracker-admin'), 
    canRead: asyncRoles.includes('read') || asyncRoles.includes('tracker-read') || asyncRoles.includes('write') || asyncRoles.includes('admin') || asyncRoles.includes('tracker-admin'),
    roles: asyncRoles,
    isLoading: isLoading || !foundPermissions,
  };
};
