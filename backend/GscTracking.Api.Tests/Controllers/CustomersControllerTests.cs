using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _mockCustomerService;
    private readonly Mock<ILogger<CustomersController>> _mockLogger;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockCustomerService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_mockCustomerService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetCustomers_ReturnsOkResult_WithCustomerList()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St" },
            new CustomerDto { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "0987654321", Address = "456 Oak Ave" }
        };
        _mockCustomerService.Setup(s => s.GetAllCustomersAsync(null)).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCustomers_ReturnsOkResult_WithSearchFilter()
    {
        // Arrange
        var searchTerm = "john";
        var customers = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Main St" }
        };
        _mockCustomerService.Setup(s => s.GetAllCustomersAsync(searchTerm)).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetCustomers(searchTerm);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCustomers_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockCustomerService.Setup(s => s.GetAllCustomersAsync(null))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetCustomer_ReturnsOkResult_WhenCustomerExists()
    {
        // Arrange
        var customerId = 1;
        var customer = new CustomerDto
        {
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };
        _mockCustomerService.Setup(s => s.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomer(customerId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.Id.Should().Be(customerId);
        returnedCustomer.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = 999;
        _mockCustomerService.Setup(s => s.GetCustomerByIdAsync(customerId)).ReturnsAsync((CustomerDto?)null);

        // Act
        var result = await _controller.GetCustomer(customerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCustomer_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var customerId = 1;
        _mockCustomerService.Setup(s => s.GetCustomerByIdAsync(customerId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetCustomer(customerId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedAtAction_WithValidData()
    {
        // Arrange
        var customerRequest = new CustomerRequestDto
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "1112223333",
            Address = "999 New St"
        };
        var createdCustomer = new CustomerDto
        {
            Id = 1,
            Name = customerRequest.Name,
            Email = customerRequest.Email,
            Phone = customerRequest.Phone,
            Address = customerRequest.Address
        };
        _mockCustomerService.Setup(s => s.CreateCustomerAsync(customerRequest)).ReturnsAsync(createdCustomer);

        // Act
        var result = await _controller.CreateCustomer(customerRequest);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetCustomer));
        var returnedCustomer = createdAtActionResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.Id.Should().Be(1);
        returnedCustomer.Name.Should().Be("New Customer");
    }

    [Fact]
    public async Task CreateCustomer_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var customerRequest = new CustomerRequestDto();
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.CreateCustomer(customerRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateCustomer_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var customerRequest = new CustomerRequestDto
        {
            Name = "New Customer",
            Email = "new@example.com",
            Phone = "1112223333",
            Address = "999 New St"
        };
        _mockCustomerService.Setup(s => s.CreateCustomerAsync(customerRequest))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateCustomer(customerRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsOkResult_WhenCustomerExists()
    {
        // Arrange
        var customerId = 1;
        var customerRequest = new CustomerRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9999999999",
            Address = "Updated Address"
        };
        var updatedCustomer = new CustomerDto
        {
            Id = customerId,
            Name = customerRequest.Name,
            Email = customerRequest.Email,
            Phone = customerRequest.Phone,
            Address = customerRequest.Address
        };
        _mockCustomerService.Setup(s => s.UpdateCustomerAsync(customerId, customerRequest)).ReturnsAsync(updatedCustomer);

        // Act
        var result = await _controller.UpdateCustomer(customerId, customerRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = 999;
        var customerRequest = new CustomerRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9999999999",
            Address = "Updated Address"
        };
        _mockCustomerService.Setup(s => s.UpdateCustomerAsync(customerId, customerRequest)).ReturnsAsync((CustomerDto?)null);

        // Act
        var result = await _controller.UpdateCustomer(customerId, customerRequest);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var customerId = 1;
        var customerRequest = new CustomerRequestDto();
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.UpdateCustomer(customerId, customerRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var customerId = 1;
        var customerRequest = new CustomerRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9999999999",
            Address = "Updated Address"
        };
        _mockCustomerService.Setup(s => s.UpdateCustomerAsync(customerId, customerRequest))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateCustomer(customerId, customerRequest);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerExists()
    {
        // Arrange
        var customerId = 1;
        _mockCustomerService.Setup(s => s.DeleteCustomerAsync(customerId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCustomer(customerId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = 999;
        _mockCustomerService.Setup(s => s.DeleteCustomerAsync(customerId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCustomer(customerId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var customerId = 1;
        _mockCustomerService.Setup(s => s.DeleteCustomerAsync(customerId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteCustomer(customerId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
