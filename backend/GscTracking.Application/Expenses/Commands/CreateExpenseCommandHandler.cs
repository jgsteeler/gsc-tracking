using GscTracking.Application.DTOs;
using GscTracking.Core.Entities;
using GscTracking.Core.Enums;
using GscTracking.Core.Interfaces;
using MediatR;

namespace GscTracking.Application.Expenses.Commands;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // Parse expense type from string
        if (!Enum.TryParse<ExpenseType>(request.ExpenseRequest.Type, true, out var expenseType))
        {
            expenseType = ExpenseType.Parts; // Default type
        }

        var expense = new Expense
        {
            JobId = request.JobId,
            Type = expenseType,
            Description = request.ExpenseRequest.Description,
            Amount = request.ExpenseRequest.Amount,
            Date = request.ExpenseRequest.Date,
            ReceiptReference = request.ExpenseRequest.ReceiptReference,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdExpense = await _expenseRepository.AddAsync(expense);
        await _expenseRepository.SaveChangesAsync();

        return new ExpenseDto
        {
            Id = createdExpense.Id,
            JobId = createdExpense.JobId,
            Type = createdExpense.Type.ToString(),
            Description = createdExpense.Description,
            Amount = createdExpense.Amount,
            Date = createdExpense.Date,
            ReceiptReference = createdExpense.ReceiptReference,
            CreatedAt = createdExpense.CreatedAt,
            UpdatedAt = createdExpense.UpdatedAt
        };
    }
}
