using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public class CalculateTotalCostQueryHandler : IRequestHandler<CalculateTotalCostQuery, decimal>
{
    private readonly IExpenseRepository _expenseRepository;

    public CalculateTotalCostQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<decimal> Handle(CalculateTotalCostQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _expenseRepository.GetExpensesByJobIdAsync(request.JobId, cancellationToken);
        return expenses.Sum(e => e.Amount);
    }
}
