import type { Customer } from '@/types/customer'
import { CustomerForm, type CustomerFormValues } from './CustomerForm'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'

interface CustomerDialogProps {
  customer?: Customer | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (data: CustomerFormValues) => Promise<void>
}

export function CustomerDialog({
  customer,
  open,
  onOpenChange,
  onSubmit,
}: CustomerDialogProps) {
  const handleSubmit = async (data: CustomerFormValues) => {
    await onSubmit(data)
    onOpenChange(false)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>{customer ? 'Edit Customer' : 'Add New Customer'}</DialogTitle>
          <DialogDescription>
            {customer
              ? 'Update the customer information below.'
              : 'Fill in the customer information below.'}
          </DialogDescription>
        </DialogHeader>
        <CustomerForm
          customer={customer || undefined}
          onSubmit={handleSubmit}
          onCancel={() => onOpenChange(false)}
        />
      </DialogContent>
    </Dialog>
  )
}
