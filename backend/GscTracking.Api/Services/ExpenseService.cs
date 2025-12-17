using Microsoft.EntityFrameworkCore;
using GscTracking.Api.Data;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Services;

public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _context;

    public ExpenseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesByJobIdAsync(int jobId)
    {
        var expenses = await _context.Expense
            .Where(e => e.JobId == jobId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return expenses.Select(e => MapToDto(e));
    }

    public async Task<ExpenseDto?> GetExpenseByIdAsync(int id)
    {
        var expense = await _context.Expense
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (expense == null)
        {
            return null;
        }

        return MapToDto(expense);
    }

    public async Task<ExpenseDto> CreateExpenseAsync(int jobId, ExpenseRequestDto expenseRequest)
    {
        // Validate that the job exists
        var jobExists = await _context.Job.AnyAsync(j => j.Id == jobId);
        if (!jobExists)
        {
            throw new ArgumentException($"Job with ID {jobId} not found");
        }

        // Parse the type string to enum
        if (!Enum.TryParse<ExpenseType>(expenseRequest.Type, ignoreCase: true, out var type))
        {
            throw new ArgumentException($"Invalid expense type: {expenseRequest.Type}");
        }

        var expense = new Expense
        {
            JobId = jobId,
            Type = type,
            Description = expenseRequest.Description,
            Amount = expenseRequest.Amount,
            Date = expenseRequest.Date,
            ReceiptReference = expenseRequest.ReceiptReference,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Expense.Add(expense);
        await _context.SaveChangesAsync();

        return MapToDto(expense);
    }

    public async Task<ExpenseDto?> UpdateExpenseAsync(int id, ExpenseRequestDto expenseRequest)
    {
        var expense = await _context.Expense
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (expense == null)
        {
            return null;
        }

        // Parse the type string to enum
        if (!Enum.TryParse<ExpenseType>(expenseRequest.Type, ignoreCase: true, out var type))
        {
            throw new ArgumentException($"Invalid expense type: {expenseRequest.Type}");
        }

        expense.Type = type;
        expense.Description = expenseRequest.Description;
        expense.Amount = expenseRequest.Amount;
        expense.Date = expenseRequest.Date;
        expense.ReceiptReference = expenseRequest.ReceiptReference;
        expense.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(expense);
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var expense = await _context.Expense
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (expense == null)
        {
            return false;
        }

        // Verify the expense belongs to a valid job before deletion
        var jobExists = await _context.Job.AnyAsync(j => j.Id == expense.JobId);
        if (!jobExists)
        {
            throw new InvalidOperationException($"Cannot delete expense {id}: associated job {expense.JobId} does not exist");
        }

        _context.Expense.Remove(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<decimal> CalculateTotalCostAsync(int jobId)
    {
        return await _context.Expense
            .Where(e => e.JobId == jobId)
            .SumAsync(e => e.Amount);
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            JobId = expense.JobId,
            Type = expense.Type.ToString(),
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            ReceiptReference = expense.ReceiptReference,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
