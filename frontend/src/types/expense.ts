export interface Expense {
  id: number
  jobId: number
  type: ExpenseType
  description: string
  amount: number
  date: string
  receiptReference?: string | null
  createdAt: string
  updatedAt: string
}

export type ExpenseType = 'Parts' | 'Labor' | 'Service'

export interface ExpenseRequestDto {
  type: ExpenseType
  description: string
  amount: number
  date: string
  receiptReference?: string | null
}

export const EXPENSE_TYPES: ExpenseType[] = ['Parts', 'Labor', 'Service']

export const EXPENSE_TYPE_LABELS: Record<ExpenseType, string> = {
  Parts: 'Parts',
  Labor: 'Labor',
  Service: 'Service'
}

export const EXPENSE_TYPE_COLORS: Record<ExpenseType, string> = {
  Parts: 'bg-blue-100 text-blue-800 border-blue-200',
  Labor: 'bg-green-100 text-green-800 border-green-200',
  Service: 'bg-purple-100 text-purple-800 border-purple-200'
}
