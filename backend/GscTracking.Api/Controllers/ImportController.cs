using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GscTracking.Api.Services;
using GscTracking.Api.DTOs;

namespace GscTracking.Api.Controllers;

[ApiController]
[Route("api/import")]
[Authorize(Policy = "AdminOnly")]
public class ImportController : ControllerBase
{
    private readonly ICsvService _csvService;
    private readonly ILogger<ImportController> _logger;
    private readonly IConfiguration _configuration;
    private readonly long _maxFileSize;

    public ImportController(ICsvService csvService, ILogger<ImportController> logger, IConfiguration configuration)
    {
        _csvService = csvService;
        _logger = logger;
        _configuration = configuration;
        // Default to 10 MB if not configured
        _maxFileSize = _configuration.GetValue<long>("CsvImport:MaxFileSizeBytes", 10 * 1024 * 1024);
    }

    /// <summary>
    /// Import expenses from a CSV file
    /// </summary>
    /// <param name="file">CSV file containing expense data</param>
    /// <returns>Import result with success/error counts and details</returns>
    /// <remarks>
    /// The CSV file should have the following columns:
    /// - Job ID (required)
    /// - Type (required, one of: Parts, Labor, Service)
    /// - Description (required)
    /// - Amount (required, must be greater than 0)
    /// - Date (required, format: yyyy-MM-dd or MM/dd/yyyy)
    /// - Receipt Reference (optional)
    /// 
    /// Example:
    /// Job ID,Type,Description,Amount,Date,Receipt Reference
    /// 1,Parts,Oil filter,15.99,2025-01-15,REC-001
    /// 1,Labor,Oil change service,45.00,2025-01-15,
    /// </remarks>
    /// <response code="200">Import completed with results</response>
    /// <response code="400">Bad request - invalid file or format</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("expenses")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportResultDto>> ImportExpenses(IFormFile file)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded or file is empty." });
            }

            // Check file size
            if (file.Length > _maxFileSize)
            {
                return BadRequest(new { message = $"File size exceeds the maximum allowed size of {_maxFileSize / 1024 / 1024} MB." });
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                return BadRequest(new { message = "Only CSV files are supported." });
            }

            // Process the import
            using var stream = file.OpenReadStream();
            var result = await _csvService.ImportExpensesAsync(stream);

            _logger.LogInformation(
                "Expense import completed: {SuccessCount} succeeded, {ErrorCount} failed",
                result.SuccessCount,
                result.ErrorCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing expenses");
            return StatusCode(500, new { message = "An error occurred while importing expenses." });
        }
    }
}
