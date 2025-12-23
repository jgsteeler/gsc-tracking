import { useState, useEffect, useCallback } from 'react'
import { jobUpdateService } from '@/services/jobUpdateService'
import type { JobUpdate, JobUpdateRequestDto } from '@/types/job'
import { useAccessToken } from '@/hooks/useAccessToken'

export function useJobUpdates(jobId: number) {
  const [updates, setUpdates] = useState<JobUpdate[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { getToken } = useAccessToken()

  const fetchUpdates = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      const token = await getToken()
      const data = await jobUpdateService.getAll(jobId, token)
      setUpdates(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred')
    } finally {
      setLoading(false)
    }
  }, [jobId, getToken])

  useEffect(() => {
    fetchUpdates()
  }, [fetchUpdates])

  const createUpdate = async (data: JobUpdateRequestDto) => {
    const token = await getToken()
    const newUpdate = await jobUpdateService.create(jobId, data, token)
    setUpdates(prev => [newUpdate, ...prev])
    return newUpdate
  }

  const deleteUpdate = async (id: number) => {
    const token = await getToken()
    await jobUpdateService.delete(jobId, id, token)
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
