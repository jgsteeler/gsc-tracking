using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class ExpensesControllerTests
{
    private readonly Mock<IExpenseService> _mockExpenseService;
    private readonly Mock<ILogger<ExpensesController>> _mockLogger;
    private readonly ExpensesController _controller;

    public ExpensesControllerTests()
    {
        _mockExpenseService = new Mock<IExpenseService>();
        _mockLogger = new Mock<ILogger<ExpensesController>>();
        _controller = new ExpensesController(_mockExpenseService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetExpenses_ReturnsOkResult_WithExpenseList()
    {
        // Arrange
        var jobId = 1;
        var expenses = new List<ExpenseDto>
        {
            new ExpenseDto { Id = 1, JobId = jobId, Type = "Parts", Description = "Oil filter", Amount = 25.99m, Date = DateTime.UtcNow, ReceiptReference = "REC001" },
            new ExpenseDto { Id = 2, JobId = jobId, Type = "Labor", Description = "Labor hours", Amount = 150.00m, Date = DateTime.UtcNow }
        };
        _mockExpenseService.Setup(s => s.GetExpensesByJobIdAsync(jobId)).ReturnsAsync(expenses);

        // Act
        var result = await _controller.GetExpenses(jobId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedExpenses = okResult.Value.Should().BeAssignableTo<IEnumerable<ExpenseDto>>().Subject;
        returnedExpenses.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetExpenses_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        _mockExpenseService.Setup(s => s.GetExpensesByJobIdAsync(jobId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetExpenses(jobId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateExpense_ReturnsCreatedAtAction_WithValidData()
    {
        // Arrange
        var jobId = 1;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Oil filter",
            Amount = 25.99m,
            Date = DateTime.UtcNow,
            ReceiptReference = "REC001"
        };
        var createdExpense = new ExpenseDto
        {
            Id = 1,
            JobId = jobId,
            Type = expenseRequest.Type,
            Description = expenseRequest.Description,
            Amount = expenseRequest.Amount,
            Date = expenseRequest.Date,
            ReceiptReference = expenseRequest.ReceiptReference
        };
        _mockExpenseService.Setup(s => s.CreateExpenseAsync(jobId, expenseRequest)).ReturnsAsync(createdExpense);

        // Act
        var result = await _controller.CreateExpense(jobId, expenseRequest);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetExpenseById));
        var returnedExpense = createdAtActionResult.Value.Should().BeOfType<ExpenseDto>().Subject;
        returnedExpense.Id.Should().Be(1);
        returnedExpense.Type.Should().Be("Parts");
    }

    [Fact]
    public async Task CreateExpense_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var jobId = 1;
        var expenseRequest = new ExpenseRequestDto();
        _controller.ModelState.AddModelError("Type", "Type is required");

        // Act
        var result = await _controller.CreateExpense(jobId, expenseRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateExpense_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "InvalidType",
            Description = "Test expense",
            Amount = 100.00m,
            Date = DateTime.UtcNow
        };
        _mockExpenseService.Setup(s => s.CreateExpenseAsync(jobId, expenseRequest))
            .ThrowsAsync(new ArgumentException("Invalid expense type"));

        // Act
        var result = await _controller.CreateExpense(jobId, expenseRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateExpense_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Oil filter",
            Amount = 25.99m,
            Date = DateTime.UtcNow
        };
        _mockExpenseService.Setup(s => s.CreateExpenseAsync(jobId, expenseRequest))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateExpense(jobId, expenseRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetExpenseById_ReturnsOkResult_WhenExpenseExists()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        var expense = new ExpenseDto
        {
            Id = expenseId,
            JobId = jobId,
            Type = "Parts",
            Description = "Oil filter",
            Amount = 25.99m,
            Date = DateTime.UtcNow,
            ReceiptReference = "REC001"
        };
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(expense);

        // Act
        var result = await _controller.GetExpenseById(jobId, expenseId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedExpense = okResult.Value.Should().BeOfType<ExpenseDto>().Subject;
        returnedExpense.Id.Should().Be(expenseId);
        returnedExpense.Type.Should().Be("Parts");
    }

    [Fact]
    public async Task GetExpenseById_ReturnsNotFound_WhenExpenseDoesNotExist()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 999;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync((ExpenseDto?)null);

        // Act
        var result = await _controller.GetExpenseById(jobId, expenseId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetExpenseById_ReturnsNotFound_WhenExpenseBelongsToDifferentJob()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        var expense = new ExpenseDto
        {
            Id = expenseId,
            JobId = 2, // Different job ID
            Type = "Parts",
            Description = "Oil filter",
            Amount = 25.99m,
            Date = DateTime.UtcNow
        };
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(expense);

        // Act
        var result = await _controller.GetExpenseById(jobId, expenseId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetExpenseById_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetExpenseById(jobId, expenseId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
