import { useState, useEffect, useCallback } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Plus, Pencil, Trash2, Loader2 } from 'lucide-react'
import { expenseService } from '@/services/expenseService'
import type { Expense } from '@/types/expense'
import { EXPENSE_TYPE_COLORS, EXPENSE_TYPE_LABELS } from '@/types/expense'
import { ExpenseDialog } from '@/components/ExpenseDialog'
import { DeleteConfirmDialog } from '@/components/DeleteConfirmDialog'
import type { ExpenseFormValues } from '@/lib/validations'
import { useToast } from '@/hooks/use-toast'

interface ExpenseListProps {
  jobId: number
  onExpenseChange?: () => void
}

export function ExpenseList({ jobId, onExpenseChange }: ExpenseListProps) {
  const { toast } = useToast()
  const [expenses, setExpenses] = useState<Expense[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [selectedExpense, setSelectedExpense] = useState<Expense | undefined>()
  const [expenseToDelete, setExpenseToDelete] = useState<Expense | null>(null)

  const fetchExpenses = useCallback(async () => {
    try {
      setLoading(true)
      const data = await expenseService.getByJobId(jobId)
      setExpenses(data)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load expenses')
    } finally {
      setLoading(false)
    }
  }, [jobId])

  useEffect(() => {
    fetchExpenses()
  }, [fetchExpenses])

  const handleCreate = async (data: ExpenseFormValues) => {
    try {
      await expenseService.create(jobId, data)
      toast({
        title: 'Success',
        description: 'Expense created successfully',
      })
      setDialogOpen(false)
      fetchExpenses()
      onExpenseChange?.()
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to create expense',
        variant: 'destructive',
      })
    }
  }

  const handleUpdate = async (data: ExpenseFormValues) => {
    if (!selectedExpense) return

    try {
      await expenseService.update(selectedExpense.id, data)
      toast({
        title: 'Success',
        description: 'Expense updated successfully',
      })
      setDialogOpen(false)
      setSelectedExpense(undefined)
      fetchExpenses()
      onExpenseChange?.()
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to update expense',
        variant: 'destructive',
      })
    }
  }

  const handleDelete = async () => {
    if (!expenseToDelete) return

    try {
      await expenseService.delete(expenseToDelete.id)
      toast({
        title: 'Success',
        description: 'Expense deleted successfully',
      })
      setDeleteDialogOpen(false)
      setExpenseToDelete(null)
      fetchExpenses()
      onExpenseChange?.()
    } catch (err) {
      toast({
        title: 'Error',
        description: err instanceof Error ? err.message : 'Failed to delete expense',
        variant: 'destructive',
      })
    }
  }

  const handleEdit = (expense: Expense) => {
    setSelectedExpense(expense)
    setDialogOpen(true)
  }

  const handleDeleteClick = (expense: Expense) => {
    setExpenseToDelete(expense)
    setDeleteDialogOpen(true)
  }

  const handleAddNew = () => {
    setSelectedExpense(undefined)
    setDialogOpen(true)
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount)
  }

  const formatDate = (dateString: string) => {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    }).format(new Date(dateString))
  }

  const totalCost = expenses.reduce((sum, expense) => sum + expense.amount, 0)

  return (
    <>
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Expenses</CardTitle>
              <CardDescription>Track all costs associated with this job</CardDescription>
            </div>
            <Button onClick={handleAddNew}>
              <Plus className="mr-2 h-4 w-4" />
              Add Expense
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : error ? (
            <div className="text-red-500 py-4">{error}</div>
          ) : expenses.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No expenses recorded yet. Click "Add Expense" to get started.
            </div>
          ) : (
            <div className="space-y-4">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Type</TableHead>
                    <TableHead>Description</TableHead>
                    <TableHead>Date</TableHead>
                    <TableHead>Receipt</TableHead>
                    <TableHead className="text-right">Amount</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {expenses.map((expense) => (
                    <TableRow key={expense.id}>
                      <TableCell>
                        <Badge className={EXPENSE_TYPE_COLORS[expense.type]}>
                          {EXPENSE_TYPE_LABELS[expense.type]}
                        </Badge>
                      </TableCell>
                      <TableCell>{expense.description}</TableCell>
                      <TableCell>{formatDate(expense.date)}</TableCell>
                      <TableCell>
                        {expense.receiptReference || '-'}
                      </TableCell>
                      <TableCell className="text-right font-medium">
                        {formatCurrency(expense.amount)}
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEdit(expense)}
                          >
                            <Pencil className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDeleteClick(expense)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              
              <div className="flex justify-end pt-4 border-t">
                <div className="text-right">
                  <p className="text-sm text-muted-foreground">Total Cost</p>
                  <p className="text-2xl font-bold">{formatCurrency(totalCost)}</p>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <ExpenseDialog
        expense={selectedExpense}
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onSubmit={selectedExpense ? handleUpdate : handleCreate}
      />

      <DeleteConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        onConfirm={handleDelete}
        title="Delete Expense"
        description={`Are you sure you want to delete this expense? This action cannot be undone.`}
      />
    </>
  )
}
