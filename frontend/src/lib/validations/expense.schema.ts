import { z } from 'zod'

/**
 * Zod schema for expense validation
 * Matches backend FluentValidation rules in ExpenseRequestDtoValidator
 */
export const expenseSchema = z.object({
  type: z.enum(['Parts', 'Labor', 'Service'], {
    message: 'Expense type must be one of: Parts, Labor, Service'
  }),
  description: z
    .string()
    .min(1, 'Description is required')
    .max(500, 'Description cannot exceed 500 characters'),
  amount: z
    .number()
    .min(0.01, 'Amount must be greater than 0'),
  date: z
    .string()
    .min(1, 'Date is required'),
  receiptReference: z
    .string()
    .max(200, 'Receipt reference cannot exceed 200 characters')
    .optional()
    .or(z.literal('')),
})

export type ExpenseFormValues = z.infer<typeof expenseSchema>
