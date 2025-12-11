using FluentAssertions;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;
using GscTracking.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GscTracking.Api.Tests.Services;

public class CustomerServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _customerService = new CustomerService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllCustomers_WhenNoSearchTerm()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "0987654321", Address = "456 Oak Ave", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "John Doe");
        result.Should().Contain(c => c.Name == "Jane Smith");
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsFilteredCustomers_WhenSearchTermProvided()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "0987654321", Address = "456 Oak Ave", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Phone = "5555555555", Address = "789 Pine Rd", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync("john");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "John Doe");
        result.Should().Contain(c => c.Name == "Bob Johnson");
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsEmpty_WhenNoCustomersMatch()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync("nonexistent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St",
            Notes = "Test notes",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Customer.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetCustomerByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
        result.Phone.Should().Be("1234567890");
        result.Address.Should().Be("123 Main St");
        result.Notes.Should().Be("Test notes");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        // Act
        var result = await _customerService.GetCustomerByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCustomerAsync_CreatesCustomer_WithValidData()
    {
        // Arrange
        var customerRequest = new CustomerRequestDto
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "1112223333",
            Address = "999 New St",
            Notes = "New customer notes"
        };

        // Act
        var result = await _customerService.CreateCustomerAsync(customerRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Customer");
        result.Email.Should().Be("new@example.com");
        result.Phone.Should().Be("1112223333");
        result.Address.Should().Be("999 New St");
        result.Notes.Should().Be("New customer notes");

        var customerInDb = await _context.Customer.FindAsync(result.Id);
        customerInDb.Should().NotBeNull();
        customerInDb!.Name.Should().Be("New Customer");
    }

    [Fact]
    public async Task UpdateCustomerAsync_UpdatesCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "Old Name",
            Email = "old@example.com",
            Phone = "1234567890",
            Address = "Old Address",
            Notes = "Old notes",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Customer.Add(customer);
        await _context.SaveChangesAsync();

        var updateRequest = new CustomerRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9999999999",
            Address = "Updated Address",
            Notes = "Updated notes"
        };

        // Act
        var result = await _customerService.UpdateCustomerAsync(1, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@example.com");
        result.Phone.Should().Be("9999999999");
        result.Address.Should().Be("Updated Address");
        result.Notes.Should().Be("Updated notes");

        var customerInDb = await _context.Customer.FindAsync(1);
        customerInDb!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateCustomerAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var updateRequest = new CustomerRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9999999999",
            Address = "Updated Address",
            Notes = "Updated notes"
        };

        // Act
        var result = await _customerService.UpdateCustomerAsync(999, updateRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCustomerAsync_SoftDeletesCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _context.Customer.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.DeleteCustomerAsync(1);

        // Assert
        result.Should().BeTrue();

        var customerInDb = await _context.Customer.FindAsync(1);
        customerInDb.Should().NotBeNull();
        customerInDb!.IsDeleted.Should().BeTrue();
        customerInDb.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
    {
        // Act
        var result = await _customerService.DeleteCustomerAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllCustomersAsync_SearchesByEmail()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@test.com", Phone = "0987654321", Address = "456 Oak Ave", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync("example.com");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(c => c.Email == "john@example.com");
    }

    [Fact]
    public async Task GetAllCustomersAsync_SearchesByPhone()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@test.com", Phone = "0987654321", Address = "456 Oak Ave", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync("123456");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(c => c.Phone == "1234567890");
    }

    [Fact]
    public async Task GetAllCustomersAsync_SearchesByAddress()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@test.com", Phone = "0987654321", Address = "456 Oak Ave", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _context.Customer.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllCustomersAsync("oak ave");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(c => c.Address == "456 Oak Ave");
    }
}
