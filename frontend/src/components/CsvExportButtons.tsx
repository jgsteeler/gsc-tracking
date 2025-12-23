import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { csvService } from '@/services/csvService'
import { useAccessToken } from '@/hooks/useAccessToken'
import { useToast } from '@/hooks/use-toast'
import { Download } from 'lucide-react'

interface CsvExportButtonsProps {
  type: 'expenses' | 'jobs'
  jobId?: number
  status?: string
  className?: string
}

const generateFilename = (type: string, filter?: string | number) => {
  const date = new Date().toISOString().split('T')[0]
  if (filter !== undefined) {
    return `${type}_${filter}_${date}.csv`
  }
  return `${type}_${date}.csv`
}

export function CsvExportButtons({ type, jobId, status, className }: CsvExportButtonsProps) {
  const [isExporting, setIsExporting] = useState(false)
  const { toast } = useToast()
  const { getToken } = useAccessToken()

  const handleExport = async () => {
    setIsExporting(true)
    try {
      const token = await getToken()
      let blob: Blob
      let filename: string

      if (type === 'expenses') {
        blob = await csvService.exportExpenses(jobId, token)
        filename = jobId ? generateFilename('expenses_job', jobId) : generateFilename('expenses')
      } else {
        blob = await csvService.exportJobs(status, token)
        filename = status ? generateFilename('jobs', status) : generateFilename('jobs')
      }

      csvService.downloadBlob(blob, filename)
      
      toast({
        title: 'Export successful',
        description: `${type === 'expenses' ? 'Expenses' : 'Jobs'} exported to CSV successfully.`,
      })
    } catch (error) {
      toast({
        variant: 'destructive',
        title: 'Export failed',
        description: error instanceof Error ? error.message : 'An error occurred during export',
      })
    } finally {
      setIsExporting(false)
    }
  }

  return (
    <Button
      variant="outline"
      size="sm"
      onClick={handleExport}
      disabled={isExporting}
      className={className}
    >
      <Download className="h-4 w-4 mr-2" />
      {isExporting ? 'Exporting...' : `Export ${type === 'expenses' ? 'Expenses' : 'Jobs'}`}
    </Button>
  )
}
