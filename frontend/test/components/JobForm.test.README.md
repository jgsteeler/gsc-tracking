# JobForm Component Tests

This test file provides comprehensive coverage for the JobForm component's customer creation feature.

## Test Coverage

The test suite covers the following scenarios:

### Basic Rendering
- ✅ Renders customer select field with existing customers
- ✅ Renders the "Add new customer" button next to the customer dropdown
- ✅ Renders all required form fields (Customer, Equipment Type, Equipment Model, Description, Status, Date Received)
- ✅ Shows "Create" button when creating a new job
- ✅ Shows "Update" button when editing an existing job

### Customer Dialog Integration
- ✅ Opens customer dialog when "Add new customer" button is clicked
- ✅ Closes customer dialog when cancel button is clicked

### Customer Creation Flow
- ✅ Creates a new customer through the dialog
- ✅ Auto-selects the newly created customer in the job form
- ✅ Updates the customer list after creating a new customer
- ✅ Handles customer creation errors gracefully

## Running the Tests

### Run only JobForm tests
```bash
npm test -- JobForm.test.tsx
```

### Run all tests
```bash
npm test
```

### Run tests in watch mode
```bash
npm test -- --watch
```

### Run tests with coverage
```bash
npm test -- --coverage
```

## Test Structure

The tests use the following tools and libraries:
- **Vitest**: Testing framework
- **React Testing Library**: For rendering and interacting with components
- **@testing-library/user-event**: For simulating user interactions
- **@testing-library/jest-dom**: For DOM matchers

## Mocking Strategy

### useCustomers Hook
The `useCustomers` hook is mocked to provide controlled test data and verify that the customer creation function is called correctly.

### CustomerDialog Component
The CustomerDialog component is mocked to simplify testing of the integration between JobForm and the dialog, focusing on the data flow rather than the dialog's internal implementation.

## Key Test Scenarios

### 1. Adding a Customer Button
Tests that the "+" button is rendered next to the customer dropdown.

### 2. Opening the Dialog
Verifies that clicking the button opens the CustomerDialog.

### 3. Creating a Customer
Tests the complete flow:
1. Click "Add new customer" button
2. Dialog opens
3. User fills in customer information
4. Click "Create" button
5. New customer is created via `createCustomer`
6. Dialog closes
7. New customer is auto-selected in the job form

### 4. Error Handling
Ensures that errors during customer creation are caught and logged, preventing the application from crashing.

### 5. Customer List Updates
Verifies that the component re-renders when the customer list changes (when a new customer is added).

## Future Improvements

Potential areas for additional test coverage:
- Integration tests with a real CustomerDialog component
- E2E tests using Playwright to test the complete user workflow
- Tests for form submission with the newly created customer
- Tests for keyboard navigation and accessibility
- Tests for concurrent customer creation scenarios
