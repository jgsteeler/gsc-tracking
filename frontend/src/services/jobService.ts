import type { Job, JobRequestDto } from '@/types/job'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

export const jobService = {
  async getAll(search?: string, status?: string): Promise<Job[]> {
    const url = new URL(`${API_BASE_URL}/jobs`)
    if (search) {
      url.searchParams.append('search', search)
    }
    if (status) {
      url.searchParams.append('status', status)
    }
    
    const response = await fetch(url.toString())
    if (!response.ok) {
      throw new Error('Failed to fetch jobs')
    }
    return response.json()
  },

  async getById(id: number): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`)
    if (!response.ok) {
      throw new Error('Failed to fetch job')
    }
    return response.json()
  },

  async getByCustomerId(customerId: number): Promise<Job[]> {
    const response = await fetch(`${API_BASE_URL}/jobs/customer/${customerId}`)
    if (!response.ok) {
      throw new Error('Failed to fetch customer jobs')
    }
    return response.json()
  },

  async create(data: JobRequestDto): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to create job' }))
      throw new Error(errorData.message || 'Failed to create job')
    }
    return response.json()
  },

  async update(id: number, data: JobRequestDto): Promise<Job> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to update job' }))
      throw new Error(errorData.message || 'Failed to update job')
    }
    return response.json()
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/jobs/${id}`, {
      method: 'DELETE',
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete job')
    }
  },
}
