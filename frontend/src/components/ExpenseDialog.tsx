import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { ExpenseForm } from '@/components/ExpenseForm'
import type { Expense } from '@/types/expense'
import type { ExpenseFormValues } from '@/lib/validations'

interface ExpenseDialogProps {
  expense?: Expense
  open: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (data: ExpenseFormValues) => Promise<void>
}

export function ExpenseDialog({ expense, open, onOpenChange, onSubmit }: ExpenseDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>{expense ? 'Edit Expense' : 'Add Expense'}</DialogTitle>
          <DialogDescription>
            {expense 
              ? 'Update the expense details below.' 
              : 'Add a new expense to this job.'}
          </DialogDescription>
        </DialogHeader>
        <ExpenseForm
          expense={expense}
          onSubmit={onSubmit}
          onCancel={() => onOpenChange(false)}
        />
      </DialogContent>
    </Dialog>
  )
}
