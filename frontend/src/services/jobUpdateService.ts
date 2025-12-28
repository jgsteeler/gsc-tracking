import type { JobUpdate, JobUpdateRequestDto } from '@/types/job'
import { apiClient } from '@/lib/api-client'

export const jobUpdateService = {
  async getAll(jobId: number, token?: string | null): Promise<JobUpdate[]> {
    return apiClient.get<JobUpdate[]>(`/jobs/${jobId}/updates`, token)
  },

  async getById(jobId: number, id: number, token?: string | null): Promise<JobUpdate> {
    return apiClient.get<JobUpdate>(`/jobs/${jobId}/updates/${id}`, token)
  },

  async create(jobId: number, data: JobUpdateRequestDto, token?: string | null): Promise<JobUpdate> {
    return apiClient.post<JobUpdate>(`/jobs/${jobId}/updates`, data, token)
  },

  async delete(jobId: number, id: number, token?: string | null): Promise<void> {
    return apiClient.delete(`/jobs/${jobId}/updates/${id}`, token)
  },
}
