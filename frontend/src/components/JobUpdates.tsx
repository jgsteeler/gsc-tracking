import { useState } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Badge } from '@/components/ui/badge'
import { Trash2, MessageSquarePlus, Loader2 } from 'lucide-react'
import { useJobUpdates } from '@/hooks/useJobUpdates'
import { useToast } from '@/hooks/use-toast'
import { DeleteConfirmDialog } from '@/components/DeleteConfirmDialog'
import type { JobUpdate } from '@/types/job'

interface JobUpdatesProps {
  jobId: number
}

export function JobUpdates({ jobId }: JobUpdatesProps) {
  const { updates, loading, error, createUpdate, deleteUpdate } = useJobUpdates(jobId)
  const { toast } = useToast()
  const [newUpdateText, setNewUpdateText] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [updateToDelete, setUpdateToDelete] = useState<JobUpdate | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!newUpdateText.trim()) {
      toast({
        title: 'Error',
        description: 'Please enter an update',
        variant: 'destructive',
      })
      return
    }

    try {
      setIsSubmitting(true)
      await createUpdate({ updateText: newUpdateText.trim() })
      setNewUpdateText('')
      toast({
        title: 'Success',
        description: 'Job update added successfully',
      })
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to add job update',
        variant: 'destructive',
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleDeleteClick = (update: JobUpdate) => {
    setUpdateToDelete(update)
    setDeleteDialogOpen(true)
  }

  const handleDeleteConfirm = async () => {
    if (!updateToDelete || isSubmitting) return

    try {
      setIsSubmitting(true)
      await deleteUpdate(updateToDelete.id)
      toast({
        title: 'Success',
        description: 'Job update deleted successfully',
      })
      setDeleteDialogOpen(false)
      setUpdateToDelete(null)
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to delete job update',
        variant: 'destructive',
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
      hour12: true,
    }).format(date)
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Job Updates</CardTitle>
          <CardDescription>Track progress and notes for this job</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="text-red-500">Error loading updates: {error}</div>
        </CardContent>
      </Card>
    )
  }

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>Job Updates</CardTitle>
          <CardDescription>Track progress and notes for this job</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {/* Add new update form */}
          <form onSubmit={handleSubmit} className="space-y-2">
            <Textarea
              placeholder="Add a job update... (e.g., 'I finished the diagnostics on Jim's snow blower, the leak is from the crankcase cover')"
              value={newUpdateText}
              onChange={(e) => setNewUpdateText(e.target.value)}
              rows={3}
              disabled={isSubmitting}
              className="resize-none"
            />
            <div className="flex justify-end">
              <Button 
                type="submit" 
                disabled={isSubmitting || !newUpdateText.trim()}
                size="sm"
              >
                {isSubmitting ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Adding...
                  </>
                ) : (
                  <>
                    <MessageSquarePlus className="mr-2 h-4 w-4" />
                    Add Update
                  </>
                )}
              </Button>
            </div>
          </form>

          {/* Updates list */}
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : updates.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No updates yet. Add your first update above.
            </div>
          ) : (
            <div className="space-y-3">
              {updates.map((update) => (
                <div
                  key={update.id}
                  className="border rounded-lg p-4 bg-muted/50 hover:bg-muted transition-colors"
                >
                  <div className="flex justify-between items-start mb-2">
                    <Badge variant="outline" className="text-xs">
                      {formatDate(update.createdAt)}
                    </Badge>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleDeleteClick(update)}
                      className="h-8 w-8 p-0"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                  <p className="text-sm whitespace-pre-wrap">{update.updateText}</p>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <DeleteConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        onConfirm={handleDeleteConfirm}
        title="Delete Job Update"
        description="Are you sure you want to delete this update? This action cannot be undone."
      />
    </>
  )
}
