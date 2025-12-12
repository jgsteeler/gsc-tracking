import type { Job } from '@/types/job'
import { JobForm, type JobFormValues } from './JobForm'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'

interface JobDialogProps {
  job?: Job | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (data: JobFormValues) => Promise<void>
}

export function JobDialog({
  job,
  open,
  onOpenChange,
  onSubmit,
}: JobDialogProps) {
  const handleSubmit = async (data: JobFormValues) => {
    await onSubmit(data)
    onOpenChange(false)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[700px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{job ? 'Edit Job' : 'Create New Job'}</DialogTitle>
          <DialogDescription>
            {job
              ? 'Update the job information below.'
              : 'Fill in the job information below.'}
          </DialogDescription>
        </DialogHeader>
        <JobForm
          job={job || undefined}
          onSubmit={handleSubmit}
          onCancel={() => onOpenChange(false)}
        />
      </DialogContent>
    </Dialog>
  )
}
