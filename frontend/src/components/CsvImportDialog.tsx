import { useState } from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { csvService } from '@/services/csvService'
import type { ImportResult } from '@/types/csv'
import { useToast } from '@/hooks/use-toast'
import Papa from 'papaparse'

interface CsvImportDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onImportComplete: () => void
}

export function CsvImportDialog({ open, onOpenChange, onImportComplete }: CsvImportDialogProps) {
  const [file, setFile] = useState<File | null>(null)
  const [preview, setPreview] = useState<string[][]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [importResult, setImportResult] = useState<ImportResult | null>(null)
  const { toast } = useToast()

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0]
    if (selectedFile) {
      setFile(selectedFile)
      setImportResult(null)
      
      // Parse CSV for preview
      Papa.parse(selectedFile, {
        preview: 6, // Show first 6 rows (header + 5 data rows)
        complete: (results) => {
          setPreview(results.data as string[][])
        },
        error: (error) => {
          toast({
            variant: 'destructive',
            title: 'Error parsing CSV',
            description: error.message,
          })
        },
      })
    }
  }

  const handleImport = async () => {
    if (!file) return

    setIsLoading(true)
    try {
      const result = await csvService.importExpenses(file)
      setImportResult(result)

      if (result.errorCount === 0) {
        toast({
          title: 'Import successful',
          description: `Successfully imported ${result.successCount} expense(s).`,
        })
        onImportComplete()
      } else if (result.successCount > 0) {
        toast({
          title: 'Import partially successful',
          description: `Imported ${result.successCount} expense(s), ${result.errorCount} failed.`,
        })
        onImportComplete()
      } else {
        toast({
          variant: 'destructive',
          title: 'Import failed',
          description: `All ${result.errorCount} expense(s) failed to import. Check errors below.`,
        })
      }
    } catch (error) {
      toast({
        variant: 'destructive',
        title: 'Import failed',
        description: error instanceof Error ? error.message : 'An error occurred during import',
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleClose = () => {
    setFile(null)
    setPreview([])
    setImportResult(null)
    onOpenChange(false)
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-4xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Import Expenses from CSV</DialogTitle>
          <DialogDescription>
            Upload a CSV file to import expenses. The file should have the following columns:
            Job ID, Type, Description, Amount, Date, Receipt Reference
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          {/* File Upload */}
          <div>
            <label htmlFor="csv-file" className="block text-sm font-medium mb-2">
              Select CSV File
            </label>
            <input
              id="csv-file"
              type="file"
              accept=".csv"
              onChange={handleFileChange}
              className="block w-full text-sm text-gray-500
                file:mr-4 file:py-2 file:px-4
                file:rounded-md file:border-0
                file:text-sm file:font-semibold
                file:bg-primary file:text-primary-foreground
                hover:file:bg-primary/90
                cursor-pointer"
            />
          </div>

          {/* Preview */}
          {preview.length > 0 && !importResult && (
            <div>
              <h3 className="text-sm font-medium mb-2">Preview (first 5 rows)</h3>
              <div className="border rounded-md overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="bg-muted">
                    <tr>
                      {preview[0]?.map((header, i) => (
                        <th key={i} className="px-3 py-2 text-left font-medium">
                          {header}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {preview.slice(1).map((row, i) => (
                      <tr key={i} className="border-t">
                        {row.map((cell, j) => (
                          <td key={j} className="px-3 py-2">
                            {cell}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* Import Result */}
          {importResult && (
            <div className="space-y-3">
              <div className="p-4 rounded-md border bg-card">
                <h3 className="font-medium mb-2">Import Summary</h3>
                <div className="space-y-1 text-sm">
                  <p className="text-green-600">
                    ✓ Successful: {importResult.successCount}
                  </p>
                  <p className="text-red-600">
                    ✗ Failed: {importResult.errorCount}
                  </p>
                </div>
              </div>

              {importResult.errors.length > 0 && (
                <div>
                  <h3 className="text-sm font-medium mb-2 text-red-600">Errors</h3>
                  <div className="border rounded-md max-h-60 overflow-y-auto">
                    <table className="w-full text-sm">
                      <thead className="bg-muted sticky top-0">
                        <tr>
                          <th className="px-3 py-2 text-left font-medium w-20">Line</th>
                          <th className="px-3 py-2 text-left font-medium">Error</th>
                          <th className="px-3 py-2 text-left font-medium">Data</th>
                        </tr>
                      </thead>
                      <tbody>
                        {importResult.errors.map((error, i) => (
                          <tr key={i} className="border-t">
                            <td className="px-3 py-2">{error.lineNumber}</td>
                            <td className="px-3 py-2 text-red-600">{error.message}</td>
                            <td className="px-3 py-2 text-xs text-muted-foreground">
                              {error.rawData}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </div>
          )}

          {/* Actions */}
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={handleClose}>
              {importResult ? 'Close' : 'Cancel'}
            </Button>
            {!importResult && (
              <Button 
                onClick={handleImport} 
                disabled={!file || isLoading}
              >
                {isLoading ? 'Importing...' : 'Import'}
              </Button>
            )}
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}
