using FluentValidation;
using GscTracking.Api.DTOs;
using GscTracking.Api.Models;

namespace GscTracking.Api.Validators;

public class JobRequestDtoValidator : AbstractValidator<JobRequestDto>
{
    public JobRequestDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

        RuleFor(x => x.EquipmentType)
            .NotEmpty().WithMessage("Equipment type is required")
            .MaximumLength(200).WithMessage("Equipment type cannot exceed 200 characters");

        RuleFor(x => x.EquipmentModel)
            .NotEmpty().WithMessage("Equipment model is required")
            .MaximumLength(200).WithMessage("Equipment model cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeValidStatus).WithMessage("Status must be one of: Quote, InProgress, Completed, Invoiced, Paid");

        RuleFor(x => x.DateReceived)
            .NotEmpty().WithMessage("Date received is required");

        RuleFor(x => x.DateCompleted)
            .GreaterThanOrEqualTo(x => x.DateReceived)
            .When(x => x.DateCompleted.HasValue)
            .WithMessage("Date completed must be on or after date received");

        RuleFor(x => x.EstimateAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimateAmount.HasValue)
            .WithMessage("Estimate amount must be greater than or equal to 0");

        RuleFor(x => x.ActualAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ActualAmount.HasValue)
            .WithMessage("Actual amount must be greater than or equal to 0");
    }

    private bool BeValidStatus(string status)
    {
        return Enum.TryParse<JobStatus>(status, ignoreCase: true, out _);
    }
}
