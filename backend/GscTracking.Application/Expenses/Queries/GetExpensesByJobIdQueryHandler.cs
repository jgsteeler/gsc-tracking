using GscTracking.Application.DTOs;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public class GetExpensesByJobIdQueryHandler : IRequestHandler<GetExpensesByJobIdQuery, IEnumerable<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpensesByJobIdQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<ExpenseDto>> Handle(GetExpensesByJobIdQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _expenseRepository.GetExpensesByJobIdAsync(request.JobId);

        return expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            JobId = e.JobId,
            Type = e.Type.ToString(),
            Description = e.Description,
            Amount = e.Amount,
            Date = e.Date,
            ReceiptReference = e.ReceiptReference,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        });
    }
}
