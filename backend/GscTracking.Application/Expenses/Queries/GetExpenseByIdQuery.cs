using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public record GetExpenseByIdQuery(int Id) : IRequest<ExpenseDto?>;
