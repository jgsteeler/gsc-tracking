import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { Button } from '@/components/ui/button'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import type { Job } from '@/types/job'
import { JOB_STATUSES, JOB_STATUS_LABELS } from '@/types/job'
import { useCustomers } from '@/hooks/useCustomers'

const jobSchema = z.object({
  customerId: z.number().min(1, 'Customer is required'),
  equipmentType: z.string().min(1, 'Equipment type is required').max(200, 'Equipment type cannot exceed 200 characters'),
  equipmentModel: z.string().min(1, 'Equipment model is required').max(200, 'Equipment model cannot exceed 200 characters'),
  description: z.string().min(1, 'Description is required').max(2000, 'Description cannot exceed 2000 characters'),
  status: z.enum(['Quote', 'InProgress', 'Completed', 'Invoiced', 'Paid']),
  dateReceived: z.string().min(1, 'Date received is required'),
  dateCompleted: z.string().optional().nullable(),
  estimateAmount: z.number().min(0, 'Estimate amount must be 0 or greater').optional().nullable(),
  actualAmount: z.number().min(0, 'Actual amount must be 0 or greater').optional().nullable(),
})

export type JobFormValues = z.infer<typeof jobSchema>

interface JobFormProps {
  job?: Job
  onSubmit: (data: JobFormValues) => Promise<void>
  onCancel?: () => void
}

export function JobForm({ job, onSubmit, onCancel }: JobFormProps) {
  const { customers } = useCustomers()
  
  const form = useForm<JobFormValues>({
    resolver: zodResolver(jobSchema),
    defaultValues: {
      customerId: job?.customerId || 0,
      equipmentType: job?.equipmentType || '',
      equipmentModel: job?.equipmentModel || '',
      description: job?.description || '',
      status: job?.status || 'Quote',
      dateReceived: job?.dateReceived ? new Date(job.dateReceived).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
      dateCompleted: job?.dateCompleted ? new Date(job.dateCompleted).toISOString().split('T')[0] : null,
      estimateAmount: job?.estimateAmount ?? null,
      actualAmount: job?.actualAmount ?? null,
    },
  })

  const handleSubmit = async (data: JobFormValues) => {
    try {
      // Convert date strings to ISO format for API
      const submitData = {
        ...data,
        dateReceived: new Date(data.dateReceived).toISOString(),
        dateCompleted: data.dateCompleted ? new Date(data.dateCompleted).toISOString() : null,
      }
      await onSubmit(submitData)
      form.reset()
    } catch (error) {
      // Error handling is done in parent component
      console.error('Form submission error:', error)
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
        <FormField
          control={form.control}
          name="customerId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Customer</FormLabel>
              <Select
                onValueChange={(value: string) => field.onChange(Number(value))}
                defaultValue={field.value ? field.value.toString() : ''}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Select a customer" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {customers.map((customer) => (
                    <SelectItem key={customer.id} value={customer.id.toString()}>
                      {customer.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name="equipmentType"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Equipment Type</FormLabel>
                <FormControl>
                  <Input placeholder="Lawn Mower" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="equipmentModel"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Equipment Model</FormLabel>
                <FormControl>
                  <Input placeholder="Honda HRX217" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="description"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Description</FormLabel>
              <FormControl>
                <textarea
                  placeholder="Describe the work to be done..."
                  className="flex min-h-[80px] w-full rounded-md border border-gray-300 bg-white text-gray-900 px-3 py-2 text-sm ring-offset-background placeholder:text-gray-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name="status"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Status</FormLabel>
                <Select onValueChange={field.onChange} defaultValue={field.value}>
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Select a status" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {JOB_STATUSES.map((status) => (
                      <SelectItem key={status} value={status}>
                        {JOB_STATUS_LABELS[status]}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="dateReceived"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Date Received</FormLabel>
                <FormControl>
                  <Input type="date" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="dateCompleted"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Date Completed (Optional)</FormLabel>
              <FormControl>
                <Input
                  type="date"
                  {...field}
                  value={field.value || ''}
                  onChange={(e) => field.onChange(e.target.value || null)}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name="estimateAmount"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Estimate Amount (Optional)</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    step="0.01"
                    min="0"
                    placeholder="0.00"
                    {...field}
                    value={field.value ?? ''}
                    onChange={(e) => field.onChange(e.target.value ? parseFloat(e.target.value) : null)}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="actualAmount"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Actual Amount (Optional)</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    step="0.01"
                    min="0"
                    placeholder="0.00"
                    {...field}
                    value={field.value ?? ''}
                    onChange={(e) => field.onChange(e.target.value ? parseFloat(e.target.value) : null)}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="flex justify-end space-x-2">
          {onCancel && (
            <Button type="button" variant="outline" onClick={onCancel}>
              Cancel
            </Button>
          )}
          <Button type="submit" disabled={form.formState.isSubmitting}>
            {form.formState.isSubmitting ? 'Saving...' : job ? 'Update' : 'Create'}
          </Button>
        </div>
      </form>
    </Form>
  )
}
