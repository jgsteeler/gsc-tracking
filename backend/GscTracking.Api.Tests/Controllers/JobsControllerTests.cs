using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Application.DTOs;
using GscTracking.Application.Jobs.Commands;
using GscTracking.Application.Jobs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class JobsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<JobsController>> _mockLogger;
    private readonly JobsController _controller;

    public JobsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<JobsController>>();
        _controller = new JobsController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetJobs_ReturnsOkResult_WithJobList()
    {
        // Arrange
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = "Quote", DateReceived = DateTime.UtcNow },
            new JobDto { Id = 2, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = "InProgress", DateReceived = DateTime.UtcNow }
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllJobsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetJobs();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedJobs = okResult.Value.Should().BeAssignableTo<IEnumerable<JobDto>>().Subject;
        returnedJobs.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetJobs_ReturnsOkResult_WithSearchAndStatusFilter()
    {
        // Arrange
        var searchTerm = "lawn";
        var statusFilter = "Quote";
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = 1, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = "Quote", DateReceived = DateTime.UtcNow }
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllJobsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetJobs(searchTerm, statusFilter);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedJobs = okResult.Value.Should().BeAssignableTo<IEnumerable<JobDto>>().Subject;
        returnedJobs.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetJobs_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllJobsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobs();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetJob_ReturnsOkResult_WhenJobExists()
    {
        // Arrange
        var jobId = 1;
        var job = new JobDto
        {
            Id = jobId,
            CustomerId = 1,
            CustomerName = "John Doe",
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(job);

        // Act
        var result = await _controller.GetJob(jobId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedJob = okResult.Value.Should().BeOfType<JobDto>().Subject;
        returnedJob.Id.Should().Be(jobId);
        returnedJob.EquipmentType.Should().Be("Lawn Mower");
    }

    [Fact]
    public async Task GetJob_ReturnsNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = 999;
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((JobDto?)null);

        // Act
        var result = await _controller.GetJob(jobId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetJob_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJob(jobId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetJobsByCustomer_ReturnsOkResult_WithCustomerJobs()
    {
        // Arrange
        var customerId = 1;
        var jobs = new List<JobDto>
        {
            new JobDto { Id = 1, CustomerId = customerId, CustomerName = "John Doe", EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = "Quote", DateReceived = DateTime.UtcNow },
            new JobDto { Id = 2, CustomerId = customerId, CustomerName = "John Doe", EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = "InProgress", DateReceived = DateTime.UtcNow }
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobsByCustomerIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetJobsByCustomer(customerId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedJobs = okResult.Value.Should().BeAssignableTo<IEnumerable<JobDto>>().Subject;
        returnedJobs.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetJobsByCustomer_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var customerId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<GetJobsByCustomerIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobsByCustomer(customerId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateJob_ReturnsCreatedAtAction_WithValidData()
    {
        // Arrange
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 150.00m
        };
        var createdJob = new JobDto
        {
            Id = 1,
            CustomerId = jobRequest.CustomerId,
            CustomerName = "John Doe",
            EquipmentType = jobRequest.EquipmentType,
            EquipmentModel = jobRequest.EquipmentModel,
            Description = jobRequest.Description,
            Status = jobRequest.Status,
            DateReceived = jobRequest.DateReceived,
            EstimateAmount = jobRequest.EstimateAmount
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateJobCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdJob);

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetJob));
        var returnedJob = createdAtActionResult.Value.Should().BeOfType<JobDto>().Subject;
        returnedJob.Id.Should().Be(1);
        returnedJob.EquipmentType.Should().Be("Lawn Mower");
    }

    [Fact]
    public async Task CreateJob_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var jobRequest = new JobRequestDto();
        _controller.ModelState.AddModelError("EquipmentType", "Equipment type is required");

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateJob_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "InvalidStatus",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateJobCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid status"));

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateJob_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateJobCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateJob(jobRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateJob_ReturnsOkResult_WhenJobExists()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change and blade sharpening",
            Status = "InProgress",
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 200.00m
        };
        var updatedJob = new JobDto
        {
            Id = jobId,
            CustomerId = jobRequest.CustomerId,
            CustomerName = "John Doe",
            EquipmentType = jobRequest.EquipmentType,
            EquipmentModel = jobRequest.EquipmentModel,
            Description = jobRequest.Description,
            Status = jobRequest.Status,
            DateReceived = jobRequest.DateReceived,
            EstimateAmount = jobRequest.EstimateAmount
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedJob);

        // Act
        var result = await _controller.UpdateJob(jobId, jobRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedJob = okResult.Value.Should().BeOfType<JobDto>().Subject;
        returnedJob.Description.Should().Be("Oil change and blade sharpening");
    }

    [Fact]
    public async Task UpdateJob_ReturnsNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = 999;
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync((JobDto?)null);

        // Act
        var result = await _controller.UpdateJob(jobId, jobRequest);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateJob_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new JobRequestDto();
        _controller.ModelState.AddModelError("EquipmentType", "Equipment type is required");

        // Act
        var result = await _controller.UpdateJob(jobId, jobRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateJob_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "InvalidStatus",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid status"));

        // Act
        var result = await _controller.UpdateJob(jobId, jobRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateJob_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateJob(jobId, jobRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task DeleteJob_ReturnsNoContent_WhenJobExists()
    {
        // Arrange
        var jobId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteJobCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteJob(jobId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteJob_ReturnsNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = 999;
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteJobCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteJob(jobId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteJob_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var jobId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteJobCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteJob(jobId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
