using CsvHelper.Configuration.Attributes;

namespace GscTracking.Api.DTOs;

/// <summary>
/// DTO for expense CSV export/import operations
/// </summary>
public class ExpenseCsvDto
{
    [Index(0)]
    [Name("Expense ID")]
    public int Id { get; set; }

    [Index(1)]
    [Name("Job ID")]
    public int JobId { get; set; }

    [Index(2)]
    [Name("Type")]
    public string Type { get; set; } = string.Empty;

    [Index(3)]
    [Name("Description")]
    public string Description { get; set; } = string.Empty;

    [Index(4)]
    [Name("Amount")]
    public decimal Amount { get; set; }

    [Index(5)]
    [Name("Date")]
    public DateTime Date { get; set; }

    [Index(6)]
    [Name("Receipt Reference")]
    public string? ReceiptReference { get; set; }

    [Index(7)]
    [Name("Created At")]
    public DateTime CreatedAt { get; set; }

    [Index(8)]
    [Name("Updated At")]
    public DateTime UpdatedAt { get; set; }
}
