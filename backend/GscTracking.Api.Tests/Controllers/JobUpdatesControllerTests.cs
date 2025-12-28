using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Application.DTOs;
using GscTracking.Application.JobUpdates.Commands;
using GscTracking.Application.JobUpdates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class JobUpdatesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<JobUpdatesController>> _mockLogger;
    private readonly JobUpdatesController _controller;

    public JobUpdatesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<JobUpdatesController>>();
        _controller = new JobUpdatesController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetJobUpdates_ReturnsOkResult_WithUpdateList()
    {
        // Arrange
        var jobId = 1;
        var updates = new List<JobUpdateDto>
        {
            new JobUpdateDto { Id = 1, JobId = 1, UpdateText = "Started diagnostics", CreatedAt = DateTime.UtcNow },
            new JobUpdateDto { Id = 2, JobId = 1, UpdateText = "Found oil leak", CreatedAt = DateTime.UtcNow }
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdatesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(updates);

        // Act
        var result = await _controller.GetJobUpdates(jobId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUpdates = okResult.Value.Should().BeAssignableTo<IEnumerable<JobUpdateDto>>().Subject;
        returnedUpdates.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetJobUpdates_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdatesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobUpdates(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetJobUpdate_ReturnsOkResult_WhenUpdateExists()
    {
        // Arrange
        var jobId = 1;
        var updateId = 1;
        var update = new JobUpdateDto { Id = 1, JobId = 1, UpdateText = "Test update", CreatedAt = DateTime.UtcNow };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(update);

        // Act
        var result = await _controller.GetJobUpdate(jobId, updateId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUpdate = okResult.Value.Should().BeOfType<JobUpdateDto>().Subject;
        returnedUpdate.UpdateText.Should().Be("Test update");
    }

    [Fact]
    public async Task GetJobUpdate_ReturnsNotFound_WhenUpdateDoesNotExist()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((JobUpdateDto?)null);

        // Act
        var result = await _controller.GetJobUpdate(1, 999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetJobUpdate_ReturnsBadRequest_WhenUpdateBelongsToDifferentJob()
    {
        // Arrange
        var update = new JobUpdateDto { Id = 1, JobId = 2, UpdateText = "Test update", CreatedAt = DateTime.UtcNow };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(update);

        // Act
        var result = await _controller.GetJobUpdate(1, 1);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateJobUpdate_ReturnsCreatedAtAction_WithValidData()
    {
        // Arrange
        var jobId = 1;
        var request = new JobUpdateRequestDto { UpdateText = "Test update" };
        var createdUpdate = new JobUpdateDto { Id = 1, JobId = 1, UpdateText = "Test update", CreatedAt = DateTime.UtcNow };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateJobUpdateCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUpdate);

        // Act
        var result = await _controller.CreateJobUpdate(jobId, request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedUpdate = createdResult.Value.Should().BeOfType<JobUpdateDto>().Subject;
        returnedUpdate.UpdateText.Should().Be("Test update");
    }

    [Fact]
    public async Task CreateJobUpdate_ReturnsBadRequest_WhenJobDoesNotExist()
    {
        // Arrange
        var request = new JobUpdateRequestDto { UpdateText = "Test update" };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateJobUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Job with ID 999 not found."));

        // Act
        var result = await _controller.CreateJobUpdate(999, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteJobUpdate_ReturnsNoContent_WhenUpdateExists()
    {
        // Arrange
        var jobId = 1;
        var updateId = 1;
        var update = new JobUpdateDto { Id = 1, JobId = 1, UpdateText = "Test update", CreatedAt = DateTime.UtcNow };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(update);
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteJobUpdateCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteJobUpdate(jobId, updateId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteJobUpdate_ReturnsNotFound_WhenUpdateDoesNotExist()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((JobUpdateDto?)null);

        // Act
        var result = await _controller.DeleteJobUpdate(1, 999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteJobUpdate_ReturnsBadRequest_WhenUpdateBelongsToDifferentJob()
    {
        // Arrange
        var update = new JobUpdateDto { Id = 1, JobId = 2, UpdateText = "Test update", CreatedAt = DateTime.UtcNow };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobUpdateByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(update);

        // Act
        var result = await _controller.DeleteJobUpdate(1, 1);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
