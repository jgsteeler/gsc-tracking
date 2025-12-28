import type { Customer, CustomerRequestDto } from '@/types/customer'
import { apiClient } from '@/lib/api-client'

export const customerService = {
  async getAll(search?: string, token?: string | null): Promise<Customer[]> {
    let url = '/customers'
    if (search) {
      url += `?search=${encodeURIComponent(search)}`
    }
    return apiClient.get<Customer[]>(url, token)
  },

  async getById(id: number, token?: string | null): Promise<Customer> {
    return apiClient.get<Customer>(`/customers/${id}`, token)
  },

  async create(data: CustomerRequestDto, token?: string | null): Promise<Customer> {
    return apiClient.post<Customer>('/customers', data, token)
  },

  async update(id: number, data: CustomerRequestDto, token?: string | null): Promise<Customer> {
    return apiClient.put<Customer>(`/customers/${id}`, data, token)
  },

  async delete(id: number, token?: string | null): Promise<void> {
    return apiClient.delete(`/customers/${id}`, token)
  },
}
