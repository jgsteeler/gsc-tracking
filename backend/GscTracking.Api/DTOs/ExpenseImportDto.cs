using CsvHelper.Configuration.Attributes;

namespace GscTracking.Api.DTOs;

/// <summary>
/// DTO for importing expenses from CSV
/// </summary>
public class ExpenseImportDto
{
    [Index(0)]
    [Name("Job ID")]
    public int JobId { get; set; }

    [Index(1)]
    [Name("Type")]
    public string Type { get; set; } = string.Empty;

    [Index(2)]
    [Name("Description")]
    public string Description { get; set; } = string.Empty;

    [Index(3)]
    [Name("Amount")]
    public decimal Amount { get; set; }

    [Index(4)]
    [Name("Date")]
    public DateTime Date { get; set; }

    [Index(5)]
    [Name("Receipt Reference")]
    public string? ReceiptReference { get; set; }
}
