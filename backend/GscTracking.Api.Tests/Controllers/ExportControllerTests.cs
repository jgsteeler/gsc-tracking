using System.Text;
using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class ExportControllerTests
{
    private readonly Mock<ICsvService> _mockCsvService;
    private readonly Mock<IExpenseService> _mockExpenseService;
    private readonly Mock<IJobService> _mockJobService;
    private readonly Mock<ILogger<ExportController>> _mockLogger;
    private readonly ExportController _controller;

    public ExportControllerTests()
    {
        _mockCsvService = new Mock<ICsvService>();
        _mockExpenseService = new Mock<IExpenseService>();
        _mockJobService = new Mock<IJobService>();
        _mockLogger = new Mock<ILogger<ExportController>>();
        _controller = new ExportController(
            _mockCsvService.Object,
            _mockExpenseService.Object,
            _mockJobService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExportExpenses_WithoutJobIdFilter_ReturnsAllExpensesAsCsv()
    {
        // Arrange
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda", Description = "Test", Status = "Quote", DateReceived = DateTime.UtcNow, TotalCost = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new JobDto { Id = 2, CustomerId = 1, CustomerName = "Jane Doe", EquipmentType = "Chainsaw", EquipmentModel = "Stihl", Description = "Test", Status = "Quote", DateReceived = DateTime.UtcNow, TotalCost = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var expenses1 = new List<ExpenseDto>
        {
            new ExpenseDto { Id = 1, JobId = 1, Type = "Parts", Description = "Oil", Amount = 10m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var expenses2 = new List<ExpenseDto>
        {
            new ExpenseDto { Id = 2, JobId = 2, Type = "Labor", Description = "Work", Amount = 50m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var csvData = Encoding.UTF8.GetBytes("test,csv,data");

        _mockJobService.Setup(s => s.GetAllJobsAsync(null, null)).ReturnsAsync(jobs);
        _mockExpenseService.Setup(s => s.GetExpensesByJobIdAsync(1)).ReturnsAsync(expenses1);
        _mockExpenseService.Setup(s => s.GetExpensesByJobIdAsync(2)).ReturnsAsync(expenses2);
        _mockCsvService.Setup(s => s.ExportExpenses(It.IsAny<IEnumerable<ExpenseDto>>())).Returns(csvData);

        // Act
        var result = await _controller.ExportExpenses();

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/csv");
        fileResult.FileContents.Should().Equal(csvData);
        fileResult.FileDownloadName.Should().Contain("expenses_export_");
    }

    [Fact]
    public async Task ExportExpenses_WithJobIdFilter_ReturnsFilteredExpensesAsCsv()
    {
        // Arrange
        var jobId = 1;
        var expenses = new List<ExpenseDto>
        {
            new ExpenseDto { Id = 1, JobId = jobId, Type = "Parts", Description = "Oil", Amount = 10m, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var csvData = Encoding.UTF8.GetBytes("test,csv,data");

        _mockExpenseService.Setup(s => s.GetExpensesByJobIdAsync(jobId)).ReturnsAsync(expenses);
        _mockCsvService.Setup(s => s.ExportExpenses(expenses)).Returns(csvData);

        // Act
        var result = await _controller.ExportExpenses(jobId);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/csv");
        fileResult.FileContents.Should().Equal(csvData);
        fileResult.FileDownloadName.Should().Contain($"expenses_job_{jobId}_export_");
    }

    [Fact]
    public async Task ExportExpenses_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockJobService.Setup(s => s.GetAllJobsAsync(null, null)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ExportExpenses();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task ExportJobs_WithoutStatusFilter_ReturnsAllJobsAsCsv()
    {
        // Arrange
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda", Description = "Test", Status = "Quote", DateReceived = DateTime.UtcNow, TotalCost = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new JobDto { Id = 2, CustomerId = 1, CustomerName = "Jane Doe", EquipmentType = "Chainsaw", EquipmentModel = "Stihl", Description = "Test", Status = "Completed", DateReceived = DateTime.UtcNow, TotalCost = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var csvData = Encoding.UTF8.GetBytes("test,csv,data");

        _mockJobService.Setup(s => s.GetAllJobsAsync(null, null)).ReturnsAsync(jobs);
        _mockCsvService.Setup(s => s.ExportJobs(jobs)).Returns(csvData);

        // Act
        var result = await _controller.ExportJobs();

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/csv");
        fileResult.FileContents.Should().Equal(csvData);
        fileResult.FileDownloadName.Should().Contain("jobs_export_");
    }

    [Fact]
    public async Task ExportJobs_WithStatusFilter_ReturnsFilteredJobsAsCsv()
    {
        // Arrange
        var status = "Completed";
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda", Description = "Test", Status = status, DateReceived = DateTime.UtcNow, TotalCost = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        var csvData = Encoding.UTF8.GetBytes("test,csv,data");

        _mockJobService.Setup(s => s.GetAllJobsAsync(null, status)).ReturnsAsync(jobs);
        _mockCsvService.Setup(s => s.ExportJobs(jobs)).Returns(csvData);

        // Act
        var result = await _controller.ExportJobs(status);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/csv");
        fileResult.FileContents.Should().Equal(csvData);
        fileResult.FileDownloadName.Should().Contain($"jobs_{status}_export_");
    }

    [Fact]
    public async Task ExportJobs_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockJobService.Setup(s => s.GetAllJobsAsync(null, null)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ExportJobs();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
