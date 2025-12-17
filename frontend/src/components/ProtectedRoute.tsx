import { useAuth0 } from '@auth0/auth0-react';
import type { ReactNode } from 'react';

interface ProtectedRouteProps {
  children: ReactNode;
}

/**
 * Protected route component that requires authentication
 * If Auth0 is not configured, the route is publicly accessible
 */
export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();

  // If Auth0 is not configured (domain/clientId/audience missing),
  // the useAuth0 hook will not work properly. In this case, allow access.
  const auth0NotConfigured = !import.meta.env.VITE_AUTH0_DOMAIN ||
                               !import.meta.env.VITE_AUTH0_CLIENT_ID ||
                               !import.meta.env.VITE_AUTH0_AUDIENCE;

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
    // Trigger login redirect
    loginWithRedirect();
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
