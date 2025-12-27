using GscTracking.Application.DTOs;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ExpenseDto?>
{
    private readonly IExpenseRepository _expenseRepository;

    public UpdateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseDto?> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdAsync(request.Id);
        
        if (expense == null)
        {
            return null;
        }

        // Parse expense type from string
        if (Enum.TryParse<ExpenseType>(request.ExpenseRequest.Type, true, out var expenseType))
        {
            expense.Type = expenseType;
        }

        expense.Description = request.ExpenseRequest.Description;
        expense.Amount = request.ExpenseRequest.Amount;
        expense.Date = request.ExpenseRequest.Date;
        expense.ReceiptReference = request.ExpenseRequest.ReceiptReference;
        expense.UpdatedAt = DateTime.UtcNow;

        await _expenseRepository.UpdateAsync(expense);
        await _expenseRepository.SaveChangesAsync();

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
