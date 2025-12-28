# GSC Tracking Clean Architecture

This document describes the Clean Architecture implementation for the GSC Tracking backend API.

## Overview

The GSC Tracking backend has been refactored to follow Clean Architecture principles, promoting separation of concerns, testability, and maintainability. The architecture is organized into four distinct layers:

## Architecture Layers

### 1. GscTracking.Core (Domain Layer)

**Purpose:** Contains the enterprise business rules and domain entities.

**Responsibilities:**
- Define domain entities (Customer, Job, JobUpdate, Expense)
- Define enums (JobStatus, ExpenseType)  
- Define repository interfaces (IRepository<T>, ICustomerRepository, etc.)
- No dependencies on other layers

**Key Principles:**
- Pure domain logic with no external dependencies
- Defines contracts (interfaces) that outer layers implement
- Represents the core business domain

**Structure:**
```
GscTracking.Core/
├── Entities/
│   ├── Customer.cs
│   ├── Job.cs
│   ├── JobUpdate.cs
│   └── Expense.cs
├── Enums/
│   ├── JobStatus.cs
│   └── ExpenseType.cs
└── Interfaces/
    ├── IRepository.cs
    ├── ICustomerRepository.cs
    ├── IJobRepository.cs
    ├── IExpenseRepository.cs
    └── IJobUpdateRepository.cs
```

### 2. GscTracking.Application (Application Layer)

**Purpose:** Contains application-specific business rules and orchestrates the flow of data.

**Responsibilities:**
- CQRS Commands and Queries using MediatR
- Command and Query handlers
- DTOs (Data Transfer Objects)
- Validation logic (FluentValidation)
- AutoMapper profiles
- Application-level interfaces

**Key Technologies:**
- **MediatR**: Implements CQRS pattern for separating reads and writes
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation

**Structure:**
```
GscTracking.Application/
├── Customers/
│   ├── Commands/
│   │   ├── CreateCustomerCommand.cs
│   │   ├── CreateCustomerCommandHandler.cs
│   │   ├── UpdateCustomerCommand.cs
│   │   ├── UpdateCustomerCommandHandler.cs
│   │   ├── DeleteCustomerCommand.cs
│   │   └── DeleteCustomerCommandHandler.cs
│   └── Queries/
│       ├── GetAllCustomersQuery.cs
│       ├── GetAllCustomersQueryHandler.cs
│       ├── GetCustomerByIdQuery.cs
│       └── GetCustomerByIdQueryHandler.cs
├── DTOs/
│   ├── CustomerDto.cs
│   ├── CustomerRequestDto.cs
│   ├── JobDto.cs
│   └── ... (other DTOs)
├── Mappings/ (for AutoMapper profiles)
├── Validators/ (for FluentValidation validators)
└── DependencyInjection.cs
```

### 3. GscTracking.Infrastructure (Infrastructure Layer)

**Purpose:** Implements external concerns like data access, file system, external services.

**Responsibilities:**
- Entity Framework Core DbContext
- Repository implementations
- Database migrations
- External service integrations
- CSV processing services

**Key Technologies:**
- **Entity Framework Core**: ORM for database access
- **Npgsql**: PostgreSQL provider
- **CsvHelper**: CSV file processing

**Structure:**
```
GscTracking.Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs
├── Repositories/
│   ├── Repository.cs (base implementation)
│   ├── CustomerRepository.cs
│   ├── JobRepository.cs
│   ├── ExpenseRepository.cs
│   └── JobUpdateRepository.cs
├── Services/
│   └── (external service implementations)
└── DependencyInjection.cs
```

### 4. GscTracking.Api (Presentation Layer)

**Purpose:** Handles HTTP requests and responses, acts as the entry point for the application.

**Responsibilities:**
- API Controllers (thin, orchestration only)
- Authentication and Authorization
- Request/Response handling
- Swagger/OpenAPI documentation
- Dependency injection configuration

**Key Principles:**
- Controllers should be thin - only orchestrate between MediatR and HTTP
- No business logic in controllers
- Proper error handling and status codes

**Structure:**
```
GscTracking.Api/
├── Controllers/
│   ├── CustomersController.cs (✅ Migrated to MediatR)
│   ├── JobsController.cs (✅ Migrated to MediatR)
│   ├── ExpensesController.cs (⏳ To be migrated)
│   └── ... (other controllers)
├── Utils/
├── Validators/ (API-level validators)
└── Program.cs (DI configuration)
```

## CQRS Pattern

The application uses the **Command Query Responsibility Segregation (CQRS)** pattern:

### Commands (Write Operations)
Commands represent operations that change state:
- `CreateCustomerCommand`
- `UpdateCustomerCommand`
- `DeleteCustomerCommand`

**Example:**
```csharp
public record CreateCustomerCommand(CustomerRequestDto CustomerRequest) : IRequest<CustomerDto>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repository;
    
    public CreateCustomerCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Business logic here
    }
}
```

### Queries (Read Operations)
Queries represent operations that retrieve data:
- `GetAllCustomersQuery`
- `GetCustomerByIdQuery`

**Example:**
```csharp
public record GetCustomerByIdQuery(int Id) : IRequest<CustomerDto?>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;
    
    public GetCustomerByIdQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        // Query logic here
    }
}
```

## Repository Pattern

The Repository Pattern provides abstraction over data access:

### Base Repository Interface
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
```

### Specific Repository Interfaces
```csharp
public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
}
```

## SOLID Principles Implementation

### Single Responsibility Principle (SRP)
- **Controllers**: Only handle HTTP concerns
- **Handlers**: Contain business logic for specific commands/queries
- **Repositories**: Only handle data access
- **Entities**: Only represent domain models

### Open/Closed Principle (OCP)
- New features are added through new Commands/Queries without modifying existing code
- Repository interfaces allow for different implementations without changing clients

### Liskov Substitution Principle (LSP)
- Any implementation of `IRepository<T>` can be substituted
- All implementations of `IRequestHandler` can be used interchangeably

### Interface Segregation Principle (ISP)
- Small, focused interfaces (`ICustomerRepository`, `IJobRepository`)
- Clients only depend on interfaces they actually use

### Dependency Inversion Principle (DIP)
- High-level modules (Application) depend on abstractions (Core interfaces)
- Low-level modules (Infrastructure) implement those abstractions
- Dependencies flow inward: Api → Application → Core ← Infrastructure

## Dependency Flow

```
┌─────────────────────────────────────────┐
│         GscTracking.Api                 │
│  (Controllers, Program.cs, Auth)        │
└────────────┬───────────────┬────────────┘
             │               │
             ↓               ↓
┌────────────────────┐   ┌──────────────────────┐
│  Application       │   │  Infrastructure      │
│  (Commands,        │   │  (Repositories,      │
│   Queries,         │   │   DbContext,         │
│   Handlers,        │   │   Services)          │
│   DTOs)            │   │                      │
└────────┬───────────┘   └──────────┬───────────┘
         │                          │
         │                          │
         └────────────┬─────────────┘
                      ↓
         ┌────────────────────────┐
         │      GscTracking.Core  │
         │  (Entities, Enums,     │
         │   Interfaces)          │
         └────────────────────────┘
```

## Migration Guide

### Adding a New Feature

1. **Define Command/Query** (Application layer)
   ```csharp
   public record CreateJobCommand(JobRequestDto JobRequest) : IRequest<JobDto>;
   ```

2. **Implement Handler** (Application layer)
   ```csharp
   public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, JobDto>
   {
       // Implementation
   }
   ```

3. **Update Controller** (Api layer)
   ```csharp
   [HttpPost]
   public async Task<ActionResult<JobDto>> CreateJob([FromBody] JobRequestDto request)
   {
       var command = new CreateJobCommand(request);
       var result = await _mediator.Send(command);
       return CreatedAtAction(nameof(GetJob), new { id = result.Id }, result);
   }
   ```

### Migrating Existing Controllers

To migrate a controller from the old service layer to CQRS:

1. Add MediatR to controller constructor
2. Replace service calls with MediatR `Send()` calls
3. Create Commands for write operations
4. Create Queries for read operations
5. Implement handlers for each command/query
6. Update tests to mock `IMediator`

**Example Migration:**
```csharp
// Before (Old Pattern)
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    
    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }
}

// After (Clean Architecture)
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetJobs()
    {
        var query = new GetAllJobsQuery();
        var jobs = await _mediator.Send(query);
        return Ok(jobs);
    }
}
```

## Testing

### Unit Testing Handlers
```csharp
[Fact]
public async Task Handle_CreateCustomer_ReturnsCustomerDto()
{
    // Arrange
    var mockRepo = new Mock<ICustomerRepository>();
    var handler = new CreateCustomerCommandHandler(mockRepo.Object);
    var command = new CreateCustomerCommand(new CustomerRequestDto { Name = "Test" });
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Name.Should().Be("Test");
}
```

### Testing Controllers
```csharp
[Fact]
public async Task CreateCustomer_ReturnsCreatedAtAction()
{
    // Arrange
    var mockMediator = new Mock<IMediator>();
    mockMediator.Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new CustomerDto { Id = 1, Name = "Test" });
    
    var controller = new CustomersController(mockMediator.Object, Mock.Of<ILogger<CustomersController>>());
    
    // Act
    var result = await controller.CreateCustomer(new CustomerRequestDto { Name = "Test" });
    
    // Assert
    result.Result.Should().BeOfType<CreatedAtActionResult>();
}
```

## Benefits of This Architecture

1. **Separation of Concerns**: Each layer has a clear responsibility
2. **Testability**: Business logic is isolated and easy to unit test
3. **Maintainability**: Changes in one layer don't affect others
4. **Flexibility**: Easy to swap implementations (e.g., different data stores)
5. **CQRS**: Optimized read and write operations
6. **Scalability**: Can scale different layers independently
7. **Domain-Driven Design**: Core layer represents the business domain

## Current Migration Status

### Completed ✅
- Core layer with all domain entities and interfaces
- Application layer with MediatR, DTOs, and DI configuration
- Infrastructure layer with repositories and DbContext
- CustomersController migrated to CQRS pattern
- JobsController migrated to CQRS pattern
- ExpensesController migrated to CQRS pattern
- JobUpdatesController migrated to CQRS pattern
- All tests updated and passing (318/318)

### Pending ⏳
- Migrate Import/Export Controllers to CQRS pattern
- Add AutoMapper profiles for entity-to-DTO mapping
- Add FluentValidation validators
- Refactor CsvService to fit Infrastructure layer

## Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
