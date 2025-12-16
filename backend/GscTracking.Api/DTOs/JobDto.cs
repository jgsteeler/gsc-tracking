namespace GscTracking.Api.DTOs;

public class JobDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string EquipmentModel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DateReceived { get; set; }
    public DateTime? DateCompleted { get; set; }
    public decimal? EstimateAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal? ProfitMargin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
