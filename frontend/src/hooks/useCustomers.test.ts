import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook, waitFor, act } from '@testing-library/react';
import { useCustomers } from './useCustomers';
import { customerService } from '@/services/customerService';
import type { Customer, CustomerRequestDto } from '@/types/customer';

// Mock the customer service
vi.mock('@/services/customerService', () => ({
  customerService: {
    getAll: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}));

// Mock useAccessToken with a stable getToken function
const mockGetToken = vi.fn().mockResolvedValue(null);
vi.mock('@/hooks/useAccessToken', () => ({
  useAccessToken: () => ({
    getToken: mockGetToken,
    isAuthEnabled: false,
    isAuthenticated: false,
  }),
}));

describe('useCustomers', () => {
  const mockCustomers: Customer[] = [
    {
      id: 1,
      name: 'John Doe',
      email: 'john@example.com',
      phone: '1234567890',
      address: '123 Main St',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: 2,
      name: 'Jane Smith',
      email: 'jane@example.com',
      phone: '0987654321',
      address: '456 Oak Ave',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should fetch customers on mount', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);

    const { result } = renderHook(() => useCustomers());

    expect(result.current.loading).toBe(true);
    expect(result.current.customers).toEqual([]);

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.customers).toEqual(mockCustomers);
    expect(result.current.error).toBeNull();
    expect(customerService.getAll).toHaveBeenCalledWith(undefined, null);
  });

  it('should fetch customers with search term', async () => {
    const searchTerm = 'john';
    vi.mocked(customerService.getAll).mockResolvedValueOnce([mockCustomers[0]]);

    const { result } = renderHook(() => useCustomers(searchTerm));

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.customers).toEqual([mockCustomers[0]]);
    expect(customerService.getAll).toHaveBeenCalledWith(searchTerm, null);
  });

  it('should handle fetch error', async () => {
    const errorMessage = 'Failed to fetch customers';
    vi.mocked(customerService.getAll).mockRejectedValueOnce(new Error(errorMessage));

    const { result } = renderHook(() => useCustomers());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBe(errorMessage);
    expect(result.current.customers).toEqual([]);
  });

  it('should create a new customer', async () => {
    const newCustomerRequest: CustomerRequestDto = {
      name: 'New Customer',
      email: 'new@example.com',
      phone: '1112223333',
      address: '999 New St',
    };

    const newCustomer: Customer = {
      id: 3,
      ...newCustomerRequest,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(customerService.create).mockResolvedValueOnce(newCustomer);

    const { result } = renderHook(() => useCustomers());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    let createdCustomer: Customer | undefined;
    await act(async () => {
      createdCustomer = await result.current.createCustomer(newCustomerRequest);
    });

    expect(createdCustomer).toEqual(newCustomer);
    expect(result.current.customers).toContainEqual(newCustomer);
    expect(result.current.customers.length).toBe(3);
  });

  it('should update an existing customer', async () => {
    const updateRequest: CustomerRequestDto = {
      name: 'Updated Name',
      email: 'updated@example.com',
      phone: '9999999999',
      address: 'Updated Address',
    };

    const updatedCustomer: Customer = {
      id: 1,
      ...updateRequest,
      createdAt: mockCustomers[0].createdAt,
      updatedAt: new Date().toISOString(),
    };

    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(customerService.update).mockResolvedValueOnce(updatedCustomer);

    const { result } = renderHook(() => useCustomers());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    let updated: Customer | undefined;
    await act(async () => {
      updated = await result.current.updateCustomer(1, updateRequest);
    });

    expect(updated).toEqual(updatedCustomer);
    expect(result.current.customers.find((c) => c.id === 1)).toEqual(updatedCustomer);
  });

  it('should delete a customer', async () => {
    vi.mocked(customerService.getAll).mockResolvedValueOnce(mockCustomers);
    vi.mocked(customerService.delete).mockResolvedValueOnce();

    const { result } = renderHook(() => useCustomers());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.customers.length).toBe(2);

    await act(async () => {
      await result.current.deleteCustomer(1);
    });

    expect(result.current.customers.length).toBe(1);
    expect(result.current.customers.find((c) => c.id === 1)).toBeUndefined();
  });

  it('should refetch customers', async () => {
    vi.mocked(customerService.getAll)
      .mockResolvedValueOnce(mockCustomers)
      .mockResolvedValueOnce([mockCustomers[0]]);

    const { result } = renderHook(() => useCustomers());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.customers.length).toBe(2);

    await result.current.refetch();

    await waitFor(() => {
      expect(result.current.customers.length).toBe(1);
    });
  });
});
