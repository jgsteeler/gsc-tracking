using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GscTracking.Api.Services;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly ICsvService _csvService;
    private readonly IExpenseService _expenseService;
    private readonly IJobService _jobService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        ICsvService csvService,
        IExpenseService expenseService,
        IJobService jobService,
        ILogger<ExportController> logger)
    {
        _csvService = csvService;
        _expenseService = expenseService;
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Export all expenses to CSV format
    /// </summary>
    /// <param name="jobId">Optional: Filter expenses by job ID</param>
    /// <returns>CSV file with expenses data</returns>
    [HttpGet("expenses")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportExpenses([FromQuery] int? jobId = null)
    {
        try
        {
            IEnumerable<DTOs.ExpenseDto> expenses;

            if (jobId.HasValue)
            {
                expenses = await _expenseService.GetExpensesByJobIdAsync(jobId.Value);
            }
            else
            {
                // Get all expenses across all jobs
                var jobs = await _jobService.GetAllJobsAsync();
                var allExpenses = new List<DTOs.ExpenseDto>();
                
                foreach (var job in jobs)
                {
                    var jobExpenses = await _expenseService.GetExpensesByJobIdAsync(job.Id);
                    allExpenses.AddRange(jobExpenses);
                }
                
                expenses = allExpenses;
            }

            var csvData = _csvService.ExportExpenses(expenses);
            var fileName = jobId.HasValue 
                ? $"expenses_job_{jobId}_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv"
                : $"expenses_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting expenses");
            return StatusCode(500, new { message = "An error occurred while exporting expenses." });
        }
    }

    /// <summary>
    /// Export all jobs with estimate/invoice data to CSV format
    /// </summary>
    /// <param name="status">Optional: Filter jobs by status</param>
    /// <returns>CSV file with jobs data</returns>
    [HttpGet("jobs")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportJobs([FromQuery] string? status = null)
    {
        try
        {
            var jobs = await _jobService.GetAllJobsAsync(null, status);
            var csvData = _csvService.ExportJobs(jobs);
            
            var fileName = status != null
                ? $"jobs_{status}_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv"
                : $"jobs_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting jobs");
            return StatusCode(500, new { message = "An error occurred while exporting jobs." });
        }
    }
}
