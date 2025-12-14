export interface Customer {
  id: number
  name: string
  email?: string | null
  phone?: string | null
  address?: string | null
  notes?: string | null
  createdAt: string
  updatedAt: string
}

export interface CustomerRequestDto {
  name: string
  email?: string
  phone?: string
  address?: string
  notes?: string
}
