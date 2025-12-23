import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { jobService } from './jobService'
import type { Job, JobRequestDto } from '@/types/job'

const API_BASE_URL = 'http://localhost:5091/api'

describe('jobService', () => {
  beforeEach(() => {
    global.fetch = vi.fn()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('getAll', () => {
    it('should fetch all jobs without filters', async () => {
      const mockJobs: Job[] = [
        {
          id: 1,
          customerId: 1,
          customerName: 'John Doe',
          equipmentType: 'Lawn Mower',
          equipmentModel: 'Honda HRX217',
          description: 'Oil change',
          status: 'Quote',
          dateReceived: '2024-01-01T00:00:00Z',
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      ]

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJobs,
      } as Response)

      const result = await jobService.getAll()

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs`, expect.objectContaining({
        headers: expect.any(Object)
      }))
      expect(result).toEqual(mockJobs)
    })

    it('should fetch jobs with search parameter', async () => {
      const mockJobs: Job[] = []
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJobs,
      } as Response)

      await jobService.getAll('lawn')

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs?search=lawn`, expect.objectContaining({
        headers: expect.any(Object)
      }))
    })

    it('should fetch jobs with status filter', async () => {
      const mockJobs: Job[] = []
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJobs,
      } as Response)

      await jobService.getAll(undefined, 'InProgress')

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs?status=InProgress`, expect.objectContaining({
        headers: expect.any(Object)
      }))
    })

    it('should fetch jobs with both search and status filter', async () => {
      const mockJobs: Job[] = []
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJobs,
      } as Response)

      await jobService.getAll('lawn', 'Quote')

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs?search=lawn&status=Quote`, expect.objectContaining({
        headers: expect.any(Object)
      }))
    })

    it('should throw error when fetch fails', async () => {
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: false,
      } as Response)

      await expect(jobService.getAll()).rejects.toThrow('Failed to fetch jobs')
    })
  })

  describe('getById', () => {
    it('should fetch a job by id', async () => {
      const mockJob: Job = {
        id: 1,
        customerId: 1,
        customerName: 'John Doe',
        equipmentType: 'Lawn Mower',
        equipmentModel: 'Honda HRX217',
        description: 'Oil change',
        status: 'Quote',
        dateReceived: '2024-01-01T00:00:00Z',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJob,
      } as Response)

      const result = await jobService.getById(1)

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs/1`, expect.objectContaining({
        headers: expect.any(Object)
      }))
      expect(result).toEqual(mockJob)
    })

    it('should throw error when job not found', async () => {
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: false,
      } as Response)

      await expect(jobService.getById(999)).rejects.toThrow('Failed to fetch job')
    })
  })

  describe('getByCustomerId', () => {
    it('should fetch jobs for a specific customer', async () => {
      const mockJobs: Job[] = [
        {
          id: 1,
          customerId: 1,
          customerName: 'John Doe',
          equipmentType: 'Lawn Mower',
          equipmentModel: 'Honda HRX217',
          description: 'Oil change',
          status: 'Quote',
          dateReceived: '2024-01-01T00:00:00Z',
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      ]

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJobs,
      } as Response)

      const result = await jobService.getByCustomerId(1)

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs/customer/1`, expect.objectContaining({
        headers: expect.any(Object)
      }))
      expect(result).toEqual(mockJobs)
    })
  })

  describe('create', () => {
    it('should create a new job', async () => {
      const jobRequest: JobRequestDto = {
        customerId: 1,
        equipmentType: 'Lawn Mower',
        equipmentModel: 'Honda HRX217',
        description: 'Oil change',
        status: 'Quote',
        dateReceived: '2024-01-01T00:00:00Z'
      }

      const mockJob: Job = {
        id: 1,
        ...jobRequest,
        customerName: 'John Doe',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJob,
      } as Response)

      const result = await jobService.create(jobRequest)

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(jobRequest),
      })
      expect(result).toEqual(mockJob)
    })

    it('should throw error when creation fails', async () => {
      const jobRequest: JobRequestDto = {
        customerId: 1,
        equipmentType: 'Lawn Mower',
        equipmentModel: 'Honda HRX217',
        description: 'Oil change',
        status: 'Quote',
        dateReceived: '2024-01-01T00:00:00Z'
      }

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: false,
        json: async () => ({ message: 'Validation error' }),
      } as Response)

      await expect(jobService.create(jobRequest)).rejects.toThrow('Validation error')
    })
  })

  describe('update', () => {
    it('should update an existing job', async () => {
      const jobRequest: JobRequestDto = {
        customerId: 1,
        equipmentType: 'Lawn Mower',
        equipmentModel: 'Honda HRX217',
        description: 'Oil change and blade sharpening',
        status: 'InProgress',
        dateReceived: '2024-01-01T00:00:00Z'
      }

      const mockJob: Job = {
        id: 1,
        ...jobRequest,
        customerName: 'John Doe',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      }

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
        json: async () => mockJob,
      } as Response)

      const result = await jobService.update(1, jobRequest)

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs/1`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(jobRequest),
      })
      expect(result).toEqual(mockJob)
    })

    it('should throw error when update fails', async () => {
      const jobRequest: JobRequestDto = {
        customerId: 1,
        equipmentType: 'Lawn Mower',
        equipmentModel: 'Honda HRX217',
        description: 'Oil change',
        status: 'Quote',
        dateReceived: '2024-01-01T00:00:00Z'
      }

      vi.mocked(fetch).mockResolvedValueOnce({
        ok: false,
        json: async () => ({ message: 'Job not found' }),
      } as Response)

      await expect(jobService.update(999, jobRequest)).rejects.toThrow('Job not found')
    })
  })

  describe('delete', () => {
    it('should delete a job', async () => {
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: true,
      } as Response)

      await jobService.delete(1)

      expect(fetch).toHaveBeenCalledWith(`${API_BASE_URL}/jobs/1`, expect.objectContaining({
        method: 'DELETE',
        headers: expect.any(Object)
      }))
    })

    it('should throw error when deletion fails', async () => {
      vi.mocked(fetch).mockResolvedValueOnce({
        ok: false,
      } as Response)

      await expect(jobService.delete(999)).rejects.toThrow('Failed to delete job')
    })
  })
})
