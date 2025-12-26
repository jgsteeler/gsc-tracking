import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { useUserRole } from '../../src/hooks/useUserRole';
import { useAuth0 } from '@auth0/auth0-react';
import type { Auth0ContextInterface } from '@auth0/auth0-react';
import { encode } from 'base64-url'; // Add this import for encoding

// Mock the useAuth0 hook
vi.mock('@auth0/auth0-react', () => ({
  useAuth0: vi.fn(),
}));

// Helper function to create a mock JWT
function createMockJWT(payload: Record<string, unknown>) {
  const header = { alg: 'HS256', typ: 'JWT' };
  return `${encode(JSON.stringify(header))}.${encode(JSON.stringify(payload))}.signature`;
}

describe('useUserRole', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should return isLoading true initially', async () => {
    const auth0Mock = {
      isAuthenticated: true,
      isLoading: true,
      user: null,
      getAccessTokenSilently: vi.fn(),
    } satisfies Partial<Auth0ContextInterface>;

    vi.mocked(useAuth0).mockReturnValue(auth0Mock as Auth0ContextInterface);

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isLoading).toBe(true);
  });

  it('should return roles and permissions after fetching token', async () => {
    const mockPayload = { permissions: ['admin', 'write'] };
    const mockToken = createMockJWT(mockPayload);

    const auth0Mock = {
      isAuthenticated: true,
      isLoading: false,
      user: {},
      getAccessTokenSilently: vi.fn().mockResolvedValue(mockToken),
    } satisfies Partial<Auth0ContextInterface>;

    vi.mocked(useAuth0).mockReturnValue(auth0Mock as Auth0ContextInterface);

    const { result } = renderHook(() => useUserRole());

    await waitFor(() => {
      expect(result.current.roles).toEqual(['admin', 'write']);
      expect(result.current.isAdmin).toBe(true);
      expect(result.current.canWrite).toBe(true);
      expect(result.current.canRead).toBe(true);
      expect(result.current.isLoading).toBe(false);
    });
  });

  it('should handle token fetch errors gracefully', async () => {
    const auth0Mock = {
      isAuthenticated: true,
      isLoading: false,
      user: {},
      getAccessTokenSilently: vi.fn().mockRejectedValue(new Error('Token fetch failed')),
    } satisfies Partial<Auth0ContextInterface>;

    vi.mocked(useAuth0).mockReturnValue(auth0Mock as Auth0ContextInterface);

    const { result } = renderHook(() => useUserRole());

    await waitFor(() => {
      expect(result.current.roles).toEqual([]);
      expect(result.current.isAdmin).toBe(false);
      expect(result.current.canWrite).toBe(false);
      expect(result.current.canRead).toBe(false);
      expect(result.current.isLoading).toBe(false);
    });
  });

  it('should return false for all permissions when not authenticated', () => {
    const auth0Mock = {
      isAuthenticated: false,
      isLoading: false,
      user: null,
      getAccessTokenSilently: vi.fn(),
    } satisfies Partial<Auth0ContextInterface>;

    vi.mocked(useAuth0).mockReturnValue(auth0Mock as Auth0ContextInterface);

    const { result } = renderHook(() => useUserRole());

    expect(result.current.roles).toEqual([]);
    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(false);
    expect(result.current.canRead).toBe(false);
    expect(result.current.isLoading).toBe(false);
  });
});
