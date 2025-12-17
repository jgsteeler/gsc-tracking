import type { Expense, ExpenseRequestDto } from '@/types/expense'
import { formatDateForApi } from '@/lib/utils'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

export const expenseService = {
  async getByJobId(jobId: number): Promise<Expense[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses`)
    if (!response.ok) {
      throw new Error('Failed to fetch expenses')
    }
    return response.json()
  },

  async create(jobId: number, data: ExpenseRequestDto): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/expenses`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(submitData),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create expense' }))
      throw new Error(errorData.message || 'Failed to create expense')
    }
    return response.json()
  },

  async update(id: number, data: ExpenseRequestDto): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    
    const response = await fetch(`${API_BASE_URL}/expenses/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(submitData),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update expense' }))
      throw new Error(errorData.message || 'Failed to update expense')
    }
    return response.json()
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/expenses/${id}`, {
      method: 'DELETE',
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete expense')
    }
  },
}
