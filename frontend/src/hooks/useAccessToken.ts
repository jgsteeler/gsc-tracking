import { useAuth0 } from '@auth0/auth0-react';

/**
 * Custom hook to get the access token for API calls and check authentication state
 * 
 * @returns An object containing:
 * - getToken: Function to retrieve the access token (returns null if not authenticated or Auth0 not configured)
 * - isAuthEnabled: Whether Auth0 authentication is configured and enabled
 * - isAuthenticated: Whether the user is currently authenticated (only meaningful when isAuthEnabled is true)
 * 
 * @example
 * ```tsx
 * const { getToken, isAuthEnabled, isAuthenticated } = useAccessToken();
 * 
 * // Check if auth is enabled
 * if (!isAuthEnabled) {
 *   // Auth0 not configured - proceed without authentication
 *   return;
 * }
 * 
 * // Check if user is authenticated
 * if (isAuthenticated) {
 *   const token = await getToken();
 *   // Use token for API calls
 * }
 * ```
 */
export const useAccessToken = () => {
  const { getAccessTokenSilently, isAuthenticated } = useAuth0();

  const isAuthEnabled = !!(import.meta.env.VITE_AUTH0_DOMAIN &&
                            import.meta.env.VITE_AUTH0_CLIENT_ID &&
                            import.meta.env.VITE_AUTH0_AUDIENCE);

  const getToken = async (): Promise<string | null> => {
    if (!isAuthEnabled || !isAuthenticated) {
      return null;
    }

    try {
      const token = await getAccessTokenSilently({
        authorizationParams: {
          audience: import.meta.env.VITE_AUTH0_AUDIENCE,
        },
      });
      return token;
    } catch (error) {
      console.error('Error getting access token:', error);
      return null;
    }
  };

  return { 
    getToken, 
    isAuthEnabled,
    isAuthenticated 
  };
};
