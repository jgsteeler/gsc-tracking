import { z } from 'zod'

/**
 * Zod schema for job update validation
 * Matches backend FluentValidation rules in JobUpdateRequestDtoValidator
 */
export const jobUpdateSchema = z.object({
  updateText: z
    .string()
    .min(1, 'Update text is required')
    .max(4000, 'Update text cannot exceed 4000 characters'),
})

export type JobUpdateFormValues = z.infer<typeof jobUpdateSchema>
