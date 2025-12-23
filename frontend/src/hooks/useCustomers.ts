import { useState, useEffect, useCallback } from 'react'
import type { Customer, CustomerRequestDto } from '@/types/customer'
import { customerService } from '@/services/customerService'
import { useAccessToken } from '@/hooks/useAccessToken'

export function useCustomers(searchTerm?: string) {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { getToken } = useAccessToken()

  const fetchCustomers = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      const token = await getToken()
      const data = await customerService.getAll(searchTerm, token)
      setCustomers(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch customers')
    } finally {
      setLoading(false)
    }
  }, [searchTerm, getToken])

  useEffect(() => {
    fetchCustomers()
  }, [fetchCustomers])

  const createCustomer = async (data: CustomerRequestDto): Promise<Customer> => {
    const token = await getToken()
    const newCustomer = await customerService.create(data, token)
    setCustomers((prev) => [...prev, newCustomer])
    return newCustomer
  }

  const updateCustomer = async (id: number, data: CustomerRequestDto): Promise<Customer> => {
    const token = await getToken()
    const updatedCustomer = await customerService.update(id, data, token)
    setCustomers((prev) =>
      prev.map((customer) => (customer.id === id ? updatedCustomer : customer))
    )
    return updatedCustomer
  }

  const deleteCustomer = async (id: number): Promise<void> => {
    const token = await getToken()
    await customerService.delete(id, token)
    setCustomers((prev) => prev.filter((customer) => customer.id !== id))
  }

  return {
    customers,
    loading,
    error,
    refetch: fetchCustomers,
    createCustomer,
    updateCustomer,
    deleteCustomer,
  }
}
