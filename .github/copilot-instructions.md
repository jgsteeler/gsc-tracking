# GitHub Copilot Instructions for GSC Tracking

This document provides comprehensive guidelines for GitHub Copilot when working on the GSC Small Engine Repair Business Management Application.

## ⚠️ CRITICAL: Commit Message Format

**EVERY COMMIT MUST FOLLOW CONVENTIONAL COMMITS FORMAT**

This is **REQUIRED** for Release Please to work. All commits are validated by commitlint.

### Required Format
```
<type>(<scope>): <description>
```

### Examples
```
feat(customer): add search functionality
fix(api): resolve null reference  
docs(readme): update setup guide
chore: add commitlint configuration
```

### NEVER use these formats:
- ❌ "Initial plan"
- ❌ "WIP"
- ❌ "Update files"
- ❌ "Fix bug"

See [COMMIT_GUIDELINES.md](../COMMIT_GUIDELINES.md) for complete details.

## Project Overview

GSC Tracking is a full-stack business management application for a small engine repair shop. It manages customers, jobs, equipment, expenses, and integrates with Wave accounting software. The project is designed to be extensible to other GSC divisions (GSC-PROD, GSC-AI, GSC-DEV).

**Tech Stack:**
- **Backend:** .NET 10 Web API (C#) with Entity Framework Core
- **Frontend:** React 19 + Vite + TypeScript
- **Database:** Azure SQL Database (or PostgreSQL)
- **Authentication:** Auth0
- **Hosting:** Azure App Service (containerized)
- **Storage:** Azure Blob Storage

## Repository Structure

```
gsc-tracking/
├── backend/
│   └── GscTracking.Api/          # .NET 10 Web API project
│       ├── Controllers/           # API controllers
│       ├── Models/                # Entity models
│       ├── Data/                  # DbContext and migrations
│       ├── Services/              # Business logic services
│       ├── DTOs/                  # Data transfer objects
│       └── Program.cs             # Application entry point
├── frontend/
│   ├── src/
│   │   ├── components/           # React components
│   │   ├── pages/                # Page components
│   │   ├── hooks/                # Custom React hooks
│   │   ├── services/             # API service layer
│   │   ├── types/                # TypeScript type definitions
│   │   └── utils/                # Utility functions
│   └── public/                   # Static assets
├── .github/
│   ├── ISSUE_TEMPLATE/           # Issue templates
│   └── copilot-instructions.md  # This file
└── docs/                         # Documentation
```

## Build and Development Commands

### Backend (.NET 10)
```bash
# Navigate to backend
cd backend/GscTracking.Api

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the API (development)
dotnet run

# Run tests (when available)
dotnet test

# Create database migration
dotnet ef migrations add <MigrationName>

# Apply database migrations
dotnet ef database update
```

The API runs at `http://localhost:5091` (or `https://localhost:7075` with HTTPS profile).

### Frontend (React + Vite)
```bash
# Navigate to frontend
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Lint the code
npm run lint

# Preview production build
npm run preview
```

The frontend runs at `http://localhost:5173`.

## Coding Standards and Conventions

### .NET Backend

**Naming Conventions:**
- Use PascalCase for classes, methods, properties, and namespaces
- Use camelCase for local variables and parameters
- Use UPPER_CASE for constants
- Prefix interfaces with "I" (e.g., `ICustomerService`)
- Use descriptive, meaningful names

**Code Organization:**
- Keep controllers thin; move business logic to services
- Use dependency injection for services
- Separate concerns: Controllers → Services → Repositories → Data
- Place validation logic in separate validators (use FluentValidation when added)

**API Design:**
- Follow RESTful conventions
- Use plural nouns for resource endpoints (e.g., `/api/customers`, `/api/jobs`)
- Use appropriate HTTP methods (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 204, 400, 404, 500)
- Use DTOs for request/response models, not domain entities
- API versioning: Use URL path versioning (e.g., `/api/v1/customers`) when needed

**Error Handling:**
- Use try-catch blocks in controllers
- Return ProblemDetails for errors
- Log exceptions with appropriate log levels
- Return meaningful error messages to clients
- Use custom exceptions for business logic errors

**Entity Framework Core:**
- Use Code First approach with migrations
- Configure entities using Fluent API in `OnModelCreating`
- Use async methods for all database operations
- Implement proper navigation properties for relationships
- Consider soft deletes for important entities (add `IsDeleted` and `DeletedAt` properties)

**Validation:**
- Use Data Annotations for simple validation
- Use FluentValidation for complex validation rules (when added)
- Validate at both client and server sides
- Return 400 Bad Request with validation errors

**Security:**
- Never expose sensitive data in responses
- Use [Authorize] attribute for protected endpoints
- Validate all user inputs
- Use parameterized queries (EF Core handles this)
- Implement CORS properly (already configured for localhost:5173)

### React/TypeScript Frontend

**Naming Conventions:**
- Use PascalCase for component files and component names (e.g., `CustomerList.tsx`)
- Use camelCase for functions, variables, and file names (non-components)
- Use kebab-case for CSS/SCSS files
- Use UPPER_SNAKE_CASE for constants
- Prefix custom hooks with "use" (e.g., `useCustomerData`)

**Component Structure:**
- Prefer functional components with hooks
- Keep components small and focused (single responsibility)
- Extract reusable logic into custom hooks
- Use TypeScript for all components and functions
- Define prop types using TypeScript interfaces

**Type Safety:**
- Define explicit types for all props, state, and function parameters
- Avoid using `any` type; use `unknown` if type is truly unknown
- Create shared type definitions in `src/types/`
- Use type inference where appropriate
- Define API response types that match backend DTOs

**State Management:**
- Use React hooks (useState, useEffect, useContext) for local state
- Consider Context API for shared state across components
- Use custom hooks for complex state logic
- Keep state close to where it's used

**API Integration:**
- Create API service functions in `src/services/`
- Use async/await for API calls
- Handle loading, error, and success states
- Use try-catch for error handling
- Consider using React Query or SWR for data fetching (when added)

**Styling:**
- Use CSS modules or styled-components (to be determined)
- Follow mobile-first responsive design
- Use consistent spacing and color variables
- When shadcn/ui is added, use those components as the foundation

**Form Handling:**
- Use React Hook Form (when added)
- Use Zod for validation schemas (when added)
- Provide clear validation error messages
- Disable submit button while submitting
- Show loading indicators during submission

**Error Handling:**
- Display user-friendly error messages
- Log errors to console in development
- Use error boundaries for component-level error handling
- Provide fallback UI for errors

## Testing Guidelines

### Backend Tests
- Write unit tests for services and business logic
- Write integration tests for API endpoints
- Use xUnit as the testing framework
- Mock dependencies using Moq
- Aim for >80% code coverage on critical paths
- Test happy path and error scenarios

### Frontend Tests
- Write unit tests for utility functions and hooks
- Write component tests using React Testing Library
- Write integration tests for user flows
- Use Jest as the test runner
- Test user interactions, not implementation details
- Aim for >70% code coverage

## Database Guidelines

**Schema Design:**
- Use singular names for tables (e.g., Customer, Job, not Customers, Jobs)
- Use `Id` as primary key name (PascalCase)
- Use foreign keys with appropriate names (e.g., `CustomerId`)
- Add indexes for frequently queried columns
- Use appropriate data types (varchar vs nvarchar, datetime2, etc.)
- Add NOT NULL constraints where appropriate
- Include audit fields: `CreatedAt`, `UpdatedAt`, `DeletedAt`

**Migrations:**
- Create migrations with descriptive names
- Review generated migrations before applying
- Never modify applied migrations
- Add new migrations for schema changes
- Test migrations in development before production

## Authentication and Authorization

**Auth0 Integration (when implemented):**
- Store tokens securely (HttpOnly cookies or secure storage)
- Refresh tokens before expiration
- Implement proper logout functionality
- Use Auth0 user roles for authorization
- Backend: Validate JWT tokens on protected endpoints
- Frontend: Include JWT in Authorization header for API calls

## Performance Best Practices

**Backend:**
- Use async/await for I/O operations
- Implement pagination for list endpoints
- Use eager loading (.Include) or explicit loading to avoid N+1 queries
- Cache frequently accessed data when appropriate
- Use projection (Select) to return only needed fields

**Frontend:**
- Lazy load routes and heavy components
- Optimize images and assets
- Implement virtual scrolling for long lists
- Debounce search inputs
- Memoize expensive computations with useMemo
- Use React.memo for components that don't need frequent re-renders

## Key Documentation References

- [Business Analysis Document](../business-management-app-analysis.md) - Comprehensive requirements and architecture
- [Setup Instructions](../SETUP-INSTRUCTIONS.md) - GitHub project setup guide
- [Issues Specifications](../ISSUES.md) - Detailed issue descriptions
- [Backend README](../backend/README.md) - Backend-specific documentation
- [Frontend README](../frontend/README.md) - Frontend-specific documentation

## Development Workflow

1. **Starting New Work:**
   - Create a new branch from main
   - Follow the issue template and acceptance criteria
   - Review relevant documentation before coding

2. **Making Changes:**
   - Write code following the conventions above
   - Add/update tests for your changes
   - Run linters and fix any issues
   - Test locally before committing

3. **Before Committing:**
   - Run backend: `dotnet build` and `dotnet test`
   - Run frontend: `npm run build` and `npm run lint`
   - Ensure all tests pass
   - Review your changes

4. **Pull Request:**
   - Provide clear description of changes
   - Reference related issues
   - Ensure CI/CD passes
   - Request reviews as needed

## Git Commit and PR Conventions

This project uses **Conventional Commits** to enable automated releases with Release Please and maintain a clear, searchable Git history.

### Commit Message Format

Every commit message **MUST** follow this structure:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Examples:**
```
feat(customer): add customer search functionality
fix(api): resolve null reference in job controller
docs(readme): update setup instructions
chore(deps): upgrade Entity Framework to 9.0.1
```

### Commit Types

Use these standardized commit types:

| Type       | Description                                      | Version Bump |
|------------|--------------------------------------------------|--------------|
| `feat`     | New feature                                      | Minor        |
| `fix`      | Bug fix                                          | Patch        |
| `docs`     | Documentation only changes                       | None         |
| `style`    | Code style changes (formatting, no logic change) | None         |
| `refactor` | Code restructuring (no behavior change)          | None         |
| `perf`     | Performance improvements                         | Patch        |
| `test`     | Adding or updating tests                         | None         |
| `build`    | Build system or external dependency changes      | None         |
| `ci`       | CI/CD configuration changes                      | None         |
| `chore`    | Other changes (tooling, configs, etc.)           | None         |
| `revert`   | Reverts a previous commit                        | Depends      |

### Scope (Optional)

Scope specifies the area of the codebase affected:

- `customer` - Customer management features
- `job` - Job tracking features
- `equipment` - Equipment management
- `expense` - Expense tracking
- `api` - API layer changes
- `db` - Database changes
- `auth` - Authentication/authorization
- `ui` - UI/UX changes
- `deps` - Dependency updates

**Examples with scope:**
```
feat(job): add job status filtering
fix(customer): correct email validation regex
refactor(api): extract common error handling
```

### Commit Description Rules

1. **Use imperative mood**: "add feature" not "added feature" or "adds feature"
2. **Start with lowercase**: unless it's a proper noun
3. **No period at the end**
4. **Keep it under 72 characters**
5. **Be specific and descriptive**

**Good:**
- `feat(auth): add JWT token refresh mechanism`
- `fix(job): prevent duplicate job creation`
- `docs(api): add OpenAPI documentation`

**Bad:**
- `Added some stuff` (vague, wrong tense)
- `Fix bug` (not specific)
- `Updated files.` (period at end, not descriptive)

### Breaking Changes

For breaking changes that require major version bump:

1. Add `!` after type/scope: `feat(api)!: remove deprecated endpoints`
2. OR add `BREAKING CHANGE:` in the footer:

```
feat(api): restructure customer endpoint response

BREAKING CHANGE: Customer API now returns nested address object instead of flat fields.
Migration guide available in docs/migrations/v2.md
```

### Commit Body (Optional)

Use the body to explain:
- **Why** the change was made
- **What** problem it solves
- **How** it differs from previous behavior

**Example:**
```
fix(job): prevent race condition in status updates

Job status updates could be lost when multiple requests
arrived simultaneously. Added optimistic locking using
Entity Framework's RowVersion to ensure consistency.

Fixes #123
```

### Commit Footer

Use footers to:
- Reference issues: `Fixes #123`, `Closes #456`, `Refs #789`
- Note breaking changes: `BREAKING CHANGE: description`
- Add co-authors: `Co-authored-by: Name <email>`

### Pull Request Requirements

1. **Title Format**: Use conventional commit format
   - `feat(customer): add bulk import from CSV`
   - `fix(api): resolve CORS issues with frontend`

2. **Description Must Include:**
   - Summary of changes
   - Motivation and context
   - Related issues (use `Fixes #123` or `Closes #456`)
   - Testing performed
   - Breaking changes (if any)

3. **PR Labels**: Add appropriate labels matching commit type
   - `enhancement` for `feat`
   - `bug` for `fix`
   - `documentation` for `docs`

### Examples of Good Commits

```bash
# Feature addition
feat(customer): add customer notes field
feat(job): implement job photo attachments
feat(api): add pagination to customer list endpoint

# Bug fixes
fix(auth): correct token expiration handling
fix(customer): validate phone number format
fix(job): prevent negative labor hours

# Documentation
docs(readme): add Azure deployment guide
docs(api): document authentication flow

# Refactoring
refactor(customer): extract validation logic to service
refactor(api): simplify error handling middleware

# Performance
perf(db): add index on job status column
perf(api): implement response caching

# Dependencies
chore(deps): update React to 19.2.1
build(deps): bump .NET SDK to 10.0.1

# Breaking changes
feat(api)!: change customer ID from int to GUID

BREAKING CHANGE: Customer IDs are now GUIDs instead of integers.
Update all API calls to use the new format.
```

### Validation

These commit conventions are **enforced** for:
- Automated changelog generation
- Semantic versioning with Release Please
- Clear project history and easier debugging

All commits will be validated, and PRs with non-conforming commits may be rejected.

## Common Patterns and Examples

### Backend API Endpoint Example
```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "An error occurred while retrieving customers.");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }
}
```

### Frontend Component Example
```typescript
import { useState, useEffect } from 'react';
import { Customer } from '../types/customer';
import { customerService } from '../services/customerService';

interface CustomerListProps {
  onCustomerSelect?: (customer: Customer) => void;
}

const CustomerList: React.FC<CustomerListProps> = ({ onCustomerSelect }) => {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCustomers = async () => {
      try {
        setLoading(true);
        const data = await customerService.getAll();
        setCustomers(data);
      } catch (err) {
        setError('Failed to load customers');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchCustomers();
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="customer-list">
      {customers.map(customer => (
        <div key={customer.id} onClick={() => onCustomerSelect?.(customer)}>
          {customer.name}
        </div>
      ))}
    </div>
  );
};

export default CustomerList;
```

## Important Notes

- **Code Quality:** Prioritize readable, maintainable code over clever solutions
- **Documentation:** Add comments for complex logic; code should be self-documenting otherwise
- **Dependencies:** Only add necessary dependencies; review licenses and security
- **Security:** Never commit secrets, API keys, or sensitive data
- **Git Commits:** MUST follow Conventional Commits format (see Git Commit and PR Conventions section)
- **Breaking Changes:** Mark with `!` or `BREAKING CHANGE:` footer; coordinate with team; update documentation

## Future Considerations

- Wave API integration (with custom GraphQL abstraction layer over Wave's REST API)
- Inventory management features
- Photo/document uploads to Azure Blob Storage
- Customer portal (separate React app or integrated)
- Multi-division support (multi-tenancy)
- Advanced reporting and analytics
- Mobile responsiveness and PWA features

## Getting Help

- Review existing code for patterns and conventions
- Check the business analysis document for requirements context
- Refer to the issues list for detailed feature specifications
- Consult .NET and React documentation for framework-specific questions
- Ask for clarification on ambiguous requirements

---

**Last Updated:** 2025-12-08  
**Maintained By:** GSC Development Team
