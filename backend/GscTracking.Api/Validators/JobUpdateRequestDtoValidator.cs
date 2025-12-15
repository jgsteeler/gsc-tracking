using FluentValidation;
using GscTracking.Api.DTOs;

namespace GscTracking.Api.Validators;

public class JobUpdateRequestDtoValidator : AbstractValidator<JobUpdateRequestDto>
{
    public JobUpdateRequestDtoValidator()
    {
        RuleFor(x => x.UpdateText)
            .NotEmpty().WithMessage("Update text is required")
            .MaximumLength(4000).WithMessage("Update text cannot exceed 4000 characters");
    }
}
