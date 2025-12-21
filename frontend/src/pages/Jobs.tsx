import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Badge } from '@/components/ui/badge'
import { Plus, Pencil, Trash2, Search, Eye } from 'lucide-react'
import { useJobs } from '@/hooks/useJobs'
import { useUserRole } from '@/hooks/useUserRole'
import type { Job } from '@/types/job'
import { JOB_STATUS_LABELS, JOB_STATUS_COLORS, JOB_STATUSES } from '@/types/job'
import { JobDialog } from '@/components/JobDialog'
import { DeleteConfirmDialog } from '@/components/DeleteConfirmDialog'
import type { JobFormValues } from '@/lib/validations'
import { useToast } from '@/hooks/use-toast'

export default function Jobs() {
  const navigate = useNavigate()
  const [searchTerm, setSearchTerm] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [selectedJob, setSelectedJob] = useState<Job | null>(null)
  const [jobToDelete, setJobToDelete] = useState<Job | null>(null)
  
  const { jobs, loading, error, createJob, updateJob, deleteJob } = useJobs(debouncedSearch, statusFilter || undefined)
  const { toast } = useToast()
  const { isAdmin } = useUserRole()

  const handleSearch = (value: string) => {
    setSearchTerm(value)
  }
  
  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchTerm)
    }, 300)
    return () => clearTimeout(timer)
  }, [searchTerm])

  const handleAddJob = () => {
    setSelectedJob(null)
    setDialogOpen(true)
  }

  const handleEditJob = (job: Job) => {
    setSelectedJob(job)
    setDialogOpen(true)
  }

  const handleDeleteClick = (job: Job) => {
    setJobToDelete(job)
    setDeleteDialogOpen(true)
  }

  const handleSubmit = async (data: JobFormValues) => {
    try {
      if (selectedJob) {
        await updateJob(selectedJob.id, data)
        toast({
          title: 'Success',
          description: 'Job updated successfully',
        })
      } else {
        await createJob(data)
        toast({
          title: 'Success',
          description: 'Job created successfully',
        })
      }
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'An error occurred',
        variant: 'destructive',
      })
      throw err
    }
  }

  const handleDeleteConfirm = async () => {
    if (!jobToDelete) return
    
    try {
      await deleteJob(jobToDelete.id)
      toast({
        title: 'Success',
        description: 'Job deleted successfully',
      })
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to delete job',
        variant: 'destructive',
      })
    }
  }

  const formatCurrency = (amount?: number | null) => {
    if (amount == null) return '-'
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount)
  }

  const formatDate = (dateString?: string | null) => {
    if (!dateString) return '-'
    return new Date(dateString).toLocaleDateString()
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Jobs</h2>
          <p className="text-muted-foreground">
            Track and manage repair jobs and service requests
          </p>
        </div>
        {isAdmin && (
          <Button onClick={handleAddJob}>
            <Plus className="mr-2 h-4 w-4" />
            Create Job
          </Button>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Job List</CardTitle>
          <CardDescription>
            View all jobs and their current status
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col md:flex-row gap-4 mb-6">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search jobs by equipment, customer, or description..."
                value={searchTerm}
                onChange={(e) => handleSearch(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-full md:w-[200px]">
                <SelectValue placeholder="Filter by status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value=" ">All Statuses</SelectItem>
                {JOB_STATUSES.map((status) => (
                  <SelectItem key={status} value={status}>
                    {JOB_STATUS_LABELS[status]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {error && (
            <div className="text-center py-4 text-red-500">
              Error: {error}
            </div>
          )}

          {loading ? (
            <div className="text-center py-8">Loading jobs...</div>
          ) : jobs.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No jobs found. Create your first job to get started.
            </div>
          ) : (
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Customer</TableHead>
                    <TableHead>Equipment</TableHead>
                    <TableHead>Description</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Date Received</TableHead>
                    <TableHead>Estimate</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {jobs.map((job) => (
                    <TableRow key={job.id}>
                      <TableCell className="font-medium">{job.customerName}</TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{job.equipmentType}</div>
                          <div className="text-sm text-muted-foreground">{job.equipmentModel}</div>
                        </div>
                      </TableCell>
                      <TableCell className="max-w-xs truncate">{job.description}</TableCell>
                      <TableCell>
                        <Badge variant="outline" className={JOB_STATUS_COLORS[job.status]}>
                          {JOB_STATUS_LABELS[job.status]}
                        </Badge>
                      </TableCell>
                      <TableCell>{formatDate(job.dateReceived)}</TableCell>
                      <TableCell>{formatCurrency(job.estimateAmount)}</TableCell>
                      <TableCell className="text-right">
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => navigate(`/jobs/${job.id}`)}
                          className="mr-2"
                        >
                          <Eye className="h-4 w-4" />
                        </Button>
                        {isAdmin && (
                          <>
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => handleEditJob(job)}
                              className="mr-2"
                            >
                              <Pencil className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => handleDeleteClick(job)}
                            >
                              <Trash2 className="h-4 w-4 text-red-500" />
                            </Button>
                          </>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>

      <JobDialog
        job={selectedJob}
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={handleSubmit}
      />

      <DeleteConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        onConfirm={handleDeleteConfirm}
        title="Delete Job"
        description={`Are you sure you want to delete the job for "${jobToDelete?.equipmentType} - ${jobToDelete?.equipmentModel}"? This action cannot be undone.`}
      />
    </div>
  )
}
