import type { Job, JobRequestDto } from '@/types/job'
import { apiClient } from '@/lib/api-client'

export const jobService = {
  async getAll(search?: string, status?: string, token?: string | null): Promise<Job[]> {
    let url = '/jobs'
    const params: string[] = []
    if (search) {
      params.push(`search=${encodeURIComponent(search)}`)
    }
    if (status) {
      params.push(`status=${encodeURIComponent(status)}`)
    }
    if (params.length > 0) {
      url += `?${params.join('&')}`
    }
    return apiClient.get<Job[]>(url, token)
  },

  async getById(id: number, token?: string | null): Promise<Job> {
    return apiClient.get<Job>(`/jobs/${id}`, token)
  },

  async getByCustomerId(customerId: number, token?: string | null): Promise<Job[]> {
    return apiClient.get<Job[]>(`/jobs/customer/${customerId}`, token)
  },

  async create(data: JobRequestDto, token?: string | null): Promise<Job> {
    return apiClient.post<Job>('/jobs', data, token)
  },

  async update(id: number, data: JobRequestDto, token?: string | null): Promise<Job> {
    return apiClient.put<Job>(`/jobs/${id}`, data, token)
  },

  async delete(id: number, token?: string | null): Promise<void> {
    return apiClient.delete(`/jobs/${id}`, token)
  },
}
