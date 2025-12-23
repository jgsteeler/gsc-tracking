import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { CsvImportDialog } from './CsvImportDialog'
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

describe('CsvImportDialog', () => {
  const mockOnOpenChange = vi.fn()
  const mockOnImportComplete = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders dialog when open', () => {
    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )
    expect(screen.getByText('Import Expenses from CSV')).toBeInTheDocument()
    expect(screen.getByLabelText('Select CSV File')).toBeInTheDocument()
  })

  it('does not render when closed', () => {
    render(
      <CsvImportDialog
        open={false}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )
    expect(screen.queryByText('Import Expenses from CSV')).not.toBeInTheDocument()
  })

  it('handles file selection and shows preview', async () => {
    const user = userEvent.setup()
    
    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const fileInput = screen.getByLabelText('Select CSV File')
    const file = new File(
      ['Job ID,Type,Description,Amount,Date,Receipt Reference\n1,Parts,Oil,10.00,2025-01-15,REC-001'],
      'test.csv',
      { type: 'text/csv' }
    )

    await user.upload(fileInput, file)

    await waitFor(() => {
      expect(screen.getByText('Preview (first 5 rows)')).toBeInTheDocument()
    })
  })

  it('enables import button when file is selected', async () => {
    const user = userEvent.setup()
    
    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const importButton = screen.getByText('Import')
    expect(importButton).toBeDisabled()

    const fileInput = screen.getByLabelText('Select CSV File')
    const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })

    await user.upload(fileInput, file)

    await waitFor(() => {
      expect(importButton).not.toBeDisabled()
    })
  })

  it('handles successful import', async () => {
    const user = userEvent.setup()
    const mockResult = {
      successCount: 2,
      errorCount: 0,
      errors: [],
    }
    vi.mocked(csvService.importExpenses).mockResolvedValue(mockResult)

    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const fileInput = screen.getByLabelText('Select CSV File')
    const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })
    await user.upload(fileInput, file)

    const importButton = screen.getByText('Import')
    await user.click(importButton)

    await waitFor(() => {
      expect(csvService.importExpenses).toHaveBeenCalledWith(file, null)
      expect(mockOnImportComplete).toHaveBeenCalled()
      expect(screen.getByText('Import Summary')).toBeInTheDocument()
      expect(screen.getByText(/Successful: 2/)).toBeInTheDocument()
      expect(screen.getByText(/Failed: 0/)).toBeInTheDocument()
    })
  })

  it('handles import with errors', async () => {
    const user = userEvent.setup()
    const mockResult = {
      successCount: 1,
      errorCount: 1,
      errors: [
        {
          lineNumber: 2,
          message: 'Job with ID 999 not found',
          rawData: '999,Parts,Oil,10.00,2025-01-15',
        },
      ],
    }
    vi.mocked(csvService.importExpenses).mockResolvedValue(mockResult)

    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const fileInput = screen.getByLabelText('Select CSV File')
    const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })
    await user.upload(fileInput, file)

    const importButton = screen.getByText('Import')
    await user.click(importButton)

    await waitFor(() => {
      expect(screen.getByText('Errors')).toBeInTheDocument()
      expect(screen.getByText('Job with ID 999 not found')).toBeInTheDocument()
      expect(screen.getByText('2')).toBeInTheDocument() // Line number
    })
  })

  it('handles import failure', async () => {
    const user = userEvent.setup()
    const error = new Error('Import failed')
    vi.mocked(csvService.importExpenses).mockRejectedValue(error)

    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const fileInput = screen.getByLabelText('Select CSV File')
    const file = new File(['test,data'], 'test.csv', { type: 'text/csv' })
    await user.upload(fileInput, file)

    const importButton = screen.getByText('Import')
    await user.click(importButton)

    await waitFor(() => {
      expect(csvService.importExpenses).toHaveBeenCalled()
    })
  })

  it('closes dialog when cancel is clicked', async () => {
    const user = userEvent.setup()
    
    render(
      <CsvImportDialog
        open={true}
        onOpenChange={mockOnOpenChange}
        onImportComplete={mockOnImportComplete}
      />
    )

    const cancelButton = screen.getByText('Cancel')
    await user.click(cancelButton)

    expect(mockOnOpenChange).toHaveBeenCalledWith(false)
  })
})
