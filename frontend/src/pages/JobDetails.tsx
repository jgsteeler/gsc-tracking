import { useParams, useNavigate } from 'react-router-dom'
import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { ArrowLeft, Pencil, Loader2 } from 'lucide-react'
import { jobService } from '@/services/jobService'
import { useUserRole } from '@/hooks/useUserRole'
import type { Job } from '@/types/job'
import { JOB_STATUS_LABELS, JOB_STATUS_COLORS } from '@/types/job'
import { JobDialog } from '@/components/JobDialog'
import type { JobFormValues } from '@/lib/validations'
import { useToast } from '@/hooks/use-toast'
import { JobUpdates } from '@/components/JobUpdates'
import { ExpenseList } from '@/components/ExpenseList'

export default function JobDetails() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { isAdmin } = useUserRole()
  const [job, setJob] = useState<Job | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)

  useEffect(() => {
    const fetchJob = async () => {
      if (!id) return

      try {
        setLoading(true)
        const data = await jobService.getById(parseInt(id))
        setJob(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load job')
      } finally {
        setLoading(false)
      }
    }

    fetchJob()
  }, [id])

  const handleUpdate = async (data: JobFormValues) => {
    if (!job) return

    try {
      const updatedJob = await jobService.update(job.id, data)
      setJob(updatedJob)
      toast({
        title: 'Success',
        description: 'Job updated successfully',
      })
      setDialogOpen(false)
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to update job',
        variant: 'destructive',
      })
    }
  }

  const handleExpenseChange = async () => {
    // Refresh job data to get updated totals
    if (!id) return
    try {
      const data = await jobService.getById(parseInt(id))
      setJob(data)
    } catch (err) {
      console.error('Failed to refresh job data:', err)
    }
  }

  const formatCurrency = (amount?: number | null) => {
    if (amount === null || amount === undefined) return 'N/A'
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount)
  }

  const formatDate = (dateString: string) => {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    }).format(new Date(dateString))
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !job) {
    return (
      <div className="space-y-4">
        <Button variant="ghost" onClick={() => navigate('/jobs')}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to Jobs
        </Button>
        <Card>
          <CardContent className="pt-6">
            <div className="text-red-500">
              {error || 'Job not found'}
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/jobs')}>
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Jobs
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Job Details</h1>
            <p className="text-muted-foreground">
              Job #{job.id} - {job.customerName}
            </p>
          </div>
        </div>
        {isAdmin && (
          <Button onClick={() => setDialogOpen(true)}>
            <Pencil className="mr-2 h-4 w-4" />
            Edit Job
          </Button>
        )}
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {/* Job Information */}
        <Card>
          <CardHeader>
            <CardTitle>Job Information</CardTitle>
            <CardDescription>Details about the job</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <label className="text-sm font-medium text-muted-foreground">Status</label>
              <div className="mt-1">
                <Badge className={JOB_STATUS_COLORS[job.status]}>
                  {JOB_STATUS_LABELS[job.status]}
                </Badge>
              </div>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Customer</label>
              <p className="mt-1 text-sm">{job.customerName}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Equipment Type</label>
              <p className="mt-1 text-sm">{job.equipmentType}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Equipment Model</label>
              <p className="mt-1 text-sm">{job.equipmentModel}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Description</label>
              <p className="mt-1 text-sm whitespace-pre-wrap">{job.description}</p>
            </div>
          </CardContent>
        </Card>

        {/* Dates and Amounts */}
        <Card>
          <CardHeader>
            <CardTitle>Dates & Amounts</CardTitle>
            <CardDescription>Timeline and financial information</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <label className="text-sm font-medium text-muted-foreground">Date Received</label>
              <p className="mt-1 text-sm">{formatDate(job.dateReceived)}</p>
            </div>

            {job.dateCompleted && (
              <div>
                <label className="text-sm font-medium text-muted-foreground">Date Completed</label>
                <p className="mt-1 text-sm">{formatDate(job.dateCompleted)}</p>
              </div>
            )}

            <div className="pt-4 border-t">
              <label className="text-sm font-medium text-muted-foreground">Estimate Amount</label>
              <p className="mt-1 text-sm font-semibold">{formatCurrency(job.estimateAmount)}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Actual Amount</label>
              <p className="mt-1 text-sm font-semibold">{formatCurrency(job.actualAmount)}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Total Cost</label>
              <p className="mt-1 text-sm font-semibold">{formatCurrency(job.totalCost)}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Profit Margin</label>
              <p className={`mt-1 text-sm font-semibold ${
                job.profitMargin !== null && job.profitMargin !== undefined
                  ? job.profitMargin < 0
                    ? 'text-red-600'
                    : job.profitMargin === 0
                    ? 'text-gray-600'
                    : 'text-green-600'
                  : ''
              }`}>
                {formatCurrency(job.profitMargin)}
              </p>
            </div>

            <div className="pt-4 border-t">
              <label className="text-sm font-medium text-muted-foreground">Created At</label>
              <p className="mt-1 text-sm">{formatDate(job.createdAt)}</p>
            </div>

            <div>
              <label className="text-sm font-medium text-muted-foreground">Last Updated</label>
              <p className="mt-1 text-sm">{formatDate(job.updatedAt)}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Expenses Section */}
      <ExpenseList jobId={job.id} onExpenseChange={handleExpenseChange} />

      {/* Job Updates Section */}
      <JobUpdates jobId={job.id} />

      <JobDialog
        job={job}
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={handleUpdate}
      />
    </div>
  )
}
