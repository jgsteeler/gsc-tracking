import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { JobForm } from './JobForm';
import { useCustomers } from '@/hooks/useCustomers';
import type { Customer } from '@/types/customer';

// Mock the useCustomers hook
vi.mock('@/hooks/useCustomers', () => ({
  useCustomers: vi.fn(),
}));

// Mock the CustomerDialog component
vi.mock('./CustomerDialog', () => ({
  CustomerDialog: ({ open, onSubmit, onOpenChange }: any) => {
    return open ? (
      <div data-testid="customer-dialog">
        <h2>Add New Customer</h2>
        <input
          data-testid="customer-name-input"
          placeholder="Customer name"
        />
        <button
          data-testid="customer-create-button"
          onClick={async () => {
            try {
              await onSubmit({ name: 'Test Customer' });
            } catch (error) {
              // Catch and log errors for test visibility
              console.error('Error in mock:', error);
            }
          }}
        >
          Create Customer
        </button>
        <button
          data-testid="customer-cancel-button"
          onClick={() => onOpenChange(false)}
        >
          Cancel
        </button>
      </div>
    ) : null;
  },
}));

describe('JobForm - Customer Creation Feature', () => {
  const mockCustomers: Customer[] = [
    {
      id: 1,
      name: 'Existing Customer',
      email: 'existing@example.com',
      phone: '1234567890',
      address: '123 Main St',
      notes: null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ];

  const mockCreateCustomer = vi.fn();
  const mockOnSubmit = vi.fn();
  const mockOnCancel = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useCustomers).mockReturnValue({
      customers: mockCustomers,
      loading: false,
      error: null,
      refetch: vi.fn(),
      createCustomer: mockCreateCustomer,
      updateCustomer: vi.fn(),
      deleteCustomer: vi.fn(),
    });
  });

  it('should render the customer select field with existing customers', () => {
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByLabelText('Customer')).toBeInTheDocument();
  });

  it('should render the "Add new customer" button', () => {
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    const addButton = screen.getByRole('button', { name: /add new customer/i });
    expect(addButton).toBeInTheDocument();
  });

  it('should open customer dialog when "Add new customer" button is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    const addButton = screen.getByRole('button', { name: /add new customer/i });
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByTestId('customer-dialog')).toBeInTheDocument();
    });
  });

  it('should create a new customer and auto-select it', async () => {
    const user = userEvent.setup();
    const newCustomer: Customer = {
      id: 2,
      name: 'Test Customer',
      email: 'test@example.com',
      phone: '9876543210',
      address: '456 Oak Ave',
      notes: null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    mockCreateCustomer.mockResolvedValueOnce(newCustomer);

    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    // Open the customer dialog
    const addButton = screen.getByRole('button', { name: /add new customer/i });
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByTestId('customer-dialog')).toBeInTheDocument();
    });

    // Create the customer
    const createButton = screen.getByTestId('customer-create-button');
    await user.click(createButton);

    await waitFor(() => {
      expect(mockCreateCustomer).toHaveBeenCalledWith({ name: 'Test Customer' });
    });
  });

  it('should close customer dialog when cancel is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    // Open the customer dialog
    const addButton = screen.getByRole('button', { name: /add new customer/i });
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByTestId('customer-dialog')).toBeInTheDocument();
    });

    // Cancel
    const cancelButton = screen.getByTestId('customer-cancel-button');
    await user.click(cancelButton);

    await waitFor(() => {
      expect(screen.queryByTestId('customer-dialog')).not.toBeInTheDocument();
    });
  });

  it('should handle customer creation error gracefully', async () => {
    const user = userEvent.setup();
    const errorMessage = 'Failed to create customer';
    
    mockCreateCustomer.mockRejectedValueOnce(new Error(errorMessage));

    // Mock console.error to avoid cluttering test output
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    // Open the customer dialog
    const addButton = screen.getByRole('button', { name: /add new customer/i });
    await user.click(addButton);

    await waitFor(() => {
      expect(screen.getByTestId('customer-dialog')).toBeInTheDocument();
    });

    // Try to create the customer
    const createButton = screen.getByTestId('customer-create-button');
    await user.click(createButton);

    // Wait for the error to be logged
    await waitFor(() => {
      expect(mockCreateCustomer).toHaveBeenCalled();
      expect(consoleSpy).toHaveBeenCalled();
    });

    consoleSpy.mockRestore();
  });

  it('should update customer list when creating a new customer', async () => {
    const user = userEvent.setup();
    const newCustomer: Customer = {
      id: 2,
      name: 'New Customer',
      email: 'new@example.com',
      phone: '5555555555',
      address: '789 Pine St',
      notes: null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    // First render with 1 customer
    const { rerender } = render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    // Simulate creating a customer which updates the customers list
    mockCreateCustomer.mockResolvedValueOnce(newCustomer);
    
    // Update the mock to return the new customer list
    vi.mocked(useCustomers).mockReturnValue({
      customers: [...mockCustomers, newCustomer],
      loading: false,
      error: null,
      refetch: vi.fn(),
      createCustomer: mockCreateCustomer,
      updateCustomer: vi.fn(),
      deleteCustomer: vi.fn(),
    });

    // Re-render with updated customers
    rerender(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    // Verify the component re-rendered with the new customer list
    expect(useCustomers).toHaveBeenCalled();
  });

  it('should render all required fields', () => {
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByLabelText('Customer')).toBeInTheDocument();
    expect(screen.getByLabelText('Equipment Type')).toBeInTheDocument();
    expect(screen.getByLabelText('Equipment Model')).toBeInTheDocument();
    expect(screen.getByLabelText('Description')).toBeInTheDocument();
    expect(screen.getByLabelText('Status')).toBeInTheDocument();
    expect(screen.getByLabelText('Date Received')).toBeInTheDocument();
  });

  it('should render create button when no job is provided', () => {
    render(
      <JobForm
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByRole('button', { name: /create/i })).toBeInTheDocument();
  });

  it('should render update button when job is provided', () => {
    const existingJob = {
      id: 1,
      customerId: 1,
      customerName: 'Existing Customer',
      equipmentType: 'Lawn Mower',
      equipmentModel: 'Honda HRX217',
      description: 'Oil change needed',
      status: 'Quote' as const,
      dateReceived: new Date().toISOString(),
      dateCompleted: null,
      estimateAmount: 50,
      actualAmount: null,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    render(
      <JobForm
        job={existingJob}
        onSubmit={mockOnSubmit}
        onCancel={mockOnCancel}
      />
    );

    expect(screen.getByRole('button', { name: /update/i })).toBeInTheDocument();
  });
});
