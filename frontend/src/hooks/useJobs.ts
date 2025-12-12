import { useState, useEffect, useCallback } from 'react'
import type { Job, JobRequestDto } from '@/types/job'
import { jobService } from '@/services/jobService'

export function useJobs(searchTerm?: string, statusFilter?: string) {
  const [jobs, setJobs] = useState<Job[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchJobs = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      const data = await jobService.getAll(searchTerm, statusFilter)
      setJobs(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch jobs')
    } finally {
      setLoading(false)
    }
  }, [searchTerm, statusFilter])

  useEffect(() => {
    fetchJobs()
  }, [fetchJobs])

  const createJob = async (data: JobRequestDto): Promise<Job> => {
    const newJob = await jobService.create(data)
    setJobs((prev) => [newJob, ...prev])
    return newJob
  }

  const updateJob = async (id: number, data: JobRequestDto): Promise<Job> => {
    const updatedJob = await jobService.update(id, data)
    setJobs((prev) =>
      prev.map((job) => (job.id === id ? updatedJob : job))
    )
    return updatedJob
  }

  const deleteJob = async (id: number): Promise<void> => {
    await jobService.delete(id)
    setJobs((prev) => prev.filter((job) => job.id !== id))
  }

  return {
    jobs,
    loading,
    error,
    refetch: fetchJobs,
    createJob,
    updateJob,
    deleteJob,
  }
}
