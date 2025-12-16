using FluentValidation.TestHelper;
using GscTracking.Api.DTOs;
using GscTracking.Api.Validators;
using Xunit;

namespace GscTracking.Api.Tests.Validators;

public class JobUpdateRequestDtoValidatorTests
{
    private readonly JobUpdateRequestDtoValidator _validator;

    public JobUpdateRequestDtoValidatorTests()
    {
        _validator = new JobUpdateRequestDtoValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UpdateText_Is_Empty()
    {
        var dto = new JobUpdateRequestDto { UpdateText = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.UpdateText)
            .WithErrorMessage("Update text is required");
    }

    [Fact]
    public void Should_Have_Error_When_UpdateText_Exceeds_4000_Characters()
    {
        var dto = new JobUpdateRequestDto { UpdateText = new string('a', 4001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.UpdateText)
            .WithErrorMessage("Update text cannot exceed 4000 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_UpdateText_Is_Valid()
    {
        var dto = new JobUpdateRequestDto
        {
            UpdateText = "Replaced spark plug and cleaned carburetor"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.UpdateText);
    }

    [Fact]
    public void Should_Not_Have_Error_When_UpdateText_Is_At_Max_Length()
    {
        var dto = new JobUpdateRequestDto { UpdateText = new string('a', 4000) };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.UpdateText);
    }
}
