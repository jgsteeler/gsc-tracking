using System.Text;
using FluentAssertions;
using GscTracking.Api.Controllers;
using GscTracking.Api.DTOs;
using GscTracking.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GscTracking.Api.Tests.Controllers;

public class ImportControllerTests
{
    private readonly Mock<ICsvService> _mockCsvService;
    private readonly Mock<ILogger<ImportController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ImportController _controller;

    public ImportControllerTests()
    {
        _mockCsvService = new Mock<ICsvService>();
        _mockLogger = new Mock<ILogger<ImportController>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup configuration section for max file size
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(s => s.Value).Returns((10 * 1024 * 1024).ToString());
        _mockConfiguration.Setup(c => c.GetSection("CsvImport:MaxFileSizeBytes")).Returns(mockConfigSection.Object);
        
        _controller = new ImportController(_mockCsvService.Object, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task ImportExpenses_WithValidFile_ReturnsOkResultWithImportSummary()
    {
        // Arrange
        var csvContent = "Job ID,Type,Description,Amount,Date,Receipt Reference\n1,Parts,Oil filter,15.99,2025-01-15,REC-001";
        var fileMock = CreateMockFormFile(csvContent, "expenses.csv");
        var expectedResult = new ImportResultDto
        {
            SuccessCount = 1,
            ErrorCount = 0,
            Errors = new List<ImportErrorDto>()
        };

        _mockCsvService.Setup(s => s.ImportExpensesAsync(It.IsAny<Stream>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var importResult = okResult!.Value.Should().BeOfType<ImportResultDto>().Subject;
        importResult.SuccessCount.Should().Be(1);
        importResult.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportExpenses_WithErrors_ReturnsOkResultWithErrorDetails()
    {
        // Arrange
        var csvContent = "Job ID,Type,Description,Amount,Date,Receipt Reference\n999,Parts,Oil filter,15.99,2025-01-15,REC-001";
        var fileMock = CreateMockFormFile(csvContent, "expenses.csv");
        var expectedResult = new ImportResultDto
        {
            SuccessCount = 0,
            ErrorCount = 1,
            Errors = new List<ImportErrorDto>
            {
                new ImportErrorDto
                {
                    LineNumber = 2,
                    Message = "Job with ID 999 not found",
                    RawData = "999,Parts,Oil filter,15.99,2025-01-15"
                }
            }
        };

        _mockCsvService.Setup(s => s.ImportExpensesAsync(It.IsAny<Stream>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var importResult = okResult!.Value.Should().BeOfType<ImportResultDto>().Subject;
        importResult.SuccessCount.Should().Be(0);
        importResult.ErrorCount.Should().Be(1);
        importResult.Errors.Should().HaveCount(1);
        importResult.Errors[0].Message.Should().Contain("Job with ID 999 not found");
    }

    [Fact]
    public async Task ImportExpenses_WithNullFile_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ImportExpenses(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportExpenses_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        var fileMock = CreateMockFormFile("", "expenses.csv");

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ImportExpenses_WithNonCsvFile_ReturnsBadRequest()
    {
        // Arrange
        var fileMock = CreateMockFormFile("some content", "expenses.txt");

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var errorMessage = badRequest!.Value!.GetType().GetProperty("message")!.GetValue(badRequest.Value)!.ToString();
        errorMessage.Should().Contain("Only CSV files are supported");
    }

    [Fact]
    public async Task ImportExpenses_WithOversizedFile_ReturnsBadRequest()
    {
        // Arrange
        var largeContent = new string('x', 11 * 1024 * 1024); // 11 MB
        var fileMock = CreateMockFormFile(largeContent, "expenses.csv", largeContent.Length);

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var errorMessage = badRequest!.Value!.GetType().GetProperty("message")!.GetValue(badRequest.Value)!.ToString();
        errorMessage.Should().Contain("File size exceeds");
    }

    [Fact]
    public async Task ImportExpenses_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var csvContent = "Job ID,Type,Description,Amount,Date,Receipt Reference\n1,Parts,Oil filter,15.99,2025-01-15,REC-001";
        var fileMock = CreateMockFormFile(csvContent, "expenses.csv");

        _mockCsvService.Setup(s => s.ImportExpensesAsync(It.IsAny<Stream>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ImportExpenses(fileMock);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    private IFormFile CreateMockFormFile(string content, string fileName, long? length = null)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var fileMock = new Mock<IFormFile>();
        
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length ?? bytes.Length);
        fileMock.Setup(f => f.ContentType).Returns("text/csv");

        return fileMock.Object;
    }
}
