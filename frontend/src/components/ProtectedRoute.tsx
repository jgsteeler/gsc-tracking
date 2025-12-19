import { useAuth0 } from '@auth0/auth0-react';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { ReactNode } from 'react';

interface ProtectedRouteProps {
  children: ReactNode;
}

/**
 * Protected route component that requires authentication
 * Redirects unauthenticated users to the landing page before login
 * 
 * @param children - The component(s) to render if the user is authenticated
 * @returns The protected content or a loading/redirect state
 */
export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, isLoading } = useAuth0();
  const navigate = useNavigate();

  // If Auth0 is not configured (domain/clientId/audience missing),
  // the useAuth0 hook will not work properly. In this case, allow access.
  const auth0NotConfigured = !import.meta.env.VITE_AUTH0_DOMAIN ||
                               !import.meta.env.VITE_AUTH0_CLIENT_ID ||
                               !import.meta.env.VITE_AUTH0_AUDIENCE;

  // Redirect to landing page when not authenticated
  useEffect(() => {
    if (!auth0NotConfigured && !isAuthenticated && !isLoading) {
      navigate('/landing');
    }
  }, [auth0NotConfigured, isAuthenticated, isLoading, navigate]);

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
          <p className="text-gray-600">Redirecting to landing page...</p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};
