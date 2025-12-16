using FluentAssertions;
using FluentValidation.TestHelper;
using GscTracking.Api.DTOs;
using GscTracking.Api.Validators;

namespace GscTracking.Api.Tests.Validators;

public class ExpenseRequestDtoValidatorTests
{
    private readonly ExpenseRequestDtoValidator _validator;

    public ExpenseRequestDtoValidatorTests()
    {
        _validator = new ExpenseRequestDtoValidator();
    }

    [Fact]
    public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Oil filter replacement",
            Amount = 15.50m,
            Date = DateTime.UtcNow,
            ReceiptReference = "REC-001"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Parts")]
    [InlineData("Labor")]
    [InlineData("Service")]
    public void Validate_ShouldNotHaveError_WhenTypeIsValid(string type)
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = type,
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsEmpty()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "",
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Expense type is required");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "InvalidType",
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Expense type must be one of: Parts, Labor, Service");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "",
            Amount = 10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = new string('a', 501),
            Amount = 10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 500 characters");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsZero()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 0,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsNegative()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = -10.00m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenAmountIsPositive()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 10.50m,
            Date = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDateIsNotProvided()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 10.00m,
            Date = default
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date)
            .WithErrorMessage("Date is required");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenReceiptReferenceIsNull()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow,
            ReceiptReference = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ReceiptReference);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReceiptReferenceExceedsMaxLength()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow,
            ReceiptReference = new string('a', 201)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReceiptReference)
            .WithErrorMessage("Receipt reference cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenReceiptReferenceIsWithinMaxLength()
    {
        // Arrange
        var dto = new ExpenseRequestDto
        {
            Type = "Parts",
            Description = "Test",
            Amount = 10.00m,
            Date = DateTime.UtcNow,
            ReceiptReference = new string('a', 200)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ReceiptReference);
    }
}
