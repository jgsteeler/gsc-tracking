using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto?>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseDto?> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdAsync(request.Id);
        
        if (expense == null)
        {
            return null;
        }

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
