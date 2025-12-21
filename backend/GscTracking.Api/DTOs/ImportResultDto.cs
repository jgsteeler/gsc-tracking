namespace GscTracking.Api.DTOs;

/// <summary>
/// Result of a CSV import operation
/// </summary>
public class ImportResultDto
{
    /// <summary>
    /// Number of successfully imported records
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed records
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// List of errors encountered during import
    /// </summary>
    public List<ImportErrorDto> Errors { get; set; } = new List<ImportErrorDto>();
}

/// <summary>
/// Details of an import error
/// </summary>
public class ImportErrorDto
{
    /// <summary>
    /// Line number in the CSV file where the error occurred
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Description of the error
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The raw data that caused the error
    /// </summary>
    public string? RawData { get; set; }
}
