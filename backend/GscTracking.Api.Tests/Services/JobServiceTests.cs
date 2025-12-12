using FluentAssertions;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;
using GscTracking.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GscTracking.Api.Tests.Services;

public class JobServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly JobService _jobService;
    private readonly Customer _testCustomer;

    public JobServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _jobService = new JobService(_context);

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
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsAllJobs_WhenNoFilters()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(j => j.EquipmentType == "Lawn Mower");
        result.Should().Contain(j => j.EquipmentType == "Chainsaw");
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsFilteredJobs_WhenSearchTermProvided()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync("lawn");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(j => j.EquipmentType == "Lawn Mower");
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsFilteredJobs_WhenStatusFilterProvided()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync(null, "InProgress");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(j => j.Status == "InProgress");
    }

    [Fact]
    public async Task GetJobByIdAsync_ReturnsJob_WhenJobExists()
    {
        // Arrange
        var job = new Job
        {
            Id = 1,
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change and blade sharpening",
            Status = JobStatus.Quote,
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 150.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetJobByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.EquipmentType.Should().Be("Lawn Mower");
        result.EquipmentModel.Should().Be("Honda HRX217");
        result.Description.Should().Be("Oil change and blade sharpening");
        result.Status.Should().Be("Quote");
        result.EstimateAmount.Should().Be(150.00m);
        result.CustomerName.Should().Be("Test Customer");
    }

    [Fact]
    public async Task GetJobByIdAsync_ReturnsNull_WhenJobDoesNotExist()
    {
        // Act
        var result = await _jobService.GetJobByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetJobsByCustomerIdAsync_ReturnsCustomerJobs()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetJobsByCustomerIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(j => j.CustomerId == 1);
    }

    [Fact]
    public async Task CreateJobAsync_CreatesJob_WithValidData()
    {
        // Arrange
        var jobRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change and blade sharpening",
            Status = "Quote",
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 150.00m
        };

        // Act
        var result = await _jobService.CreateJobAsync(jobRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.EquipmentType.Should().Be("Lawn Mower");
        result.EquipmentModel.Should().Be("Honda HRX217");
        result.Description.Should().Be("Oil change and blade sharpening");
        result.Status.Should().Be("Quote");
        result.EstimateAmount.Should().Be(150.00m);
        result.CustomerName.Should().Be("Test Customer");

        var jobInDb = await _context.Job.FindAsync(result.Id);
        jobInDb.Should().NotBeNull();
        jobInDb!.EquipmentType.Should().Be("Lawn Mower");
    }

    [Fact]
    public async Task CreateJobAsync_ThrowsException_WithInvalidStatus()
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

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _jobService.CreateJobAsync(jobRequest));
    }

    [Fact]
    public async Task UpdateJobAsync_UpdatesJob_WhenJobExists()
    {
        // Arrange
        var job = new Job
        {
            Id = 1,
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = JobStatus.Quote,
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 150.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        var updateRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change and blade sharpening",
            Status = "InProgress",
            DateReceived = DateTime.UtcNow,
            EstimateAmount = 200.00m,
            ActualAmount = 180.00m
        };

        // Act
        var result = await _jobService.UpdateJobAsync(1, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Description.Should().Be("Oil change and blade sharpening");
        result.Status.Should().Be("InProgress");
        result.EstimateAmount.Should().Be(200.00m);
        result.ActualAmount.Should().Be(180.00m);

        var jobInDb = await _context.Job.FindAsync(1);
        jobInDb!.Description.Should().Be("Oil change and blade sharpening");
        jobInDb.Status.Should().Be(JobStatus.InProgress);
    }

    [Fact]
    public async Task UpdateJobAsync_ReturnsNull_WhenJobDoesNotExist()
    {
        // Arrange
        var updateRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };

        // Act
        var result = await _jobService.UpdateJobAsync(999, updateRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateJobAsync_ThrowsException_WithInvalidStatus()
    {
        // Arrange
        var job = new Job
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
        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        var updateRequest = new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = "InvalidStatus",
            DateReceived = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _jobService.UpdateJobAsync(1, updateRequest));
    }

    [Fact]
    public async Task DeleteJobAsync_SoftDeletesJob_WhenJobExists()
    {
        // Arrange
        var job = new Job
        {
            Id = 1,
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = JobStatus.Quote,
            DateReceived = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.DeleteJobAsync(1);

        // Assert
        result.Should().BeTrue();

        var jobInDb = await _context.Job.FindAsync(1);
        jobInDb.Should().NotBeNull();
        jobInDb!.IsDeleted.Should().BeTrue();
        jobInDb.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteJobAsync_ReturnsFalse_WhenJobDoesNotExist()
    {
        // Act
        var result = await _jobService.DeleteJobAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllJobsAsync_SearchesByEquipmentModel()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync("Honda");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(j => j.EquipmentModel == "Honda HRX217");
    }

    [Fact]
    public async Task GetAllJobsAsync_SearchesByDescription()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, CustomerId = 1, EquipmentType = "Lawn Mower", EquipmentModel = "Honda HRX217", Description = "Oil change", Status = JobStatus.Quote, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Job { Id = 2, CustomerId = 1, EquipmentType = "Chainsaw", EquipmentModel = "Stihl MS271", Description = "Chain sharpening", Status = JobStatus.InProgress, DateReceived = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Job.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync("chain");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(j => j.Description.Contains("Chain"));
    }

    [Fact]
    public async Task GetAllJobsAsync_SearchesByCustomerName()
    {
        // Arrange
        var job = new Job
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
        _context.Job.Add(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobService.GetAllJobsAsync("Test Customer");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(j => j.CustomerName == "Test Customer");
    }
}
