import type { JobUpdate, JobUpdateRequestDto } from '@/types/job'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

export const jobUpdateService = {
  async getAll(jobId: number): Promise<JobUpdate[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates`)
    if (!response.ok) {
      throw new Error('Failed to fetch job updates')
    }
    return response.json()
  },

  async getById(jobId: number, id: number): Promise<JobUpdate> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates/${id}`)
    if (!response.ok) {
      throw new Error('Failed to fetch job update')
    }
    return response.json()
  },

  async create(jobId: number, data: JobUpdateRequestDto): Promise<JobUpdate> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create job update' }))
      throw new Error(errorData.message || 'Failed to create job update')
    }
    return response.json()
  },

  async delete(jobId: number, id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates/${id}`, {
      method: 'DELETE',
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete job update')
    }
  },
}
