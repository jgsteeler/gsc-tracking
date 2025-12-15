namespace GscTracking.Api.DTOs;

public class JobUpdateDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string UpdateText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
