import type { Customer, CustomerRequestDto } from '@/types/customer'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

export const customerService = {
  async getAll(search?: string): Promise<Customer[]> {
    const url = new URL(`${API_BASE_URL}/customers`)
    if (search) {
      url.searchParams.append('search', search)
    }
    
    const response = await fetch(url.toString())
    if (!response.ok) {
      throw new Error('Failed to fetch customers')
    }
    return response.json()
  },

  async getById(id: number): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`)
    if (!response.ok) {
      throw new Error('Failed to fetch customer')
    }
    return response.json()
  },

  async create(data: CustomerRequestDto): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create customer' }))
      throw new Error(errorData.message || 'Failed to create customer')
    }
    return response.json()
  },

  async update(id: number, data: CustomerRequestDto): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update customer' }))
      throw new Error(errorData.message || 'Failed to update customer')
    }
    return response.json()
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/customers/${id}`, {
      method: 'DELETE',
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete customer')
    }
  },
}
