using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace GscTracking.Api.Tests;

public class CorsConfigurationTests
{
    private readonly string _netlifyPreviewPattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";

    [Theory]
    [InlineData("http://localhost:5173", true)]
    [InlineData("http://localhost:5174", true)]
    [InlineData("http://localhost:3000", true)]
    [InlineData("https://localhost:5173", false)]
    [InlineData("http://localhost:8080", false)]
    [InlineData("http://localhost", false)]
    public void LocalhostOrigins_WithConfiguredPorts_AreValidatedCorrectly(string origin, bool expectedResult)
    {
        // Arrange
        var allowedLocalPorts = new[] { "5173", "5174", "3000" };

        // Act
        var isAllowed = false;
        foreach (var port in allowedLocalPorts)
        {
            if (origin == $"http://localhost:{port.Trim()}")
            {
                isAllowed = true;
                break;
            }
        }

        // Assert
        isAllowed.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("https://gsc-tracking-ui.netlify.app", true)]
    [InlineData("http://gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://gsc-tracking-ui.netlify.app/", false)]
    [InlineData("https://gsc-tracking-ui.netlify.app:443", false)]
    [InlineData("https://different-app.netlify.app", false)]
    public void ProductionOrigin_IsValidatedCorrectly(string origin, bool expectedResult)
    {
        // Act
        var isAllowed = origin == "https://gsc-tracking-ui.netlify.app";

        // Assert
        isAllowed.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("https://staging--gsc-tracking-ui.netlify.app", true)]
    [InlineData("http://staging--gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://staging--gsc-tracking-ui.netlify.app/", false)]
    [InlineData("https://staging-gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://staging--different-app.netlify.app", false)]
    public void StagingOrigin_IsValidatedCorrectly(string origin, bool expectedResult)
    {
        // Act
        var isAllowed = origin == "https://staging--gsc-tracking-ui.netlify.app";

        // Assert
        isAllowed.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("https://deploy-preview-1--gsc-tracking-ui.netlify.app", true)]
    [InlineData("https://deploy-preview-123--gsc-tracking-ui.netlify.app", true)]
    [InlineData("https://deploy-preview-999999--gsc-tracking-ui.netlify.app", true)]
    [InlineData("https://deploy-preview-0--gsc-tracking-ui.netlify.app", true)]
    [InlineData("http://deploy-preview-1--gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://deploy-preview-abc--gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://deploy-preview--gsc-tracking-ui.netlify.app", false)]
    [InlineData("https://deploy-preview-1--different-app.netlify.app", false)]
    [InlineData("https://deploy-preview-1--gsc-tracking-ui.netlify.app/", false)]
    [InlineData("https://deploy-preview-1--gsc-tracking-ui.netlify.app:443", false)]
    [InlineData("https://preview-1--gsc-tracking-ui.netlify.app", false)]
    public void NetlifyDeployPreviewOrigins_MatchPatternCorrectly(string origin, bool expectedResult)
    {
        // Act
        var isAllowed = Regex.IsMatch(origin, _netlifyPreviewPattern);

        // Assert
        isAllowed.Should().Be(expectedResult, $"origin '{origin}' should {(expectedResult ? "match" : "not match")} the pattern");
    }

    [Theory]
    [InlineData("https://malicious-site.com", false)]
    [InlineData("https://evil.netlify.app", false)]
    [InlineData("http://gsc-tracking-ui.netlify.app", false)]
    [InlineData("", false)]
    [InlineData("not-a-url", false)]
    [InlineData("ftp://localhost:5173", false)]
    public void UnauthorizedOrigins_AreRejected(string origin, bool expectedResult)
    {
        // Arrange
        var allowedLocalPorts = new[] { "5173", "5174", "3000" };

        // Act - Simulate the full CORS validation logic
        var isAllowed = false;

        // Check localhost ports
        foreach (var port in allowedLocalPorts)
        {
            if (origin == $"http://localhost:{port.Trim()}")
            {
                isAllowed = true;
                break;
            }
        }

        // Check production
        if (origin == "https://gsc-tracking-ui.netlify.app")
            isAllowed = true;

        // Check staging
        if (origin == "https://staging--gsc-tracking-ui.netlify.app")
            isAllowed = true;

        // Check deploy previews
        if (Regex.IsMatch(origin, _netlifyPreviewPattern))
            isAllowed = true;

        // Assert
        isAllowed.Should().Be(expectedResult);
    }

    [Fact]
    public void AllowedLocalPorts_Configuration_ParsesCommaSeparatedValues()
    {
        // Arrange
        var configValue = "5173,5174,3000";

        // Act
        var ports = configValue.Split(',');

        // Assert
        ports.Should().HaveCount(3);
        ports.Should().Contain("5173");
        ports.Should().Contain("5174");
        ports.Should().Contain("3000");
    }

    [Fact]
    public void AllowedLocalPorts_Configuration_HandlesWhitespace()
    {
        // Arrange
        var configValue = "5173, 5174 , 3000";

        // Act
        var ports = configValue.Split(',').Select(p => p.Trim()).ToArray();

        // Assert
        ports.Should().HaveCount(3);
        ports.Should().AllSatisfy(p => p.Should().NotContain(" "));
    }

    [Fact]
    public void AllowedLocalPorts_Configuration_DefaultsTo5173WhenNull()
    {
        // Arrange
        string? configValue = null;

        // Act
        var ports = configValue?.Split(',') ?? new[] { "5173" };

        // Assert
        ports.Should().HaveCount(1);
        ports.Should().Contain("5173");
    }

    [Fact]
    public void NetlifyPreviewPattern_MatchesExpectedFormat()
    {
        // Arrange
        var pattern = @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$";

        // Act & Assert
        Regex.IsMatch("https://deploy-preview-42--gsc-tracking-ui.netlify.app", pattern).Should().BeTrue();
        Regex.IsMatch("https://deploy-preview-1--gsc-tracking-ui.netlify.app", pattern).Should().BeTrue();
        Regex.IsMatch("https://deploy-preview-999--gsc-tracking-ui.netlify.app", pattern).Should().BeTrue();
    }
}
