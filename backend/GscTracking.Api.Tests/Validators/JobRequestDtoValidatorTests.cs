using FluentValidation.TestHelper;
using GscTracking.Api.DTOs;
using GscTracking.Api.Validators;
using Xunit;

namespace GscTracking.Api.Tests.Validators;

public class JobRequestDtoValidatorTests
{
    private readonly JobRequestDtoValidator _validator;

    public JobRequestDtoValidatorTests()
    {
        _validator = new JobRequestDtoValidator();
    }

    private JobRequestDto CreateValidDto()
    {
        return new JobRequestDto
        {
            CustomerId = 1,
            EquipmentType = "Lawn Mower",
            EquipmentModel = "Honda HRX217",
            Description = "Needs blade sharpening",
            Status = "Quote",
            DateReceived = DateTime.UtcNow
        };
    }

    [Fact]
    public void Should_Have_Error_When_CustomerId_Is_Zero()
    {
        var dto = CreateValidDto();
        dto.CustomerId = 0;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId)
            .WithErrorMessage("Customer ID must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_CustomerId_Is_Negative()
    {
        var dto = CreateValidDto();
        dto.CustomerId = -1;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_CustomerId_Is_Valid()
    {
        var dto = CreateValidDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Should_Have_Error_When_EquipmentType_Is_Empty()
    {
        var dto = CreateValidDto();
        dto.EquipmentType = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentType)
            .WithErrorMessage("Equipment type is required");
    }

    [Fact]
    public void Should_Have_Error_When_EquipmentType_Exceeds_200_Characters()
    {
        var dto = CreateValidDto();
        dto.EquipmentType = new string('a', 201);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentType)
            .WithErrorMessage("Equipment type cannot exceed 200 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_EquipmentType_Is_Valid()
    {
        var dto = CreateValidDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.EquipmentType);
    }

    [Fact]
    public void Should_Have_Error_When_EquipmentModel_Is_Empty()
    {
        var dto = CreateValidDto();
        dto.EquipmentModel = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentModel)
            .WithErrorMessage("Equipment model is required");
    }

    [Fact]
    public void Should_Have_Error_When_EquipmentModel_Exceeds_200_Characters()
    {
        var dto = CreateValidDto();
        dto.EquipmentModel = new string('a', 201);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EquipmentModel)
            .WithErrorMessage("Equipment model cannot exceed 200 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_EquipmentModel_Is_Valid()
    {
        var dto = CreateValidDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.EquipmentModel);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var dto = CreateValidDto();
        dto.Description = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_2000_Characters()
    {
        var dto = CreateValidDto();
        dto.Description = new string('a', 2001);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Valid()
    {
        var dto = CreateValidDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Status_Is_Empty()
    {
        var dto = CreateValidDto();
        dto.Status = "";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status is required");
    }

    [Theory]
    [InlineData("Quote")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    [InlineData("Invoiced")]
    [InlineData("Paid")]
    public void Should_Not_Have_Error_When_Status_Is_Valid(string status)
    {
        var dto = CreateValidDto();
        dto.Status = status;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Have_Error_When_Status_Is_Invalid()
    {
        var dto = CreateValidDto();
        dto.Status = "InvalidStatus";
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must be one of: Quote, InProgress, Completed, Invoiced, Paid");
    }

    [Fact]
    public void Should_Have_Error_When_DateReceived_Is_Default()
    {
        var dto = CreateValidDto();
        dto.DateReceived = default;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DateReceived)
            .WithErrorMessage("Date received is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_DateReceived_Is_Valid()
    {
        var dto = CreateValidDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.DateReceived);
    }

    [Fact]
    public void Should_Have_Error_When_DateCompleted_Is_Before_DateReceived()
    {
        var dto = CreateValidDto();
        dto.DateReceived = DateTime.UtcNow;
        dto.DateCompleted = DateTime.UtcNow.AddDays(-1);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DateCompleted)
            .WithErrorMessage("Date completed must be on or after date received");
    }

    [Fact]
    public void Should_Not_Have_Error_When_DateCompleted_Is_After_DateReceived()
    {
        var dto = CreateValidDto();
        dto.DateReceived = DateTime.UtcNow;
        dto.DateCompleted = DateTime.UtcNow.AddDays(1);
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.DateCompleted);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DateCompleted_Equals_DateReceived()
    {
        var dto = CreateValidDto();
        var date = DateTime.UtcNow;
        dto.DateReceived = date;
        dto.DateCompleted = date;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.DateCompleted);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DateCompleted_Is_Null()
    {
        var dto = CreateValidDto();
        dto.DateCompleted = null;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.DateCompleted);
    }

    [Fact]
    public void Should_Have_Error_When_EstimateAmount_Is_Negative()
    {
        var dto = CreateValidDto();
        dto.EstimateAmount = -10;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.EstimateAmount)
            .WithErrorMessage("Estimate amount must be greater than or equal to 0");
    }

    [Fact]
    public void Should_Not_Have_Error_When_EstimateAmount_Is_Zero()
    {
        var dto = CreateValidDto();
        dto.EstimateAmount = 0;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.EstimateAmount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_EstimateAmount_Is_Positive()
    {
        var dto = CreateValidDto();
        dto.EstimateAmount = 150.50m;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.EstimateAmount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_EstimateAmount_Is_Null()
    {
        var dto = CreateValidDto();
        dto.EstimateAmount = null;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.EstimateAmount);
    }

    [Fact]
    public void Should_Have_Error_When_ActualAmount_Is_Negative()
    {
        var dto = CreateValidDto();
        dto.ActualAmount = -5;
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ActualAmount)
            .WithErrorMessage("Actual amount must be greater than or equal to 0");
    }

    [Fact]
    public void Should_Not_Have_Error_When_ActualAmount_Is_Zero()
    {
        var dto = CreateValidDto();
        dto.ActualAmount = 0;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ActualAmount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ActualAmount_Is_Positive()
    {
        var dto = CreateValidDto();
        dto.ActualAmount = 200.75m;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ActualAmount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ActualAmount_Is_Null()
    {
        var dto = CreateValidDto();
        dto.ActualAmount = null;
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ActualAmount);
    }
}
