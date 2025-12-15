namespace GscTracking.Api.Models;

public class Job
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string EquipmentType { get; set; } = string.Empty;
    public string EquipmentModel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Quote;
    public DateTime DateReceived { get; set; } = DateTime.UtcNow;
    public DateTime? DateCompleted { get; set; }
    public decimal? EstimateAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public ICollection<JobUpdate> JobUpdates { get; set; } = new List<JobUpdate>();
}
