import { describe, it, expect } from 'vitest'
import { jobSchema } from './job.schema'

describe('jobSchema', () => {
  const validJob = {
    customerId: 1,
    equipmentType: 'Lawn Mower',
    equipmentModel: 'Honda HRX217',
    description: 'Needs blade sharpening',
    status: 'Quote' as const,
    dateReceived: '2024-01-15',
  }

  describe('customerId field', () => {
    it('should accept valid customerId', () => {
      const result = jobSchema.safeParse(validJob)
      expect(result.success).toBe(true)
    })

    it('should reject customerId of 0', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        customerId: 0,
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Customer is required')
      }
    })

    it('should reject negative customerId', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        customerId: -1,
      })
      expect(result.success).toBe(false)
    })
  })

  describe('equipmentType field', () => {
    it('should reject empty equipmentType', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        equipmentType: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Equipment type is required')
      }
    })

    it('should reject equipmentType exceeding 200 characters', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        equipmentType: 'a'.repeat(201),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Equipment type cannot exceed 200 characters')
      }
    })
  })

  describe('equipmentModel field', () => {
    it('should reject empty equipmentModel', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        equipmentModel: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Equipment model is required')
      }
    })

    it('should reject equipmentModel exceeding 200 characters', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        equipmentModel: 'a'.repeat(201),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Equipment model cannot exceed 200 characters')
      }
    })
  })

  describe('description field', () => {
    it('should reject empty description', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        description: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Description is required')
      }
    })

    it('should reject description exceeding 2000 characters', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        description: 'a'.repeat(2001),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Description cannot exceed 2000 characters')
      }
    })
  })

  describe('status field', () => {
    it('should accept valid statuses', () => {
      const statuses = ['Quote', 'InProgress', 'Completed', 'Invoiced', 'Paid'] as const
      
      statuses.forEach(status => {
        const result = jobSchema.safeParse({
          ...validJob,
          status,
        })
        expect(result.success).toBe(true)
      })
    })

    it('should reject invalid status', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        status: 'InvalidStatus',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toContain('Status must be one of')
      }
    })
  })

  describe('dateReceived field', () => {
    it('should accept valid date', () => {
      const result = jobSchema.safeParse(validJob)
      expect(result.success).toBe(true)
    })

    it('should reject empty dateReceived', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        dateReceived: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Date received is required')
      }
    })
  })

  describe('dateCompleted field', () => {
    it('should accept valid dateCompleted after dateReceived', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        dateReceived: '2024-01-15',
        dateCompleted: '2024-01-20',
      })
      expect(result.success).toBe(true)
    })

    it('should accept dateCompleted same as dateReceived', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        dateReceived: '2024-01-15',
        dateCompleted: '2024-01-15',
      })
      expect(result.success).toBe(true)
    })

    it('should reject dateCompleted before dateReceived', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        dateReceived: '2024-01-20',
        dateCompleted: '2024-01-15',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Date completed must be on or after date received')
      }
    })

    it('should accept null dateCompleted', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        dateCompleted: null,
      })
      expect(result.success).toBe(true)
    })
  })

  describe('estimateAmount field', () => {
    it('should accept valid positive amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        estimateAmount: 150.50,
      })
      expect(result.success).toBe(true)
    })

    it('should accept zero amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        estimateAmount: 0,
      })
      expect(result.success).toBe(true)
    })

    it('should reject negative amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        estimateAmount: -10,
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Estimate amount must be 0 or greater')
      }
    })

    it('should accept null estimateAmount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        estimateAmount: null,
      })
      expect(result.success).toBe(true)
    })
  })

  describe('actualAmount field', () => {
    it('should accept valid positive amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        actualAmount: 200.75,
      })
      expect(result.success).toBe(true)
    })

    it('should accept zero amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        actualAmount: 0,
      })
      expect(result.success).toBe(true)
    })

    it('should reject negative amount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        actualAmount: -5,
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Actual amount must be 0 or greater')
      }
    })

    it('should accept null actualAmount', () => {
      const result = jobSchema.safeParse({
        ...validJob,
        actualAmount: null,
      })
      expect(result.success).toBe(true)
    })
  })
})
