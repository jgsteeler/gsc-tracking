using FluentValidation.TestHelper;
using GscTracking.Api.DTOs;
using GscTracking.Api.Validators;
using Xunit;

namespace GscTracking.Api.Tests.Validators;

public class CustomerRequestDtoValidatorTests
{
    private readonly CustomerRequestDtoValidator _validator;

    public CustomerRequestDtoValidatorTests()
    {
        _validator = new CustomerRequestDtoValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var dto = new CustomerRequestDto { Name = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_200_Characters()
    {
        var dto = new CustomerRequestDto { Name = new string('a', 201) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var dto = new CustomerRequestDto { Name = "John Doe" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Email = "invalid-email"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_200_Characters()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Email = new string('a', 192) + "@test.com" // 204 characters
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email cannot exceed 200 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Null()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Email = null
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Empty()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Email = ""
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Phone_Exceeds_50_Characters()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Phone = new string('1', 51)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone cannot exceed 50 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Phone_Is_Valid()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Phone = "(555) 123-4567"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Should_Have_Error_When_Address_Exceeds_500_Characters()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Address = new string('a', 501)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address cannot exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Address_Is_Valid()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Address = "123 Main St, City, State ZIP"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Should_Have_Error_When_Notes_Exceeds_2000_Characters()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Notes = new string('a', 2001)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes cannot exceed 2000 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Notes_Is_Valid()
    {
        var dto = new CustomerRequestDto
        {
            Name = "John Doe",
            Notes = "Some notes about the customer"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }
}
