import type { Job, JobRequestDto } from '@/types/job'

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

export const jobService = {
  async getAll(search?: string, status?: string, token?: string | null): Promise<Job[]> {
    const url = new URL(`${API_BASE_URL}/jobs`)
    if (search) {
      url.searchParams.append('search', search)
    }
    if (status) {
      url.searchParams.append('status', status)
    }
    
    const response = await fetch(url.toString(), {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch jobs')
    }
    return response.json()
  },

  async getById(id: number, token?: string | null): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch job')
    }
    return response.json()
  },

  async getByCustomerId(customerId: number, token?: string | null): Promise<Job[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/customer/${customerId}`, {
      headers: createHeaders(token),
    })
    if (!response.ok) {
      throw new Error('Failed to fetch customer jobs')
    }
    return response.json()
  },

  async create(data: JobRequestDto, token?: string | null): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs`, {
      method: 'POST',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create job' }))
      throw new Error(errorData.message || 'Failed to create job')
    }
    return response.json()
  },

  async update(id: number, data: JobRequestDto, token?: string | null): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`, {
      method: 'PUT',
      headers: createHeaders(token),
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update job' }))
      throw new Error(errorData.message || 'Failed to update job')
    }
    return response.json()
  },

  async delete(id: number, token?: string | null): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`, {
      method: 'DELETE',
      headers: createHeaders(token),
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete job')
    }
  },
}
