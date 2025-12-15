# Form Validation Documentation

This document describes the comprehensive form validation implemented in the GSC Tracking application, covering both frontend (React + Zod) and backend (.NET + FluentValidation).

## Overview

The application uses a dual-layer validation approach:
1. **Frontend validation** with Zod schemas and React Hook Form for immediate user feedback
2. **Backend validation** with FluentValidation for data integrity and security

Both layers use **identical validation rules** to ensure consistency between client and server.

## Validation Architecture

### Frontend (Zod + React Hook Form)

All Zod schemas are centralized in `frontend/src/lib/validations/`:
- `customer.schema.ts` - Customer form validation
- `job.schema.ts` - Job form validation
- `jobUpdate.schema.ts` - Job update validation
- `index.ts` - Barrel export for easy imports

**Usage Example:**
```typescript
import { customerSchema, type CustomerFormValues } from '@/lib/validations'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

const form = useForm<CustomerFormValues>({
  resolver: zodResolver(customerSchema),
  defaultValues: { name: '', email: '', phone: '', address: '', notes: '' }
})
```

### Backend (FluentValidation)

Validators are in `backend/GscTracking.Api/Validators/`:
- `CustomerRequestDtoValidator.cs` - Customer DTO validation
- `JobRequestDtoValidator.cs` - Job DTO validation
- `JobUpdateRequestDtoValidator.cs` - Job update validation

FluentValidation is configured in `Program.cs`:
```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerRequestDtoValidator>();
```

## Validation Rules

### Customer Validation

| Field   | Rules                                           | Frontend | Backend |
|---------|-------------------------------------------------|----------|---------|
| Name    | Required, max 200 characters                    | ✅       | ✅      |
| Email   | Optional, valid email format, max 200 characters| ✅       | ✅      |
| Phone   | Optional, max 50 characters                     | ✅       | ✅      |
| Address | Optional, max 500 characters                    | ✅       | ✅      |
| Notes   | Optional, max 2000 characters                   | ✅       | ✅      |

**Example Error Messages:**
- Name: "Name is required"
- Email: "Invalid email format"
- Phone: "Phone cannot exceed 50 characters"

### Job Validation

| Field          | Rules                                              | Frontend | Backend |
|----------------|----------------------------------------------------|----------|---------|
| CustomerId     | Required, must be > 0                              | ✅       | ✅      |
| EquipmentType  | Required, max 200 characters                       | ✅       | ✅      |
| EquipmentModel | Required, max 200 characters                       | ✅       | ✅      |
| Description    | Required, max 2000 characters                      | ✅       | ✅      |
| Status         | Required, must be one of: Quote, InProgress, Completed, Invoiced, Paid | ✅ | ✅ |
| DateReceived   | Required                                           | ✅       | ✅      |
| DateCompleted  | Optional, must be >= DateReceived                  | ✅       | ✅      |
| EstimateAmount | Optional, must be >= 0                             | ✅       | ✅      |
| ActualAmount   | Optional, must be >= 0                             | ✅       | ✅      |

**Cross-Field Validation:**
- DateCompleted must be on or after DateReceived (validated on both frontend and backend)

**Example Error Messages:**
- CustomerId: "Customer is required"
- Status: "Status must be one of: Quote, InProgress, Completed, Invoiced, Paid"
- DateCompleted: "Date completed must be on or after date received"

### Job Update Validation

| Field       | Rules                              | Frontend | Backend |
|-------------|------------------------------------|----------|---------|
| UpdateText  | Required, max 4000 characters      | ✅       | ✅      |

## Error Handling

### Frontend Error Display

Forms use shadcn/ui Form components with inline error messages:

```typescript
<FormField
  control={form.control}
  name="name"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Name</FormLabel>
      <FormControl>
        <Input {...field} />
      </FormControl>
      <FormMessage /> {/* Displays validation errors */}
    </FormItem>
  )}
/>
```

**Form Submission Prevention:**
- React Hook Form automatically prevents submission when validation fails
- Submit button shows "Saving..." state during submission
- Submit button can be disabled with `disabled={form.formState.isSubmitting}`

### Backend Error Response

When validation fails, the backend returns `400 Bad Request` with detailed error information:

**Example Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Name is required"],
    "Email": ["Invalid email format"]
  }
}
```

**API Response Codes:**
- `200 OK` - Successful update
- `201 Created` - Successful creation
- `400 Bad Request` - Validation error
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Testing

### Frontend Tests

All validation schemas have comprehensive unit tests in `frontend/src/lib/validations/*.test.ts`:

**Test Coverage:**
- Customer validation: 15 tests
- Job validation: 25 tests
- Job update validation: 4 tests

**Total:** 52 validation tests (part of 101 total frontend tests)

**Run Tests:**
```bash
cd frontend
npm test
```

### Backend Tests

All validators have comprehensive unit tests in `backend/GscTracking.Api.Tests/Validators/`:

**Test Coverage:**
- CustomerRequestDtoValidator: 17 tests
- JobRequestDtoValidator: 28 tests
- JobUpdateRequestDtoValidator: 4 tests

**Total:** 49 validator tests (part of 162 total backend tests)

**Run Tests:**
```bash
cd backend
dotnet test
```

## API Documentation

API endpoints include comprehensive validation documentation in Swagger/OpenAPI:

**Access Swagger UI:**
- Development: `http://localhost:5091/swagger`
- Staging: `https://your-staging-url/swagger`

Each endpoint includes:
- Required/optional field information
- Validation rules in remarks section
- Response status codes with descriptions
- Example request/response bodies

## Best Practices

1. **Always validate on both frontend and backend**
   - Frontend: Better UX with immediate feedback
   - Backend: Security and data integrity

2. **Keep validation rules in sync**
   - Frontend Zod schemas match backend FluentValidation rules
   - Same error messages on both sides

3. **Write tests for validation logic**
   - Test happy path (valid data)
   - Test error cases (invalid data)
   - Test edge cases (boundary values)

4. **Provide clear error messages**
   - Be specific about what's wrong
   - Suggest how to fix the issue
   - Use consistent language

5. **Handle validation errors gracefully**
   - Display errors inline near the field
   - Don't lose user's input on validation failure
   - Provide visual feedback (red border, error icon)

## Adding New Validation

To add validation for a new entity:

### 1. Frontend (Zod Schema)

Create schema in `frontend/src/lib/validations/entityName.schema.ts`:

```typescript
import { z } from 'zod'

export const entitySchema = z.object({
  field1: z.string().min(1, 'Field1 is required'),
  field2: z.number().min(0, 'Field2 must be >= 0'),
})

export type EntityFormValues = z.infer<typeof entitySchema>
```

Add tests in `entityName.schema.test.ts` and export from `index.ts`.

### 2. Backend (FluentValidation)

Create validator in `backend/GscTracking.Api/Validators/EntityRequestDtoValidator.cs`:

```csharp
using FluentValidation;
using GscTracking.Api.DTOs;

namespace GscTracking.Api.Validators;

public class EntityRequestDtoValidator : AbstractValidator<EntityRequestDto>
{
    public EntityRequestDtoValidator()
    {
        RuleFor(x => x.Field1)
            .NotEmpty().WithMessage("Field1 is required");

        RuleFor(x => x.Field2)
            .GreaterThanOrEqualTo(0).WithMessage("Field2 must be >= 0");
    }
}
```

Add tests in `backend/GscTracking.Api.Tests/Validators/EntityRequestDtoValidatorTests.cs`.

### 3. Update API Controller

Add validation documentation to controller action:

```csharp
/// <remarks>
/// Validation Rules:
/// - Field1: Required
/// - Field2: Must be >= 0
/// 
/// Returns 400 Bad Request if validation fails with detailed error messages.
/// </remarks>
/// <response code="201">Entity created successfully</response>
/// <response code="400">Validation error - check response for details</response>
[HttpPost]
[ProducesResponseType(typeof(EntityDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
public async Task<ActionResult<EntityDto>> CreateEntity([FromBody] EntityRequestDto request)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    // ... implementation
}
```

## Resources

- [Zod Documentation](https://zod.dev/)
- [React Hook Form Documentation](https://react-hook-form.com/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [shadcn/ui Form Component](https://ui.shadcn.com/docs/components/form)

## Troubleshooting

### Frontend validation not working

1. Check that the schema is imported correctly
2. Verify zodResolver is configured: `resolver: zodResolver(schema)`
3. Ensure form field names match schema keys
4. Check browser console for errors

### Backend validation not triggering

1. Verify FluentValidation is registered in `Program.cs`
2. Check validator is in the correct namespace
3. Ensure ModelState check is present in controller
4. Review API logs for validation errors

### Validation rules out of sync

1. Compare frontend schema with backend validator
2. Run both frontend and backend tests
3. Test end-to-end with API calls
4. Update documentation if rules change
