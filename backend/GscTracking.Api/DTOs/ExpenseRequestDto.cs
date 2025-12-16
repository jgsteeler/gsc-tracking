namespace GscTracking.Api.DTOs;

public class ExpenseRequestDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? ReceiptReference { get; set; }
}
