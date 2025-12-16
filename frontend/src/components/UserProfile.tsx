import { useAuth0 } from '@auth0/auth0-react';

/**
 * User profile component that displays authenticated user information
 */
export const UserProfile = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <div className="flex items-center space-x-2">
      {user.picture && (
        <img
          src={user.picture}
          alt={user.name || 'User'}
          className="w-8 h-8 rounded-full"
        />
      )}
      <span className="text-sm font-medium">{user.name || user.email}</span>
    </div>
  );
};
