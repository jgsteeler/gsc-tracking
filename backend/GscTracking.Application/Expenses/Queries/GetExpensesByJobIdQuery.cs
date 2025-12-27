using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public record GetExpensesByJobIdQuery(int JobId) : IRequest<IEnumerable<ExpenseDto>>;
