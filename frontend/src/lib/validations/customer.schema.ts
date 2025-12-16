import { z } from 'zod'

/**
 * Zod schema for customer validation
 * Matches backend FluentValidation rules in CustomerRequestDtoValidator
 */
export const customerSchema = z.object({
  name: z
    .string()
    .min(1, 'Name is required')
    .max(200, 'Name cannot exceed 200 characters'),
  email: z
    .string()
    .max(200, 'Email cannot exceed 200 characters')
    .email('Invalid email format')
    .optional()
    .or(z.literal('')),
  phone: z
    .string()
    .max(50, 'Phone cannot exceed 50 characters')
    .optional(),
  address: z
    .string()
    .max(500, 'Address cannot exceed 500 characters')
    .optional(),
  notes: z
    .string()
    .max(2000, 'Notes cannot exceed 2000 characters')
    .optional(),
})

export type CustomerFormValues = z.infer<typeof customerSchema>
