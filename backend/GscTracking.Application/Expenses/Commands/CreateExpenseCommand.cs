using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public record CreateExpenseCommand(int JobId, ExpenseRequestDto ExpenseRequest) : IRequest<ExpenseDto>;
