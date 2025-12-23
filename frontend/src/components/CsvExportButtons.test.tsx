import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { CsvExportButtons } from './CsvExportButtons'
import { csvService } from '@/services/csvService'

// Mock the services and hooks
vi.mock('@/services/csvService')
vi.mock('@/hooks/use-toast', () => ({
  useToast: () => ({
    toast: vi.fn(),
  }),
}))

// Mock useAccessToken with a stable getToken function
const mockGetToken = vi.fn().mockResolvedValue(null);
vi.mock('@/hooks/useAccessToken', () => ({
  useAccessToken: () => ({
    getToken: mockGetToken,
    isAuthEnabled: false,
    isAuthenticated: false,
  }),
}))

describe('CsvExportButtons', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders export button for expenses', () => {
    render(<CsvExportButtons type="expenses" />)
    expect(screen.getByText('Export Expenses')).toBeInTheDocument()
  })

  it('renders export button for jobs', () => {
    render(<CsvExportButtons type="jobs" />)
    expect(screen.getByText('Export Jobs')).toBeInTheDocument()
  })

  it('handles expense export click', async () => {
    const user = userEvent.setup()
    const mockBlob = new Blob(['test'], { type: 'text/csv' })
    vi.mocked(csvService.exportExpenses).mockResolvedValue(mockBlob)
    vi.mocked(csvService.downloadBlob).mockImplementation(() => {})

    render(<CsvExportButtons type="expenses" />)
    
    const button = screen.getByText('Export Expenses')
    await user.click(button)

    await waitFor(() => {
      expect(csvService.exportExpenses).toHaveBeenCalledWith(undefined, null)
      expect(csvService.downloadBlob).toHaveBeenCalledWith(
        mockBlob,
        expect.stringContaining('expenses_')
      )
    })
  })

  it('handles job export click with status filter', async () => {
    const user = userEvent.setup()
    const mockBlob = new Blob(['test'], { type: 'text/csv' })
    vi.mocked(csvService.exportJobs).mockResolvedValue(mockBlob)
    vi.mocked(csvService.downloadBlob).mockImplementation(() => {})

    render(<CsvExportButtons type="jobs" status="Completed" />)
    
    const button = screen.getByText('Export Jobs')
    await user.click(button)

    await waitFor(() => {
      expect(csvService.exportJobs).toHaveBeenCalledWith('Completed', null)
      expect(csvService.downloadBlob).toHaveBeenCalledWith(
        mockBlob,
        expect.stringContaining('jobs_Completed_')
      )
    })
  })

  it('disables button while exporting', async () => {
    const user = userEvent.setup()
    const mockBlob = new Blob(['test'], { type: 'text/csv' })
    vi.mocked(csvService.exportExpenses).mockImplementation(
      () => new Promise((resolve) => setTimeout(() => resolve(mockBlob), 100))
    )
    vi.mocked(csvService.downloadBlob).mockImplementation(() => {})

    render(<CsvExportButtons type="expenses" />)
    
    const button = screen.getByText('Export Expenses')
    await user.click(button)

    // Button should show loading state
    expect(screen.getByText('Exporting...')).toBeInTheDocument()
    expect(button).toBeDisabled()

    // Wait for export to complete
    await waitFor(() => {
      expect(screen.getByText('Export Expenses')).toBeInTheDocument()
    })
  })

  it('handles export error', async () => {
    const user = userEvent.setup()
    const error = new Error('Export failed')
    vi.mocked(csvService.exportExpenses).mockRejectedValue(error)

    render(<CsvExportButtons type="expenses" />)
    
    const button = screen.getByText('Export Expenses')
    await user.click(button)

    await waitFor(() => {
      expect(csvService.exportExpenses).toHaveBeenCalled()
    })
  })
})
