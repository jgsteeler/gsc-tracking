import { useState, useEffect, useCallback } from 'react'
import type { Customer } from '@/types/customer'
import type { Job } from '@/types/job'
import { customerService } from '@/services/customerService'
import { jobService } from '@/services/jobService'

export interface DashboardStats {
  totalCustomers: number
  totalJobs: number
  activeJobs: number
  revenue: number
  recentJobs: Job[]
  recentCustomers: Customer[]
}

export function useDashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalCustomers: 0,
    totalJobs: 0,
    activeJobs: 0,
    revenue: 0,
    recentJobs: [],
    recentCustomers: [],
  })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchDashboardData = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)

      // Fetch all data in parallel
      const [customers, jobs] = await Promise.all([
        customerService.getAll(),
        jobService.getAll(),
      ])

      // Calculate statistics
      const totalCustomers = customers.length
      const totalJobs = jobs.length
      const activeJobs = jobs.filter(job => job.status === 'InProgress').length
      
      // Calculate revenue from completed, invoiced, or paid jobs in the current month
      // Only use dateCompleted for revenue calculation to ensure job is actually finished
      const currentMonth = new Date().getMonth()
      const currentYear = new Date().getFullYear()
      
      const revenue = jobs
        .filter(job => {
          // Only include jobs that have been completed
          if (!job.dateCompleted) return false
          
          const completedDate = new Date(job.dateCompleted)
          const isCurrentMonth = completedDate.getMonth() === currentMonth && completedDate.getFullYear() === currentYear
          const isPaid = ['Completed', 'Invoiced', 'Paid'].includes(job.status)
          return isCurrentMonth && isPaid
        })
        .reduce((sum, job) => sum + (job.actualAmount || job.estimateAmount || 0), 0)

      // Get recent jobs (last 5, sorted by creation date)
      const recentJobs = [...jobs]
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        .slice(0, 5)

      // Get recent customers (last 5, sorted by creation date)
      const recentCustomers = [...customers]
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        .slice(0, 5)

      setStats({
        totalCustomers,
        totalJobs,
        activeJobs,
        revenue,
        recentJobs,
        recentCustomers,
      })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch dashboard data')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchDashboardData()
  }, [fetchDashboardData])

  return {
    stats,
    loading,
    error,
    refetch: fetchDashboardData,
  }
}
