namespace GscTracking.Application.DTOs;

public class JobRequestDto
{
    public int CustomerId { get; set; }
    public string EquipmentType { get; set; } = string.Empty;
    public string EquipmentModel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DateReceived { get; set; }
    public DateTime? DateCompleted { get; set; }
    public decimal? EstimateAmount { get; set; }
    public decimal? ActualAmount { get; set; }
}
