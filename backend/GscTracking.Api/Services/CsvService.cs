using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using GscTracking.Api.DTOs;

namespace GscTracking.Api.Services;

/// <summary>
/// Interface for CSV export and import operations
/// </summary>
public interface ICsvService
{
    /// <summary>
    /// Export expenses to CSV format
    /// </summary>
    byte[] ExportExpenses(IEnumerable<ExpenseDto> expenses);

    /// <summary>
    /// Export jobs to CSV format
    /// </summary>
    byte[] ExportJobs(IEnumerable<JobDto> jobs);

    /// <summary>
    /// Import expenses from CSV format
    /// </summary>
    Task<ImportResultDto> ImportExpensesAsync(Stream csvStream);
}

/// <summary>
/// Service for handling CSV export and import operations
/// </summary>
public class CsvService : ICsvService
{
    private readonly IExpenseService _expenseService;
    private readonly IJobService _jobService;
    private readonly IValidator<ExpenseImportDto> _validator;
    private readonly ILogger<CsvService> _logger;

    public CsvService(
        IExpenseService expenseService, 
        IJobService jobService, 
        IValidator<ExpenseImportDto> validator,
        ILogger<CsvService> logger)
    {
        _expenseService = expenseService;
        _jobService = jobService;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Export expenses to CSV format
    /// </summary>
    public byte[] ExportExpenses(IEnumerable<ExpenseDto> expenses)
    {
        var csvDtos = expenses.Select(e => new ExpenseCsvDto
        {
            Id = e.Id,
            JobId = e.JobId,
            Type = e.Type,
            Description = e.Description,
            Amount = e.Amount,
            Date = e.Date,
            ReceiptReference = e.ReceiptReference,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });

        return WriteCsv(csvDtos);
    }

    /// <summary>
    /// Export jobs to CSV format
    /// </summary>
    public byte[] ExportJobs(IEnumerable<JobDto> jobs)
    {
        var csvDtos = jobs.Select(j => new JobCsvDto
        {
            Id = j.Id,
            CustomerId = j.CustomerId,
            CustomerName = j.CustomerName,
            EquipmentType = j.EquipmentType,
            EquipmentModel = j.EquipmentModel,
            Description = j.Description,
            Status = j.Status,
            DateReceived = j.DateReceived,
            DateCompleted = j.DateCompleted,
            EstimateAmount = j.EstimateAmount,
            ActualAmount = j.ActualAmount,
            TotalCost = j.TotalCost,
            ProfitMargin = j.ProfitMargin,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt
        });

        return WriteCsv(csvDtos);
    }

    /// <summary>
    /// Import expenses from CSV format
    /// </summary>
    public async Task<ImportResultDto> ImportExpensesAsync(Stream csvStream)
    {
        var result = new ImportResultDto();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null // Ignore bad data and handle it ourselves
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        var records = new List<ExpenseImportDto>();
        var lineNumber = 1; // Header is line 1

        try
        {
            // Read all records synchronously for better performance with large files
            csv.Read();
            csv.ReadHeader();
            lineNumber++;

            while (csv.Read())
            {
                var record = csv.GetRecord<ExpenseImportDto>();
                if (record != null)
                {
                    records.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CSV file at line {LineNumber}", lineNumber);
            result.Errors.Add(new ImportErrorDto
            {
                LineNumber = lineNumber,
                Message = $"Error parsing CSV: {ex.Message}"
            });
            return result;
        }

        // Pre-validate all job IDs to avoid N+1 queries
        var jobIds = records.Select(r => r.JobId).Distinct().ToList();
        var existingJobs = await _jobService.GetAllJobsAsync();
        var validJobIds = new HashSet<int>(existingJobs.Select(j => j.Id));

        // Process each record
        lineNumber = 1; // Reset for processing
        foreach (var record in records)
        {
            lineNumber++;
            try
            {
                // Validate the record
                var validationResult = await _validator.ValidateAsync(record);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    result.Errors.Add(new ImportErrorDto
                    {
                        LineNumber = lineNumber,
                        Message = $"Validation failed: {errors}",
                        RawData = $"{record.JobId},{record.Type},{record.Description},{record.Amount},{record.Date}"
                    });
                    result.ErrorCount++;
                    continue;
                }

                // Check if job exists using pre-loaded data
                if (!validJobIds.Contains(record.JobId))
                {
                    result.Errors.Add(new ImportErrorDto
                    {
                        LineNumber = lineNumber,
                        Message = $"Job with ID {record.JobId} not found",
                        RawData = $"{record.JobId},{record.Type},{record.Description},{record.Amount},{record.Date}"
                    });
                    result.ErrorCount++;
                    continue;
                }

                // Create expense request DTO
                var expenseRequest = new ExpenseRequestDto
                {
                    Type = record.Type,
                    Description = record.Description,
                    Amount = record.Amount,
                    Date = record.Date,
                    ReceiptReference = record.ReceiptReference
                };

                // Create the expense
                await _expenseService.CreateExpenseAsync(record.JobId, expenseRequest);
                result.SuccessCount++;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error at line {LineNumber}", lineNumber);
                result.Errors.Add(new ImportErrorDto
                {
                    LineNumber = lineNumber,
                    Message = ex.Message,
                    RawData = $"{record.JobId},{record.Type},{record.Description},{record.Amount},{record.Date}"
                });
                result.ErrorCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing expense at line {LineNumber}", lineNumber);
                result.Errors.Add(new ImportErrorDto
                {
                    LineNumber = lineNumber,
                    Message = $"Unexpected error: {ex.Message}",
                    RawData = $"{record.JobId},{record.Type},{record.Description},{record.Amount},{record.Date}"
                });
                result.ErrorCount++;
            }
        }

        return result;
    }

    /// <summary>
    /// Helper method to write CSV data
    /// </summary>
    private byte[] WriteCsv<T>(IEnumerable<T> records)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(records);
        writer.Flush();
        
        return memoryStream.ToArray();
    }
}
