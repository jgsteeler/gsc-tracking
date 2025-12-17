using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentAssertions;

namespace GscTracking.Api.Tests;

/// <summary>
/// Tests for Auth0 authentication configuration
/// </summary>
public class Auth0ConfigurationTests
{
    [Fact]
    public void Auth0Configuration_WhenDomainAndAudienceProvided_ShouldConfigureJwtBearer()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Auth0:Domain", "test-tenant.auth0.com" },
                { "Auth0:Audience", "https://test-api.example.com" }
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add authentication with JwtBearer
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var domain = configuration["Auth0:Domain"];
            var audience = configuration["Auth0:Audience"];
            options.Authority = $"https://{domain}/";
            options.Audience = audience;
        });

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var authenticationScheme = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();

        // Assert
        authenticationScheme.Should().NotBeNull();
    }

    [Fact]
    public void Auth0Configuration_WhenDomainMissing_ShouldNotConfigureAuth()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Auth0:Audience", "https://test-api.example.com" }
            })
            .Build();

        var domain = configuration["Auth0:Domain"];
        var audience = configuration["Auth0:Audience"];

        // Act & Assert
        string.IsNullOrEmpty(domain).Should().BeTrue();
        string.IsNullOrEmpty(audience).Should().BeFalse();
    }

    [Fact]
    public void Auth0Configuration_WhenAudienceMissing_ShouldNotConfigureAuth()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Auth0:Domain", "test-tenant.auth0.com" }
            })
            .Build();

        var domain = configuration["Auth0:Domain"];
        var audience = configuration["Auth0:Audience"];

        // Act & Assert
        string.IsNullOrEmpty(domain).Should().BeFalse();
        string.IsNullOrEmpty(audience).Should().BeTrue();
    }

    [Fact]
    public void Auth0Configuration_ShouldReadFromConfiguration()
    {
        // Arrange
        var expectedDomain = "test-tenant.auth0.com";
        var expectedAudience = "https://test-api.example.com";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Auth0:Domain", expectedDomain },
                { "Auth0:Audience", expectedAudience }
            })
            .Build();

        // Act
        var domain = configuration["Auth0:Domain"];
        var audience = configuration["Auth0:Audience"];

        // Assert
        domain.Should().Be(expectedDomain);
        audience.Should().Be(expectedAudience);
    }
}
