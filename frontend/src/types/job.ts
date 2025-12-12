export interface Job {
  id: number
  customerId: number
  customerName: string
  equipmentType: string
  equipmentModel: string
  description: string
  status: JobStatus
  dateReceived: string
  dateCompleted?: string | null
  estimateAmount?: number | null
  actualAmount?: number | null
  createdAt: string
  updatedAt: string
}

export type JobStatus = 'Quote' | 'InProgress' | 'Completed' | 'Invoiced' | 'Paid'

export interface JobRequestDto {
  customerId: number
  equipmentType: string
  equipmentModel: string
  description: string
  status: JobStatus
  dateReceived: string
  dateCompleted?: string | null
  estimateAmount?: number | null
  actualAmount?: number | null
}

export const JOB_STATUSES: JobStatus[] = ['Quote', 'InProgress', 'Completed', 'Invoiced', 'Paid']

export const JOB_STATUS_LABELS: Record<JobStatus, string> = {
  Quote: 'Quote',
  InProgress: 'In Progress',
  Completed: 'Completed',
  Invoiced: 'Invoiced',
  Paid: 'Paid'
}

export const JOB_STATUS_COLORS: Record<JobStatus, string> = {
  Quote: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  InProgress: 'bg-blue-100 text-blue-800 border-blue-200',
  Completed: 'bg-green-100 text-green-800 border-green-200',
  Invoiced: 'bg-purple-100 text-purple-800 border-purple-200',
  Paid: 'bg-gray-100 text-gray-800 border-gray-200'
}
