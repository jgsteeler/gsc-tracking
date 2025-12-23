import { describe, it, expect, beforeEach, vi } from 'vitest'
import { renderHook, waitFor, act } from '@testing-library/react'
import { useJobs } from './useJobs'
import { jobService } from '@/services/jobService'
import type { Job, JobRequestDto } from '@/types/job'

// Mock the job service
vi.mock('@/services/jobService', () => ({
  jobService: {
    getAll: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}))

// Mock useAccessToken
vi.mock('@/hooks/useAccessToken', () => ({
  useAccessToken: () => ({
    getToken: vi.fn().mockResolvedValue(null),
    isAuthEnabled: false,
    isAuthenticated: false,
  }),
}))

describe('useJobs', () => {
  const mockJobs: Job[] = [
    {
      id: 1,
      customerId: 1,
      customerName: 'John Doe',
      equipmentType: 'Lawn Mower',
      equipmentModel: 'Honda HRX217',
      description: 'Oil change',
      status: 'Quote',
      dateReceived: new Date().toISOString(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: 2,
      customerId: 1,
      customerName: 'John Doe',
      equipmentType: 'Chainsaw',
      equipmentModel: 'Stihl MS271',
      description: 'Chain sharpening',
      status: 'InProgress',
      dateReceived: new Date().toISOString(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ]

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch jobs on mount', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs)

    const { result } = renderHook(() => useJobs())

    expect(result.current.loading).toBe(true)
    expect(result.current.jobs).toEqual([])

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.jobs).toEqual(mockJobs)
    expect(result.current.error).toBeNull()
    expect(jobService.getAll).toHaveBeenCalledTimes(1)
  })

  it('should fetch jobs with search term', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce([mockJobs[0]])

    const { result } = renderHook(() => useJobs('lawn'))

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(jobService.getAll).toHaveBeenCalledWith('lawn', undefined, null)
  })

  it('should fetch jobs with status filter', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce([mockJobs[1]])

    const { result } = renderHook(() => useJobs(undefined, 'InProgress'))

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(jobService.getAll).toHaveBeenCalledWith(undefined, 'InProgress', null)
  })

  it('should handle fetch error', async () => {
    const errorMessage = 'Failed to fetch jobs'
    vi.mocked(jobService.getAll).mockRejectedValueOnce(new Error(errorMessage))

    const { result } = renderHook(() => useJobs())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.jobs).toEqual([])
    expect(result.current.error).toBe(errorMessage)
  })

  it('should create a job', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs)
    const newJob: Job = {
      id: 3,
      customerId: 2,
      customerName: 'Jane Smith',
      equipmentType: 'Trimmer',
      equipmentModel: 'Echo SRM-225',
      description: 'New trimmer job',
      status: 'Quote',
      dateReceived: new Date().toISOString(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    }
    vi.mocked(jobService.create).mockResolvedValueOnce(newJob)

    const { result } = renderHook(() => useJobs())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    const jobRequest: JobRequestDto = {
      customerId: 2,
      equipmentType: 'Trimmer',
      equipmentModel: 'Echo SRM-225',
      description: 'New trimmer job',
      status: 'Quote',
      dateReceived: new Date().toISOString(),
    }

    await act(async () => {
      await result.current.createJob(jobRequest)
    })

    expect(jobService.create).toHaveBeenCalledWith(jobRequest)
    expect(result.current.jobs).toHaveLength(3)
    expect(result.current.jobs[0]).toEqual(newJob)
  })

  it('should update a job', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs)
    const updatedJob: Job = {
      ...mockJobs[0],
      description: 'Oil change and blade sharpening',
      status: 'InProgress',
    }
    vi.mocked(jobService.update).mockResolvedValueOnce(updatedJob)

    const { result } = renderHook(() => useJobs())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    const jobRequest: JobRequestDto = {
      customerId: 1,
      equipmentType: 'Lawn Mower',
      equipmentModel: 'Honda HRX217',
      description: 'Oil change and blade sharpening',
      status: 'InProgress',
      dateReceived: new Date().toISOString(),
    }

    await act(async () => {
      await result.current.updateJob(1, jobRequest)
    })

    expect(jobService.update).toHaveBeenCalledWith(1, jobRequest)
    expect(result.current.jobs[0].description).toBe('Oil change and blade sharpening')
    expect(result.current.jobs[0].status).toBe('InProgress')
  })

  it('should delete a job', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs)
    vi.mocked(jobService.delete).mockResolvedValueOnce(undefined)

    const { result } = renderHook(() => useJobs())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.jobs).toHaveLength(2)

    await act(async () => {
      await result.current.deleteJob(1)
    })

    expect(jobService.delete).toHaveBeenCalledWith(1)
    expect(result.current.jobs).toHaveLength(1)
    expect(result.current.jobs[0].id).toBe(2)
  })

  it('should refetch jobs', async () => {
    vi.mocked(jobService.getAll).mockResolvedValueOnce(mockJobs)

    const { result } = renderHook(() => useJobs())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    vi.mocked(jobService.getAll).mockResolvedValueOnce([mockJobs[0]])

    await act(async () => {
      await result.current.refetch()
    })

    expect(jobService.getAll).toHaveBeenCalledTimes(2)
  })
})
