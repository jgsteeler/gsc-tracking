using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public record DeleteExpenseCommand(int Id) : IRequest<bool>;
