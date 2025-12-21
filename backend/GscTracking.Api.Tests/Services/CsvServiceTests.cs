using System.Text;
using FluentAssertions;
using FluentValidation;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Services;

public class CsvServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ExpenseService _expenseService;
    private readonly JobService _jobService;
    private readonly CsvService _csvService;
    private readonly IValidator<ExpenseImportDto> _validator;
    private readonly Customer _testCustomer;
    private readonly Job _testJob;

    public CsvServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _expenseService = new ExpenseService(_context);
        _jobService = new JobService(_context);
        _validator = new ExpenseImportDtoValidator();
        
        var mockLogger = new Mock<ILogger<CsvService>>();
        _csvService = new CsvService(_expenseService, _jobService, _validator, mockLogger.Object);

        // Create a test customer for foreign key relationships
        _testCustomer = new Customer
        {
            Id = 1,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "1234567890",
            Address = "123 Test St",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Customer.Add(_testCustomer);

        // Create a test job for foreign key relationships
        _testJob = new Job
        {
            Id = 1,
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = JobStatus.Quote,
            DateReceived = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Job.Add(_testJob);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void ExportExpenses_ReturnsValidCsvData()
    {
        // Arrange
        var expenses = new List<ExpenseDto>
        {
            new ExpenseDto
            {
                Id = 1,
                JobId = 1,
                Type = "Parts",
                Description = "Oil filter",
                Amount = 15.99m,
                Date = new DateTime(2025, 1, 15),
                ReceiptReference = "REC-001",
                CreatedAt = new DateTime(2025, 1, 15),
                UpdatedAt = new DateTime(2025, 1, 15)
            },
            new ExpenseDto
            {
                Id = 2,
                JobId = 1,
                Type = "Labor",
                Description = "Oil change service",
                Amount = 45.00m,
                Date = new DateTime(2025, 1, 15),
                CreatedAt = new DateTime(2025, 1, 15),
                UpdatedAt = new DateTime(2025, 1, 15)
            }
        };

        // Act
        var result = _csvService.ExportExpenses(expenses);
        var csvContent = Encoding.UTF8.GetString(result);

        // Assert
        result.Should().NotBeNull();
        csvContent.Should().Contain("Expense ID");
        csvContent.Should().Contain("Job ID");
        csvContent.Should().Contain("Type");
        csvContent.Should().Contain("Description");
        csvContent.Should().Contain("Amount");
        csvContent.Should().Contain("Oil filter");
        csvContent.Should().Contain("Oil change service");
        csvContent.Should().Contain("15.99");
        csvContent.Should().Contain("45.00");
    }

    [Fact]
    public void ExportJobs_ReturnsValidCsvData()
    {
        // Arrange
        var jobs = new List<JobDto>
        {
            new JobDto
            {
                Id = 1,
                CustomerId = 1,
                CustomerName = "John Doe",
                EquipmentType = "Lawn Mower",
                EquipmentModel = "Honda HRX217",
                Description = "Annual maintenance",
                Status = "Completed",
                DateReceived = new DateTime(2025, 1, 10),
                DateCompleted = new DateTime(2025, 1, 15),
                EstimateAmount = 150.00m,
                ActualAmount = 145.50m,
                TotalCost = 85.25m,
                ProfitMargin = 60.25m,
                CreatedAt = new DateTime(2025, 1, 10),
                UpdatedAt = new DateTime(2025, 1, 15)
            }
        };

        // Act
        var result = _csvService.ExportJobs(jobs);
        var csvContent = Encoding.UTF8.GetString(result);

        // Assert
        result.Should().NotBeNull();
        csvContent.Should().Contain("Job ID");
        csvContent.Should().Contain("Customer Name");
        csvContent.Should().Contain("Equipment Type");
        csvContent.Should().Contain("Status");
        csvContent.Should().Contain("John Doe");
        csvContent.Should().Contain("Lawn Mower");
        csvContent.Should().Contain("Honda HRX217");
        csvContent.Should().Contain("Annual maintenance");
    }

    [Fact]
    public async Task ImportExpensesAsync_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
1,Parts,Oil filter,15.99,2025-01-15,REC-001
1,Labor,Oil change service,45.00,2025-01-15,";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(2);
        result.ErrorCount.Should().Be(0);
        result.Errors.Should().BeEmpty();

        // Verify expenses were created in the database
        var expenses = await _expenseService.GetExpensesByJobIdAsync(1);
        expenses.Should().HaveCount(2);
    }

    [Fact]
    public async Task ImportExpensesAsync_WithInvalidJobId_ReturnsErrorResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
999,Parts,Oil filter,15.99,2025-01-15,REC-001";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(0);
        result.ErrorCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Job with ID 999 not found");
        result.Errors[0].LineNumber.Should().Be(2);
    }

    [Fact]
    public async Task ImportExpensesAsync_WithInvalidType_ReturnsErrorResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
1,InvalidType,Oil filter,15.99,2025-01-15,REC-001";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(0);
        result.ErrorCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Validation failed");
        result.Errors[0].Message.Should().Contain("Expense type must be one of: Parts, Labor, Service");
    }

    [Fact]
    public async Task ImportExpensesAsync_WithInvalidAmount_ReturnsErrorResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
1,Parts,Oil filter,-15.99,2025-01-15,REC-001";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(0);
        result.ErrorCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Amount must be greater than 0");
    }

    [Fact]
    public async Task ImportExpensesAsync_WithMixedValidAndInvalidData_ReturnsPartialSuccessResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
1,Parts,Oil filter,15.99,2025-01-15,REC-001
999,Labor,Invalid job,45.00,2025-01-15,
1,Labor,Oil change service,45.00,2025-01-15,";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(2);
        result.ErrorCount.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Job with ID 999 not found");
        result.Errors[0].LineNumber.Should().Be(3);
    }

    [Fact]
    public async Task ImportExpensesAsync_WithEmptyDescription_ReturnsErrorResult()
    {
        // Arrange
        var csvContent = @"Job ID,Type,Description,Amount,Date,Receipt Reference
1,Parts,,15.99,2025-01-15,REC-001";
        
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ImportExpensesAsync(stream);

        // Assert
        result.Should().NotBeNull();
        result.SuccessCount.Should().Be(0);
        result.ErrorCount.Should().Be(1);
        result.Errors[0].Message.Should().Contain("Description is required");
    }

    [Fact]
    public void ExportExpenses_WithEmptyList_ReturnsValidCsvWithHeadersOnly()
    {
        // Arrange
        var expenses = new List<ExpenseDto>();

        // Act
        var result = _csvService.ExportExpenses(expenses);
        var csvContent = Encoding.UTF8.GetString(result);

        // Assert
        result.Should().NotBeNull();
        csvContent.Should().Contain("Expense ID");
        csvContent.Should().Contain("Job ID");
        csvContent.Should().Contain("Type");
    }
}
