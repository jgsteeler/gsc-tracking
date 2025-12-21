using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with optional search functionality
    /// </summary>
    /// <param name="search">Optional search term to filter customers</param>
    /// <returns>List of customers</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([FromQuery] string? search = null)
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync(search);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return StatusCode(500, new { message = "An error occurred while retrieving customers." });
        }
    }
    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found." });
            }
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the customer." });
        }
    }
    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="customerRequest">Customer data</param>
    /// <returns>Created customer</returns>
    /// <remarks>
    /// Validation Rules:
    /// - Name: Required, max 200 characters
    /// - Email: Optional, valid email format, max 200 characters
    /// - Phone: Optional, max 50 characters
    /// - Address: Optional, max 500 characters
    /// - Notes: Optional, max 2000 characters
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="201">Customer created successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerRequestDto customerRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customer = await _customerService.CreateCustomerAsync(customerRequest);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, new { message = "An error occurred while creating the customer." });
        }
    }
    /// <summary>
    /// Update an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="customerRequest">Updated customer data</param>
    /// <returns>Updated customer</returns>
    /// <remarks>
    /// Validation Rules:
    /// - Name: Required, max 200 characters
    /// - Email: Optional, valid email format, max 200 characters
    /// - Phone: Optional, max 50 characters
    /// - Address: Optional, max 500 characters
    /// - Notes: Optional, max 2000 characters
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="200">Customer updated successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="404">Customer not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, [FromBody] CustomerRequestDto customerRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customer = await _customerService.UpdateCustomerAsync(id, customerRequest);
            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found." });
            }
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the customer." });
        }
    }
    /// <summary>
    /// Delete a customer (soft delete)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Customer with ID {id} not found." });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the customer." });
        }
    }
}
