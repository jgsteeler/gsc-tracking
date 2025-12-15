import { useState, useEffect, useCallback } from 'react'
import { jobUpdateService } from '@/services/jobUpdateService'
import type { JobUpdate, JobUpdateRequestDto } from '@/types/job'

export function useJobUpdates(jobId: number) {
  const [updates, setUpdates] = useState<JobUpdate[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchUpdates = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      const data = await jobUpdateService.getAll(jobId)
      setUpdates(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred')
    } finally {
      setLoading(false)
    }
  }, [jobId])

  useEffect(() => {
    fetchUpdates()
  }, [fetchUpdates])

  const createUpdate = async (data: JobUpdateRequestDto) => {
    const newUpdate = await jobUpdateService.create(jobId, data)
    setUpdates(prev => [newUpdate, ...prev])
    return newUpdate
  }

  const deleteUpdate = async (id: number) => {
    await jobUpdateService.delete(jobId, id)
    setUpdates(prev => prev.filter(update => update.id !== id))
  }

  return {
    updates,
    loading,
    error,
    createUpdate,
    deleteUpdate,
    refetch: fetchUpdates,
  }
}
