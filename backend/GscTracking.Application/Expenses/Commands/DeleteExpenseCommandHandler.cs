using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, bool>
{
    private readonly IExpenseRepository _expenseRepository;

    public DeleteExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        return await _expenseRepository.DeleteAsync(request.Id);
    }
}
