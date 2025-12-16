import { describe, it, expect } from 'vitest'
import { jobUpdateSchema } from './jobUpdate.schema'

describe('jobUpdateSchema', () => {
  describe('updateText field', () => {
    it('should accept valid update text', () => {
      const result = jobUpdateSchema.safeParse({
        updateText: 'Replaced spark plug and cleaned carburetor',
      })
      expect(result.success).toBe(true)
    })

    it('should reject empty updateText', () => {
      const result = jobUpdateSchema.safeParse({
        updateText: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Update text is required')
      }
    })

    it('should reject updateText exceeding 4000 characters', () => {
      const result = jobUpdateSchema.safeParse({
        updateText: 'a'.repeat(4001),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Update text cannot exceed 4000 characters')
      }
    })

    it('should accept updateText at max length (4000 chars)', () => {
      const result = jobUpdateSchema.safeParse({
        updateText: 'a'.repeat(4000),
      })
      expect(result.success).toBe(true)
    })
  })
})
