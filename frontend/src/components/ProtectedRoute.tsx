import { useAuth0 } from '@auth0/auth0-react';
import { useEffect } from 'react';
import type { ReactNode } from 'react';

interface ProtectedRouteProps {
  children: ReactNode;
}

/**
 * Protected route component that requires authentication
 * 
 * CURRENT STATUS: This component is not currently integrated into the application routing.
 * It has been implemented as preparatory work for future use when route-level authentication
 * becomes a requirement.
 * 
 * FEATURES:
 * - Checks if the user is authenticated using Auth0
 * - Shows a loading spinner while authentication status is being determined
 * - Redirects unauthenticated users to the Auth0 login page
 * - Gracefully handles cases where Auth0 is not configured (allows public access)
 * 
 * FUTURE USAGE:
 * Wrap routes that require authentication with this component in App.tsx:
 * 
 * ```tsx
 * <Route 
 *   path="customers" 
 *   element={
 *     <ProtectedRoute>
 *       <Customers />
 *     </ProtectedRoute>
 *   } 
 * />
 * ```
 * 
 * See docs/AUTH0-SETUP.md for complete usage examples and integration guide.
 * 
 * @param children - The component(s) to render if the user is authenticated
 * @returns The protected content or a loading/redirect state
 */
export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();

  // If Auth0 is not configured (domain/clientId/audience missing),
  // the useAuth0 hook will not work properly. In this case, allow access.
  const auth0NotConfigured = !import.meta.env.VITE_AUTH0_DOMAIN ||
                               !import.meta.env.VITE_AUTH0_CLIENT_ID ||
                               !import.meta.env.VITE_AUTH0_AUDIENCE;

  // Trigger login redirect when not authenticated
  useEffect(() => {
    if (!auth0NotConfigured && !isAuthenticated && !isLoading) {
      loginWithRedirect();
    }
  }, [auth0NotConfigured, isAuthenticated, isLoading, loginWithRedirect]);

  if (auth0NotConfigured) {
    // Auth0 not configured, allow access
    return <>{children}</>;
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <p className="text-gray-600">Redirecting to login...</p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};
