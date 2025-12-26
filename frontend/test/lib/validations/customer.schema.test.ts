import { describe, it, expect } from 'vitest'
import { customerSchema } from '../../../src/lib/validations/customer.schema'

describe('customerSchema', () => {
  describe('name field', () => {
    it('should accept valid name', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
      })
      expect(result.success).toBe(true)
    })

    it('should reject empty name', () => {
      const result = customerSchema.safeParse({
        name: '',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Name is required')
      }
    })

    it('should reject name exceeding 200 characters', () => {
      const result = customerSchema.safeParse({
        name: 'a'.repeat(201),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Name cannot exceed 200 characters')
      }
    })
  })

  describe('email field', () => {
    it('should accept valid email', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        email: 'john@example.com',
      })
      expect(result.success).toBe(true)
    })

    it('should accept empty email', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        email: '',
      })
      expect(result.success).toBe(true)
    })

    it('should accept undefined email', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
      })
      expect(result.success).toBe(true)
    })

    it('should reject invalid email format', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        email: 'invalid-email',
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Invalid email format')
      }
    })

    it('should reject email exceeding 200 characters', () => {
      // Create a valid email that's > 200 characters
      const longLocal = 'a'.repeat(192) // local part
      const longEmail = `${longLocal}@example.com` // 192 + 12 = 204 chars
      const result = customerSchema.safeParse({
        name: 'John Doe',
        email: longEmail,
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Email cannot exceed 200 characters')
      }
    })
  })

  describe('phone field', () => {
    it('should accept valid phone', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        phone: '(555) 123-4567',
      })
      expect(result.success).toBe(true)
    })

    it('should accept empty phone', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        phone: '',
      })
      expect(result.success).toBe(true)
    })

    it('should reject phone exceeding 50 characters', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        phone: '1'.repeat(51),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Phone cannot exceed 50 characters')
      }
    })
  })

  describe('address field', () => {
    it('should accept valid address', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        address: '123 Main St, City, State ZIP',
      })
      expect(result.success).toBe(true)
    })

    it('should reject address exceeding 500 characters', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        address: 'a'.repeat(501),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Address cannot exceed 500 characters')
      }
    })
  })

  describe('notes field', () => {
    it('should accept valid notes', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        notes: 'Some notes about the customer',
      })
      expect(result.success).toBe(true)
    })

    it('should reject notes exceeding 2000 characters', () => {
      const result = customerSchema.safeParse({
        name: 'John Doe',
        notes: 'a'.repeat(2001),
      })
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error.issues[0].message).toBe('Notes cannot exceed 2000 characters')
      }
    })
  })
})
