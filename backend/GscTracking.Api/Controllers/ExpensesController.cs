using Microsoft.AspNetCore.Mvc;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId}/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    /// <summary>
    /// Get all expenses for a specific job
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <returns>List of expenses</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses(int jobId)
    {
        try
        {
            var expenses = await _expenseService.GetExpensesByJobIdAsync(jobId);
            return Ok(expenses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expenses for job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while retrieving expenses." });
        }
    }

    /// <summary>
    /// Create a new expense for a job
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="expenseRequest">Expense data</param>
    /// <returns>Created expense</returns>
    /// <remarks>
    /// Validation Rules:
    /// - Type: Required, must be one of: Parts, Labor, Service
    /// - Description: Required, max 500 characters
    /// - Amount: Required, must be greater than 0
    /// - Date: Required
    /// - ReceiptReference: Optional, max 200 characters
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="201">Expense created successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(int jobId, [FromBody] ExpenseRequestDto expenseRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var expense = await _expenseService.CreateExpenseAsync(jobId, expenseRequest);
            return CreatedAtAction(nameof(GetExpenseById), new { jobId, id = expense.Id }, expense);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when creating expense for job {JobId}", jobId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense for job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while creating the expense." });
        }
    }

    /// <summary>
    /// Get a specific expense by ID
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="id">Expense ID</param>
    /// <returns>Expense details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById(int jobId, int id)
    {
        try
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} not found." });
            }
            // Verify the expense belongs to the specified job
            if (expense.JobId != jobId)
            {
                return NotFound(new { message = $"Expense with ID {id} not found for job {jobId}." });
            }
            return Ok(expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expense {ExpenseId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the expense." });
        }
    }

    /// <summary>
    /// Update an existing expense
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="id">Expense ID</param>
    /// <param name="expenseRequest">Updated expense data</param>
    /// <returns>Updated expense</returns>
    /// <remarks>
    /// Validation Rules:
    /// - Type: Required, must be one of: Parts, Labor, Service
    /// - Description: Required, max 500 characters
    /// - Amount: Required, must be greater than 0
    /// - Date: Required
    /// - ReceiptReference: Optional, max 200 characters
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="200">Expense updated successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="404">Expense not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(int jobId, int id, [FromBody] ExpenseRequestDto expenseRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Verify the expense exists and belongs to the specified job
            var existingExpense = await _expenseService.GetExpenseByIdAsync(id);
            if (existingExpense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} not found." });
            }
            if (existingExpense.JobId != jobId)
            {
                return BadRequest(new { message = "Expense does not belong to the specified job." });
            }
            
            var expense = await _expenseService.UpdateExpenseAsync(id, expenseRequest);
            if (expense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} not found." });
            }
            return Ok(expense);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating expense {ExpenseId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the expense." });
        }
    }

    /// <summary>
    /// Delete an expense
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="id">Expense ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int jobId, int id)
    {
        try
        {
            // Verify the expense exists and belongs to the specified job
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} not found." });
            }
            if (expense.JobId != jobId)
            {
                return BadRequest(new { message = "Expense does not belong to the specified job." });
            }

            // Attempt to delete - if it fails (already deleted), we'll return 404
            var result = await _expenseService.DeleteExpenseAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Expense with ID {id} not found." });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation when deleting expense {ExpenseId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the expense." });
        }
    }
}
