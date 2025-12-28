using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Application.DTOs;
using GscTracking.Application.Expenses.Commands;
using GscTracking.Application.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class ExpensesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<ExpensesController>> _mockLogger;
    private readonly ExpensesController _controller;

    public ExpensesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<ExpensesController>>();
        _controller = new ExpensesController(_mockMediator.Object, _mockLogger.Object);
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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateExpenseCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedExpense);

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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((ExpenseDto?)null);
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateExpenseCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync((ExpenseDto?)null);

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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ArgumentException("Invalid expense"));
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteExpenseCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ArgumentException("Invalid expense"));

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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateExpenseCommand>(), It.IsAny<CancellationToken>()))
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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteExpenseCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((ExpenseDto?)null);
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteExpenseCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

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
        _mockMediator.Setup(m => m.Send(It.IsAny<GetExpenseByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ExpenseDto { Id = expenseId, JobId = jobId });
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteExpenseCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteExpense(jobId, expenseId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
