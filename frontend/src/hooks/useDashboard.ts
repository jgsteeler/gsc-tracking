import { useState, useEffect, useCallback } from 'react'
import type { Customer } from '@/types/customer'
import type { Job } from '@/types/job'
import { customerService } from '@/services/customerService'
import { jobService } from '@/services/jobService'

export interface DashboardStats {
  totalCustomers: number
  activeJobs: number
  revenue: number
  recentJobs: Job[]
  recentCustomers: Customer[]
}

export function useDashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalCustomers: 0,
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
      const activeJobs = jobs.filter(job => job.status === 'InProgress').length
      
      // Calculate revenue from completed, invoiced, or paid jobs
      const currentMonth = new Date().getMonth()
      const currentYear = new Date().getFullYear()
      
      const revenue = jobs
        .filter(job => {
          const jobDate = new Date(job.dateCompleted || job.createdAt)
          const isCurrentMonth = jobDate.getMonth() === currentMonth && jobDate.getFullYear() === currentYear
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
