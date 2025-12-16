import { useAuth0 } from '@auth0/auth0-react';

/**
 * Custom hook to get the access token for API calls
 * Returns null if Auth0 is not configured
 */
export const useAccessToken = () => {
  const { getAccessTokenSilently, isAuthenticated } = useAuth0();

  const auth0NotConfigured = !import.meta.env.VITE_AUTH0_DOMAIN ||
                               !import.meta.env.VITE_AUTH0_CLIENT_ID ||
                               !import.meta.env.VITE_AUTH0_AUDIENCE;

  const getToken = async (): Promise<string | null> => {
    if (auth0NotConfigured || !isAuthenticated) {
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

  return { getToken, isAuthenticated: !auth0NotConfigured && isAuthenticated };
};
