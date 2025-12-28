import type { Expense, ExpenseRequestDto } from '@/types/expense'
import { formatDateForApi } from '@/lib/utils'
import { apiClient } from '@/lib/api-client'

export const expenseService = {
  async getByJobId(jobId: number, token?: string | null): Promise<Expense[]> {
    return apiClient.get<Expense[]>(`/jobs/${jobId}/expenses`, token)
  },

  async create(jobId: number, data: ExpenseRequestDto, token?: string | null): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    return apiClient.post<Expense>(`/jobs/${jobId}/expenses`, submitData, token)
  },

  async update(jobId: number, id: number, data: ExpenseRequestDto, token?: string | null): Promise<Expense> {
    // Ensure date is in proper format for backend
    const submitData = {
      ...data,
      date: formatDateForApi(data.date)
    }
    return apiClient.put<Expense>(`/jobs/${jobId}/expenses/${id}`, submitData, token)
  },

  async delete(jobId: number, id: number, token?: string | null): Promise<void> {
    return apiClient.delete(`/jobs/${jobId}/expenses/${id}`, token)
  },
}
