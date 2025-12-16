import { z } from 'zod'

/**
 * Zod schema for job validation
 * Matches backend FluentValidation rules in JobRequestDtoValidator
 */
export const jobSchema = z.object({
  customerId: z
    .number()
    .min(1, 'Customer is required'),
  equipmentType: z
    .string()
    .min(1, 'Equipment type is required')
    .max(200, 'Equipment type cannot exceed 200 characters'),
  equipmentModel: z
    .string()
    .min(1, 'Equipment model is required')
    .max(200, 'Equipment model cannot exceed 200 characters'),
  description: z
    .string()
    .min(1, 'Description is required')
    .max(2000, 'Description cannot exceed 2000 characters'),
  status: z.enum(['Quote', 'InProgress', 'Completed', 'Invoiced', 'Paid'], {
    message: 'Status must be one of: Quote, InProgress, Completed, Invoiced, Paid'
  }),
  dateReceived: z
    .string()
    .min(1, 'Date received is required'),
  dateCompleted: z
    .string()
    .optional()
    .nullable(),
  estimateAmount: z
    .number()
    .min(0, 'Estimate amount must be 0 or greater')
    .optional()
    .nullable(),
  actualAmount: z
    .number()
    .min(0, 'Actual amount must be 0 or greater')
    .optional()
    .nullable(),
}).refine(
  (data) => {
    // Validate that dateCompleted is on or after dateReceived
    if (data.dateCompleted && data.dateReceived) {
      const received = new Date(data.dateReceived)
      const completed = new Date(data.dateCompleted)
      return completed >= received
    }
    return true
  },
  {
    message: 'Date completed must be on or after date received',
    path: ['dateCompleted'],
  }
)

export type JobFormValues = z.infer<typeof jobSchema>
