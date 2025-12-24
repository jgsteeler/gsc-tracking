import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useUserRole } from './useUserRole';
import { useAuth0 } from '@auth0/auth0-react';
import type { User } from '@auth0/auth0-react';

// Mock the useAuth0 hook
vi.mock('@auth0/auth0-react', () => ({
  useAuth0: vi.fn(),
}));

describe('useUserRole', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should return all false when not authenticated', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: undefined,
      isLoading: false,
      isAuthenticated: false,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(false);
    expect(result.current.canRead).toBe(false);
    expect(result.current.roles).toEqual([]);
    expect(result.current.isLoading).toBe(false);
  });

  it('should return isLoading true when loading', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: undefined,
      isLoading: true,
      isAuthenticated: false,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.isLoading).toBe(true);
  });

  it('should return admin permissions when user has admin permission', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'https://gsc-tracking.com/permissions': ['admin'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(true);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['admin']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should return write permissions when user has write permission', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'https://gsc-tracking.com/permissions': ['write'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['write']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should return read permissions when user has read permission', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'https://gsc-tracking.com/permissions': ['read'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(false);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['read']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should work with tracker-admin role format', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        roles: ['tracker-admin', 'some-other-role'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(true);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['tracker-admin', 'some-other-role']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should work with tracker-write role format', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        roles: ['tracker-write'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['tracker-write']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should prioritize permissions over roles', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'https://gsc-tracking.com/permissions': ['write'],
        'https://gsc-tracking.com/roles': ['tracker-read'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['write']);
    expect(result.current.isLoading).toBe(false);
  });

  it('should return false for all permissions when user has no roles', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {} as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(false);
    expect(result.current.canWrite).toBe(false);
    expect(result.current.canRead).toBe(false);
    expect(result.current.roles).toEqual([]);
    expect(result.current.isLoading).toBe(false);
  });

  it('should check multiple namespace formats for permissions', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'http://gsc-tracking.com/permissions': ['admin'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(true);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['admin']);
  });

  it('should check multiple namespace formats for roles', () => {
    vi.mocked(useAuth0).mockReturnValue({
      user: {
        'http://gsc-tracking.com/roles': ['tracker-admin'],
      } as User,
      isLoading: false,
      isAuthenticated: true,
      getAccessTokenSilently: vi.fn(),
      getAccessTokenWithPopup: vi.fn(),
      getIdTokenClaims: vi.fn(),
      loginWithRedirect: vi.fn(),
      loginWithPopup: vi.fn(),
      logout: vi.fn(),
      handleRedirectCallback: vi.fn(),
    });

    const { result } = renderHook(() => useUserRole());

    expect(result.current.isAdmin).toBe(true);
    expect(result.current.canWrite).toBe(true);
    expect(result.current.canRead).toBe(true);
    expect(result.current.roles).toEqual(['tracker-admin']);
  });
});
