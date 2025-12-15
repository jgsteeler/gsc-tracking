using FluentAssertions;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;
using GscTracking.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GscTracking.Api.Tests.Services;

public class JobUpdateServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly JobUpdateService _jobUpdateService;
    private readonly Customer _testCustomer;
    private readonly Job _testJob;

    public JobUpdateServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _jobUpdateService = new JobUpdateService(_context);

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

        // Create a test job
        _testJob = new Job
        {
            Id = 1,
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Oil change",
            Status = JobStatus.InProgress,
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
    public async Task GetJobUpdatesAsync_ReturnsAllUpdates_ForGivenJob()
    {
        // Arrange
        var updates = new List<JobUpdate>
        {
            new JobUpdate { Id = 1, JobId = 1, UpdateText = "Started diagnostics", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
            new JobUpdate { Id = 2, JobId = 1, UpdateText = "Found oil leak", CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
            new JobUpdate { Id = 3, JobId = 1, UpdateText = "Ordered parts", CreatedAt = DateTime.UtcNow }
        };
        _context.JobUpdate.AddRange(updates);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobUpdateService.GetJobUpdatesAsync(1);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(u => u.UpdateText == "Started diagnostics");
        result.Should().Contain(u => u.UpdateText == "Found oil leak");
        result.Should().Contain(u => u.UpdateText == "Ordered parts");
    }

    [Fact]
    public async Task GetJobUpdatesAsync_ReturnsEmpty_WhenNoUpdates()
    {
        // Act
        var result = await _jobUpdateService.GetJobUpdatesAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetJobUpdateByIdAsync_ReturnsUpdate_WhenExists()
    {
        // Arrange
        var update = new JobUpdate
        {
            Id = 1,
            JobId = 1,
            UpdateText = "Test update",
            CreatedAt = DateTime.UtcNow
        };
        _context.JobUpdate.Add(update);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobUpdateService.GetJobUpdateByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.UpdateText.Should().Be("Test update");
        result.JobId.Should().Be(1);
    }

    [Fact]
    public async Task GetJobUpdateByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _jobUpdateService.GetJobUpdateByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateJobUpdateAsync_CreatesUpdate_WithValidData()
    {
        // Arrange
        var request = new JobUpdateRequestDto
        {
            UpdateText = "I finished the diagnostics on Jim's snow blower, the leak is from the crankcase cover"
        };

        // Act
        var result = await _jobUpdateService.CreateJobUpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.JobId.Should().Be(1);
        result.UpdateText.Should().Be("I finished the diagnostics on Jim's snow blower, the leak is from the crankcase cover");
        result.Id.Should().BeGreaterThan(0);

        // Verify in database
        var updateInDb = await _context.JobUpdate.FindAsync(result.Id);
        updateInDb.Should().NotBeNull();
        updateInDb!.UpdateText.Should().Be(request.UpdateText);
    }

    [Fact]
    public async Task CreateJobUpdateAsync_ThrowsException_WhenJobDoesNotExist()
    {
        // Arrange
        var request = new JobUpdateRequestDto
        {
            UpdateText = "Test update"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _jobUpdateService.CreateJobUpdateAsync(999, request));
    }

    [Fact]
    public async Task DeleteJobUpdateAsync_ReturnsTrue_WhenUpdateExists()
    {
        // Arrange
        var update = new JobUpdate
        {
            Id = 1,
            JobId = 1,
            UpdateText = "Test update",
            CreatedAt = DateTime.UtcNow
        };
        _context.JobUpdate.Add(update);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobUpdateService.DeleteJobUpdateAsync(1);

        // Assert
        result.Should().BeTrue();
        var updateInDb = await _context.JobUpdate.FindAsync(1);
        updateInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteJobUpdateAsync_ReturnsFalse_WhenUpdateDoesNotExist()
    {
        // Act
        var result = await _jobUpdateService.DeleteJobUpdateAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetJobUpdatesAsync_ReturnsUpdatesInDescendingOrder_ByCreatedAt()
    {
        // Arrange
        var updates = new List<JobUpdate>
        {
            new JobUpdate { Id = 1, JobId = 1, UpdateText = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-20) },
            new JobUpdate { Id = 2, JobId = 1, UpdateText = "Second", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
            new JobUpdate { Id = 3, JobId = 1, UpdateText = "Third", CreatedAt = DateTime.UtcNow }
        };
        _context.JobUpdate.AddRange(updates);
        await _context.SaveChangesAsync();

        // Act
        var result = await _jobUpdateService.GetJobUpdatesAsync(1);

        // Assert
        var resultList = result.ToList();
        resultList[0].UpdateText.Should().Be("Third");
        resultList[1].UpdateText.Should().Be("Second");
        resultList[2].UpdateText.Should().Be("First");
    }
}
