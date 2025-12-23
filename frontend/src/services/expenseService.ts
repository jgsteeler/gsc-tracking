import type { Expense, ExpenseRequestDto } from '@/types/expense'
import { formatDateForApi } from '@/lib/utils'

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

export const expenseService = {
  async getByJobId(jobId: number, token?: string | null): Promise<Expense[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch expenses')
    }
    return response.json()
  },

  async create(jobId: number, data: ExpenseRequestDto, token?: string | null): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses`, {
      method: 'POST',
      headers: createHeaders(token),
      body: JSON.stringify(submitData),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create expense' }))
      throw new Error(errorData.message || 'Failed to create expense')
    }
    return response.json()
  },

  async update(jobId: number, id: number, data: ExpenseRequestDto, token?: string | null): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses/${id}`, {
      method: 'PUT',
      headers: createHeaders(token),
      body: JSON.stringify(submitData),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update expense' }))
      throw new Error(errorData.message || 'Failed to update expense')
    }
    return response.json()
  },

  async delete(jobId: number, id: number, token?: string | null): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses/${id}`, {
      method: 'DELETE',
      headers: createHeaders(token),
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete expense')
    }
  },
}
