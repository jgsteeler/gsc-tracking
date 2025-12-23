import type { Customer, CustomerRequestDto } from '@/types/customer'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

/**
 * Helper function to create headers with optional Authorization token
 */
const createHeaders = (token?: string | null): HeadersInit => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  return headers
}

export const customerService = {
  async getAll(search?: string, token?: string | null): Promise<Customer[]> {
    const url = new URL(`${API_BASE_URL}/customers`)
    if (search) {
      url.searchParams.append('search', search)
    }
    
    const response = await fetch(url.toString(), {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch customers')
    }
    return response.json()
  },

  async getById(id: number, token?: string | null): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch customer')
    }
    return response.json()
  },

  async create(data: CustomerRequestDto, token?: string | null): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers`, {
      method: 'POST',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create customer' }))
      throw new Error(errorData.message || 'Failed to create customer')
    }
    return response.json()
  },

  async update(id: number, data: CustomerRequestDto, token?: string | null): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`, {
      method: 'PUT',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update customer' }))
      throw new Error(errorData.message || 'Failed to update customer')
    }
    return response.json()
  },

  async delete(id: number, token?: string | null): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`, {
      method: 'DELETE',
      headers: createHeaders(token),
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete customer')
    }
  },
}
