using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public record UpdateExpenseCommand(int Id, ExpenseRequestDto ExpenseRequest) : IRequest<ExpenseDto?>;
