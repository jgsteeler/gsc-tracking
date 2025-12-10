import { describe, it, expect, beforeEach, vi } from 'vitest';
import { customerService } from './customerService';
import type { Customer, CustomerRequestDto } from '@/types/customer';

// Mock fetch
global.fetch = vi.fn();

function createFetchResponse(data: any, ok = true, status = 200) {
  return {
    ok,
    status,
    json: async () => data,
  } as Response;
}

describe('customerService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAll', () => {
    it('should fetch all customers without search', async () => {
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
      ];

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(mockCustomers)
      );

      const result = await customerService.getAll();

      expect(result).toEqual(mockCustomers);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/customers')
      );
    });

    it('should fetch customers with search parameter', async () => {
      const mockCustomers: Customer[] = [];
      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(mockCustomers)
      );

      await customerService.getAll('john');

      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('search=john')
      );
    });

    it('should throw error when fetch fails', async () => {
      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(null, false, 500)
      );

      await expect(customerService.getAll()).rejects.toThrow(
        'Failed to fetch customers'
      );
    });
  });

  describe('getById', () => {
    it('should fetch a customer by id', async () => {
      const mockCustomer: Customer = {
        id: 1,
        name: 'John Doe',
        email: 'john@example.com',
        phone: '1234567890',
        address: '123 Main St',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(mockCustomer)
      );

      const result = await customerService.getById(1);

      expect(result).toEqual(mockCustomer);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/customers/1')
      );
    });

    it('should throw error when customer not found', async () => {
      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(null, false, 404)
      );

      await expect(customerService.getById(999)).rejects.toThrow(
        'Failed to fetch customer'
      );
    });
  });

  describe('create', () => {
    it('should create a new customer', async () => {
      const customerRequest: CustomerRequestDto = {
        name: 'New Customer',
        email: 'new@example.com',
        phone: '1112223333',
        address: '999 New St',
      };

      const mockCustomer: Customer = {
        id: 1,
        ...customerRequest,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(mockCustomer)
      );

      const result = await customerService.create(customerRequest);

      expect(result).toEqual(mockCustomer);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/customers'),
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(customerRequest),
        })
      );
    });

    it('should throw error when creation fails', async () => {
      const customerRequest: CustomerRequestDto = {
        name: 'New Customer',
        email: 'new@example.com',
        phone: '1112223333',
        address: '999 New St',
      };

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse({ message: 'Validation error' }, false, 400)
      );

      await expect(customerService.create(customerRequest)).rejects.toThrow(
        'Validation error'
      );
    });
  });

  describe('update', () => {
    it('should update an existing customer', async () => {
      const customerRequest: CustomerRequestDto = {
        name: 'Updated Customer',
        email: 'updated@example.com',
        phone: '9999999999',
        address: 'Updated Address',
      };

      const mockCustomer: Customer = {
        id: 1,
        ...customerRequest,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(mockCustomer)
      );

      const result = await customerService.update(1, customerRequest);

      expect(result).toEqual(mockCustomer);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/customers/1'),
        expect.objectContaining({
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(customerRequest),
        })
      );
    });

    it('should throw error when update fails', async () => {
      const customerRequest: CustomerRequestDto = {
        name: 'Updated Customer',
        email: 'updated@example.com',
        phone: '9999999999',
        address: 'Updated Address',
      };

      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse({ message: 'Not found' }, false, 404)
      );

      await expect(
        customerService.update(999, customerRequest)
      ).rejects.toThrow('Not found');
    });
  });

  describe('delete', () => {
    it('should delete a customer', async () => {
      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(null, true, 204)
      );

      await customerService.delete(1);

      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/customers/1'),
        expect.objectContaining({
          method: 'DELETE',
        })
      );
    });

    it('should throw error when delete fails', async () => {
      (global.fetch as any).mockResolvedValueOnce(
        createFetchResponse(null, false, 404)
      );

      await expect(customerService.delete(999)).rejects.toThrow(
        'Failed to delete customer'
      );
    });
  });
});
