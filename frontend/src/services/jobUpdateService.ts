import type { JobUpdate, JobUpdateRequestDto } from '@/types/job'

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

export const jobUpdateService = {
  async getAll(jobId: number, token?: string | null): Promise<JobUpdate[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch job updates')
    }
    return response.json()
  },

  async getById(jobId: number, id: number, token?: string | null): Promise<JobUpdate> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates/${id}`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch job update')
    }
    return response.json()
  },

  async create(jobId: number, data: JobUpdateRequestDto, token?: string | null): Promise<JobUpdate> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates`, {
      method: 'POST',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create job update' }))
      throw new Error(errorData.message || 'Failed to create job update')
    }
    return response.json()
  },

  async delete(jobId: number, id: number, token?: string | null): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/jobs/${jobId}/updates/${id}`, {
      method: 'DELETE',
      headers: createHeaders(token),
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete job update')
    }
  },
}
