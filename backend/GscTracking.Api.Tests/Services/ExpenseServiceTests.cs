using FluentAssertions;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;
using GscTracking.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GscTracking.Api.Tests.Services;

public class ExpenseServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ExpenseService _expenseService;
    private readonly Customer _testCustomer;
    private readonly Job _testJob;

    public ExpenseServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _expenseService = new ExpenseService(_context);

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
    public async Task GetExpensesByJobIdAsync_ReturnsAllExpensesForJob()
    {
        // Arrange
        var expenses = new List<Expense>
        {
            new Expense { Id = 1, JobId = 1, Type = ExpenseType.Parts, Description = "Oil filter", Amount = 10.50m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Expense { Id = 2, JobId = 1, Type = ExpenseType.Labor, Description = "Labor", Amount = 50.00m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Expense.AddRange(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _expenseService.GetExpensesByJobIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Description == "Oil filter");
        result.Should().Contain(e => e.Description == "Labor");
    }

    [Fact]
    public async Task GetExpensesByJobIdAsync_ReturnsEmptyList_WhenNoExpenses()
    {
        // Act
        var result = await _expenseService.GetExpensesByJobIdAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExpenseByIdAsync_ReturnsExpense_WhenExpenseExists()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1,
            JobId = 1,
            Type = ExpenseType.Parts,
            Description = "Oil filter",
            Amount = 10.50m,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Expense.Add(expense);
        await _context.SaveChangesAsync();

        // Act
        var result = await _expenseService.GetExpenseByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("Oil filter");
        result.Amount.Should().Be(10.50m);
    }

    [Fact]
    public async Task GetExpenseByIdAsync_ReturnsNull_WhenExpenseDoesNotExist()
    {
        // Act
        var result = await _expenseService.GetExpenseByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateExpenseAsync_CreatesExpense_WhenValidData()
    {
        // Arrange
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Air filter",
            Amount = 15.00m,
            Date = DateTime.UtcNow,
            ReceiptReference = "REC-001"
        };

        // Act
        var result = await _expenseService.CreateExpenseAsync(1, expenseRequest);

        // Assert
        result.Should().NotBeNull();
        result.JobId.Should().Be(1);
        result.Description.Should().Be("Air filter");
        result.Amount.Should().Be(15.00m);
        result.Type.Should().Be("Parts");
        result.ReceiptReference.Should().Be("REC-001");

        var expenseInDb = await _context.Expense.FindAsync(result.Id);
        expenseInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateExpenseAsync_ThrowsArgumentException_WhenJobDoesNotExist()
    {
        // Arrange
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Air filter",
            Amount = 15.00m,
            Date = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _expenseService.CreateExpenseAsync(999, expenseRequest));
    }

    [Fact]
    public async Task CreateExpenseAsync_ThrowsArgumentException_WhenInvalidType()
    {
        // Arrange
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "InvalidType",
            Description = "Air filter",
            Amount = 15.00m,
            Date = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _expenseService.CreateExpenseAsync(1, expenseRequest));
    }

    [Fact]
    public async Task UpdateExpenseAsync_UpdatesExpense_WhenValidData()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1,
            JobId = 1,
            Type = ExpenseType.Parts,
            Description = "Old description",
            Amount = 10.00m,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Expense.Add(expense);
        await _context.SaveChangesAsync();

        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Labor",
            Description = "New description",
            Amount = 20.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _expenseService.UpdateExpenseAsync(1, expenseRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("New description");
        result.Amount.Should().Be(20.00m);
        result.Type.Should().Be("Labor");
    }

    [Fact]
    public async Task UpdateExpenseAsync_ReturnsNull_WhenExpenseDoesNotExist()
    {
        // Arrange
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "New description",
            Amount = 20.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = await _expenseService.UpdateExpenseAsync(999, expenseRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteExpenseAsync_DeletesExpense_WhenExpenseExists()
    {
        // Arrange
        var expense = new Expense
        {
            Id = 1,
            JobId = 1,
            Type = ExpenseType.Parts,
            Description = "Oil filter",
            Amount = 10.00m,
            Date = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Expense.Add(expense);
        await _context.SaveChangesAsync();

        // Act
        var result = await _expenseService.DeleteExpenseAsync(1);

        // Assert
        result.Should().BeTrue();
        var expenseInDb = await _context.Expense.FindAsync(1);
        expenseInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteExpenseAsync_ReturnsFalse_WhenExpenseDoesNotExist()
    {
        // Act
        var result = await _expenseService.DeleteExpenseAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateTotalCostAsync_ReturnsSumOfExpenses()
    {
        // Arrange
        var expenses = new List<Expense>
        {
            new Expense { Id = 1, JobId = 1, Type = ExpenseType.Parts, Description = "Part 1", Amount = 10.50m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Expense { Id = 2, JobId = 1, Type = ExpenseType.Labor, Description = "Labor", Amount = 50.00m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Expense { Id = 3, JobId = 1, Type = ExpenseType.Service, Description = "Service", Amount = 25.75m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Expense.AddRange(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _expenseService.CalculateTotalCostAsync(1);

        // Assert
        result.Should().Be(86.25m);
    }

    [Fact]
    public async Task CalculateTotalCostAsync_ReturnsZero_WhenNoExpenses()
    {
        // Act
        var result = await _expenseService.CalculateTotalCostAsync(1);

        // Assert
        result.Should().Be(0m);
    }
}
