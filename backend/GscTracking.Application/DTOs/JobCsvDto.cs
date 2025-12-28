using CsvHelper.Configuration.Attributes;

namespace GscTracking.Application.DTOs;

/// <summary>
/// DTO for job CSV export with estimate/invoice data
/// </summary>
public class JobCsvDto
{
    [Index(0)]
    [Name("Job ID")]
    public int Id { get; set; }

    [Index(1)]
    [Name("Customer ID")]
    public int CustomerId { get; set; }

    [Index(2)]
    [Name("Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Index(3)]
    [Name("Equipment Type")]
    public string EquipmentType { get; set; } = string.Empty;

    [Index(4)]
    [Name("Equipment Model")]
    public string EquipmentModel { get; set; } = string.Empty;

    [Index(5)]
    [Name("Description")]
    public string Description { get; set; } = string.Empty;

    [Index(6)]
    [Name("Status")]
    public string Status { get; set; } = string.Empty;

    [Index(7)]
    [Name("Date Received")]
    public DateTime DateReceived { get; set; }

    [Index(8)]
    [Name("Date Completed")]
    public DateTime? DateCompleted { get; set; }

    [Index(9)]
    [Name("Estimate Amount")]
    public decimal? EstimateAmount { get; set; }

    [Index(10)]
    [Name("Actual Amount")]
    public decimal? ActualAmount { get; set; }

    [Index(11)]
    [Name("Total Cost")]
    public decimal TotalCost { get; set; }

    [Index(12)]
    [Name("Profit Margin")]
    public decimal? ProfitMargin { get; set; }

    [Index(13)]
    [Name("Created At")]
    public DateTime CreatedAt { get; set; }

    [Index(14)]
    [Name("Updated At")]
    public DateTime UpdatedAt { get; set; }
}
