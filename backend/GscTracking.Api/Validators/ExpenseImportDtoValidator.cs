using FluentValidation;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Validators;

public class ExpenseImportDtoValidator : AbstractValidator<ExpenseImportDto>
{
    public ExpenseImportDtoValidator()
    {
        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("Job ID must be greater than 0");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Expense type is required")
            .Must(BeValidExpenseType).WithMessage("Expense type must be one of: Parts, Labor, Service");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");

        RuleFor(x => x.ReceiptReference)
            .MaximumLength(200).WithMessage("Receipt reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ReceiptReference));
    }

    private bool BeValidExpenseType(string type)
    {
        return Enum.TryParse<ExpenseType>(type, ignoreCase: true, out _);
    }
}
