using GscTracking.Api.DTOs;

namespace GscTracking.Api.Services;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetExpensesByJobIdAsync(int jobId);
    Task<ExpenseDto?> GetExpenseByIdAsync(int id);
    Task<ExpenseDto> CreateExpenseAsync(int jobId, ExpenseRequestDto expenseRequest);
    Task<ExpenseDto?> UpdateExpenseAsync(int id, ExpenseRequestDto expenseRequest);
    Task<bool> DeleteExpenseAsync(int id);
    Task<decimal> CalculateTotalCostAsync(int jobId);
}
