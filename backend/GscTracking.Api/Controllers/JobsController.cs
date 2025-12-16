using Microsoft.AspNetCore.Mvc;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobService jobService, ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Get all jobs with optional search and status filter
    /// </summary>
    /// <param name="search">Optional search term to filter jobs</param>
    /// <param name="status">Optional status filter</param>
    /// <returns>List of jobs</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetJobs([FromQuery] string? search = null, [FromQuery] string? status = null)
    {
        try
        {
            var jobs = await _jobService.GetAllJobsAsync(search, status);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs");
            return StatusCode(500, new { message = "An error occurred while retrieving jobs." });
        }
    }

    /// <summary>
    /// Get a specific job by ID
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>Job details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { message = $"Job with ID {id} not found." });
            }
            return Ok(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the job." });
        }
    }

    /// <summary>
    /// Get all jobs for a specific customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of jobs for the customer</returns>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetJobsByCustomer(int customerId)
    {
        try
        {
            var jobs = await _jobService.GetJobsByCustomerIdAsync(customerId);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs for customer {CustomerId}", customerId);
            return StatusCode(500, new { message = "An error occurred while retrieving customer jobs." });
        }
    }

    /// <summary>
    /// Create a new job
    /// </summary>
    /// <param name="jobRequest">Job data</param>
    /// <returns>Created job</returns>
    /// <remarks>
    /// Validation Rules:
    /// - CustomerId: Required, must be greater than 0
    /// - EquipmentType: Required, max 200 characters
    /// - EquipmentModel: Required, max 200 characters
    /// - Description: Required, max 2000 characters
    /// - Status: Required, must be one of: Quote, InProgress, Completed, Invoiced, Paid
    /// - DateReceived: Required
    /// - DateCompleted: Optional, must be on or after DateReceived
    /// - EstimateAmount: Optional, must be >= 0
    /// - ActualAmount: Optional, must be >= 0
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="201">Job created successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] JobRequestDto jobRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var job = await _jobService.CreateJobAsync(jobRequest);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when creating job");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job");
            return StatusCode(500, new { message = "An error occurred while creating the job." });
        }
    }

    /// <summary>
    /// Update an existing job
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <param name="jobRequest">Updated job data</param>
    /// <returns>Updated job</returns>
    /// <remarks>
    /// Validation Rules:
    /// - CustomerId: Required, must be greater than 0
    /// - EquipmentType: Required, max 200 characters
    /// - EquipmentModel: Required, max 200 characters
    /// - Description: Required, max 2000 characters
    /// - Status: Required, must be one of: Quote, InProgress, Completed, Invoiced, Paid
    /// - DateReceived: Required
    /// - DateCompleted: Optional, must be on or after DateReceived
    /// - EstimateAmount: Optional, must be >= 0
    /// - ActualAmount: Optional, must be >= 0
    /// 
    /// Returns 400 Bad Request if validation fails with detailed error messages.
    /// </remarks>
    /// <response code="200">Job updated successfully</response>
    /// <response code="400">Validation error - check response for details</response>
    /// <response code="404">Job not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JobDto>> UpdateJob(int id, [FromBody] JobRequestDto jobRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var job = await _jobService.UpdateJobAsync(id, jobRequest);
            if (job == null)
            {
                return NotFound(new { message = $"Job with ID {id} not found." });
            }
            return Ok(job);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating job {JobId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the job." });
        }
    }

    /// <summary>
    /// Delete a job (soft delete)
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        try
        {
            var result = await _jobService.DeleteJobAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Job with ID {id} not found." });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job {JobId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the job." });
        }
    }
}
