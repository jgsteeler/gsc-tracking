import { useAuth0 } from '@auth0/auth0-react';
import { Button } from './ui/button';

/**
 * Login button component that triggers Auth0 authentication
 */
export const LoginButton = () => {
  const { loginWithRedirect, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return <Button disabled>Loading...</Button>;
  }

  if (isAuthenticated) {
    return null; // Don't show login button if already authenticated
  }

  return (
    <Button onClick={() => loginWithRedirect()}>
      Log In
    </Button>
  );
};
