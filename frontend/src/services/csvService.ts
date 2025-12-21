import type { ImportResult } from '@/types/csv'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5091/api'

export const csvService = {
  /**
   * Export expenses to CSV file
   * @param jobId Optional job ID to filter expenses
   */
  async exportExpenses(jobId?: number): Promise<Blob> {
    const url = jobId 
      ? `${API_BASE_URL}/export/expenses?jobId=${jobId}`
      : `${API_BASE_URL}/export/expenses`
    
    const response = await fetch(url)
    
    if (!response.ok) {
      throw new Error('Failed to export expenses')
    }
    
    return response.blob()
  },

  /**
   * Export jobs to CSV file
   * @param status Optional status to filter jobs
   */
  async exportJobs(status?: string): Promise<Blob> {
    const url = status 
      ? `${API_BASE_URL}/export/jobs?status=${status}`
      : `${API_BASE_URL}/export/jobs`
    
    const response = await fetch(url)
    
    if (!response.ok) {
      throw new Error('Failed to export jobs')
    }
    
    return response.blob()
  },

  /**
   * Import expenses from CSV file
   * @param file CSV file containing expense data
   */
  async importExpenses(file: File): Promise<ImportResult> {
    const formData = new FormData()
    formData.append('file', file)
    
    const response = await fetch(`${API_BASE_URL}/import/expenses`, {
      method: 'POST',
      body: formData,
    })
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: 'Failed to import expenses' }))
      throw new Error(errorData.message || 'Failed to import expenses')
    }
    
    return response.json()
  },

  /**
   * Download a blob as a file
   * @param blob Blob data
   * @param filename Filename for download
   */
  downloadBlob(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  },
}
