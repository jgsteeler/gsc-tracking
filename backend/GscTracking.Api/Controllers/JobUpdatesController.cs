using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using GscTracking.Application.DTOs;
using GscTracking.Application.JobUpdates.Commands;
using GscTracking.Application.JobUpdates.Queries;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId}/updates")]
[Authorize(Policy = "ReadAccess")] // Require read access for all endpoints
public class JobUpdatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobUpdatesController> _logger;

    public JobUpdatesController(IMediator mediator, ILogger<JobUpdatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all updates for a specific job
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <returns>List of job updates</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobUpdateDto>>> GetJobUpdates(int jobId)
    {
        try
        {
            var query = new GetJobUpdatesQuery(jobId);
            var updates = await _mediator.Send(query);
            return Ok(updates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving updates for job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while retrieving job updates." });
        }
    }

    /// <summary>
    /// Get a specific job update by ID
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="id">Update ID</param>
    /// <returns>Job update details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobUpdateDto>> GetJobUpdate(int jobId, int id)
    {
        try
        {
            var query = new GetJobUpdateByIdQuery(id);
            var update = await _mediator.Send(query);
            if (update == null)
            {
                return NotFound(new { message = $"Job update with ID {id} not found." });
            }
            if (update.JobId != jobId)
            {
                return BadRequest(new { message = "Job update does not belong to the specified job." });
            }
            return Ok(update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job update {UpdateId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the job update." });
        }
    }

    /// <summary>
    /// Create a new job update
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="updateRequest">Update data</param>
    /// <returns>Created job update</returns>
    [HttpPost]
    [Authorize(Policy = "WriteAccess")]
    public async Task<ActionResult<JobUpdateDto>> CreateJobUpdate(int jobId, [FromBody] JobUpdateRequestDto updateRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var command = new CreateJobUpdateCommand(jobId, updateRequest);
            var update = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetJobUpdate), new { jobId = update.JobId, id = update.Id }, update);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when creating job update for job {JobId}", jobId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job update for job {JobId}", jobId);
            return StatusCode(500, new { message = "An error occurred while creating the job update." });
        }
    }

    /// <summary>
    /// Delete a job update
    /// </summary>
    /// <param name="jobId">Job ID</param>
    /// <param name="id">Update ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteJobUpdate(int jobId, int id)
    {
        try
        {
            // Verify the update exists and belongs to the specified job in one operation
            var query = new GetJobUpdateByIdQuery(id);
            var update = await _mediator.Send(query);
            if (update == null)
            {
                return NotFound(new { message = $"Job update with ID {id} not found." });
            }
            if (update.JobId != jobId)
            {
                return BadRequest(new { message = "Job update does not belong to the specified job." });
            }

            // Attempt to delete - if it fails (already deleted), we'll return 404
            var command = new DeleteJobUpdateCommand(id);
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound(new { message = $"Job update with ID {id} not found." });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job update {UpdateId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the job update." });
        }
    }
}
