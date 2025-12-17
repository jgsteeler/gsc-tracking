import { useAuth0 } from '@auth0/auth0-react';
import { Button } from './ui/button';

/**
 * Logout button component that logs out from Auth0
 */
export const LogoutButton = () => {
  const { logout, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return null;
  }

  if (!isAuthenticated) {
    return null; // Don't show logout button if not authenticated
  }

  return (
    <Button
      variant="outline"
      onClick={() =>
        logout({
          logoutParams: {
            returnTo: window.location.origin,
          },
        })
      }
    >
      Log Out
    </Button>
  );
};
