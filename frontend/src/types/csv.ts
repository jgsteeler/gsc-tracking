export interface ImportResult {
  successCount: number
  errorCount: number
  errors: ImportError[]
}

export interface ImportError {
  lineNumber: number
  message: string
  rawData?: string
}

export interface ExpenseImport {
  jobId: number
  type: string
  description: string
  amount: number
  date: string
  receiptReference?: string
}
