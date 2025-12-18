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
    public async Task UpdateExpense_ReturnsOkResult_WhenExpenseExists()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Updated oil filter",
            Amount = 29.99m,
            Date = DateTime.UtcNow,
            ReceiptReference = "REC002"
        };
        var updatedExpense = new ExpenseDto
        {
            Id = expenseId,
            JobId = jobId,
            Type = expenseRequest.Type,
            Description = expenseRequest.Description,
            Amount = expenseRequest.Amount,
            Date = expenseRequest.Date,
            ReceiptReference = expenseRequest.ReceiptReference
        };
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockExpenseService.Setup(s => s.UpdateExpenseAsync(expenseId, expenseRequest)).ReturnsAsync(updatedExpense);

        // Act
        var result = await _controller.UpdateExpense(jobId, expenseId, expenseRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedExpense = okResult.Value.Should().BeOfType<ExpenseDto>().Subject;
        returnedExpense.Description.Should().Be("Updated oil filter");
        returnedExpense.Amount.Should().Be(29.99m);
    }

    [Fact]
    public async Task UpdateExpense_ReturnsNotFound_WhenExpenseDoesNotExist()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 999;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Updated oil filter",
            Amount = 29.99m,
            Date = DateTime.UtcNow
        };
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync((ExpenseDto?)null);
        _mockExpenseService.Setup(s => s.UpdateExpenseAsync(expenseId, expenseRequest)).ReturnsAsync((ExpenseDto?)null);

        // Act
        var result = await _controller.UpdateExpense(jobId, expenseId, expenseRequest);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateExpense_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        var expenseRequest = new ExpenseRequestDto();
        _controller.ModelState.AddModelError("Type", "Type is required");

        // Act
        var result = await _controller.UpdateExpense(jobId, expenseId, expenseRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteExpense_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ThrowsAsync(new ArgumentException("Invalid expense"));
        _mockExpenseService.Setup(s => s.DeleteExpenseAsync(expenseId)).ThrowsAsync(new ArgumentException("Invalid expense"));

        // Act
        var result = await _controller.DeleteExpense(jobId, expenseId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateExpense_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        var expenseRequest = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Updated oil filter",
            Amount = 29.99m,
            Date = DateTime.UtcNow
        };
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockExpenseService.Setup(s => s.UpdateExpenseAsync(expenseId, expenseRequest))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateExpense(jobId, expenseId, expenseRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task DeleteExpense_ReturnsNoContent_WhenExpenseExists()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockExpenseService.Setup(s => s.DeleteExpenseAsync(expenseId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteExpense(jobId, expenseId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteExpense_ReturnsNotFound_WhenExpenseDoesNotExist()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 999;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync((ExpenseDto?)null);
        _mockExpenseService.Setup(s => s.DeleteExpenseAsync(expenseId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteExpense(jobId, expenseId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteExpense_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var expenseId = 1;
        _mockExpenseService.Setup(s => s.GetExpenseByIdAsync(expenseId)).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockExpenseService.Setup(s => s.DeleteExpenseAsync(expenseId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteExpense(jobId, expenseId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
