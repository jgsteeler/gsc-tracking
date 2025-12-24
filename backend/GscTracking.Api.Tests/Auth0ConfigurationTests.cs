using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentAssertions;
using System.Security.Claims;

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
    public void Auth0Configuration_WhenDomainMissing_ShouldBeDetectableAsNull()
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
        // Note: In actual application startup, this condition would cause app to fail
        string.IsNullOrEmpty(domain).Should().BeTrue();
        string.IsNullOrEmpty(audience).Should().BeFalse();
    }

    [Fact]
    public void Auth0Configuration_WhenAudienceMissing_ShouldBeDetectableAsNull()
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
        // Note: In actual application startup, this condition would cause app to fail
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

    [Theory]
    [InlineData("https://gsc-tracking.com/roles")]
    [InlineData("http://gsc-tracking.com/roles")]
    [InlineData("roles")]
    public void Auth0RoleClaimMapping_ShouldMapCustomRoleClaimsToStandardRoleClaims(string roleClaimType)
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity("test");
        claimsIdentity.AddClaim(new Claim(roleClaimType, "tracker-admin"));
        claimsIdentity.AddClaim(new Claim(roleClaimType, "tracker-user"));

        // Simulate the OnTokenValidated claim transformation logic from Program.cs
        var possibleRoleClaims = new[]
        {
            "https://gsc-tracking.com/roles",
            "http://gsc-tracking.com/roles",
            "roles"
        };

        foreach (var claimType in possibleRoleClaims)
        {
            var roleClaims = claimsIdentity.FindAll(claimType).ToList();
            if (roleClaims.Any())
            {
                foreach (var roleClaim in roleClaims)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
                break;
            }
        }

        // Act
        var standardRoleClaims = claimsIdentity.FindAll(ClaimTypes.Role).ToList();

        // Assert
        standardRoleClaims.Should().HaveCount(2);
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-admin");
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-user");
    }

    [Fact]
    public void Auth0RoleClaimMapping_WhenNoRoleClaimsPresent_ShouldNotAddStandardRoleClaims()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity("test");
        claimsIdentity.AddClaim(new Claim("sub", "user123"));
        claimsIdentity.AddClaim(new Claim("email", "test@example.com"));

        // Simulate the OnTokenValidated claim transformation logic from Program.cs
        var possibleRoleClaims = new[]
        {
            "https://gsc-tracking.com/roles",
            "http://gsc-tracking.com/roles",
            "roles"
        };

        foreach (var claimType in possibleRoleClaims)
        {
            var roleClaims = claimsIdentity.FindAll(claimType).ToList();
            if (roleClaims.Any())
            {
                foreach (var roleClaim in roleClaims)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
                break;
            }
        }

        // Act
        var standardRoleClaims = claimsIdentity.FindAll(ClaimTypes.Role).ToList();

        // Assert
        standardRoleClaims.Should().BeEmpty();
    }

    [Fact]
    public void Auth0RoleClaimMapping_ShouldPrioritizeHttpsNamespace()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity("test");
        // Add roles with different namespaces
        claimsIdentity.AddClaim(new Claim("https://gsc-tracking.com/roles", "tracker-admin"));
        claimsIdentity.AddClaim(new Claim("http://gsc-tracking.com/roles", "other-role"));
        claimsIdentity.AddClaim(new Claim("roles", "another-role"));

        // Simulate the OnTokenValidated claim transformation logic from Program.cs
        // This should only process the first matching namespace
        var possibleRoleClaims = new[]
        {
            "https://gsc-tracking.com/roles",
            "http://gsc-tracking.com/roles",
            "roles"
        };

        foreach (var claimType in possibleRoleClaims)
        {
            var roleClaims = claimsIdentity.FindAll(claimType).ToList();
            if (roleClaims.Any())
            {
                foreach (var roleClaim in roleClaims)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
                break; // Only process the first matching namespace
            }
        }

        // Act
        var standardRoleClaims = claimsIdentity.FindAll(ClaimTypes.Role).ToList();

        // Assert
        standardRoleClaims.Should().HaveCount(1);
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-admin");
        standardRoleClaims.Should().NotContain(c => c.Value == "other-role");
        standardRoleClaims.Should().NotContain(c => c.Value == "another-role");
    }

    [Theory]
    [InlineData("https://gsc-tracking.com/permissions")]
    [InlineData("http://gsc-tracking.com/permissions")]
    [InlineData("permissions")]
    public void Auth0PermissionClaimMapping_ShouldMapPermissionsToTrackerRoles(string permissionClaimType)
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(permissionClaimType, "admin"));
        claimsIdentity.AddClaim(new Claim(permissionClaimType, "write"));
        claimsIdentity.AddClaim(new Claim(permissionClaimType, "read"));

        // Simulate the claim transformation logic from Program.cs
        var possiblePermissionClaims = new[]
        {
            "https://gsc-tracking.com/permissions",
            "http://gsc-tracking.com/permissions",
            "permissions"
        };

        bool foundPermissions = false;
        foreach (var claimType in possiblePermissionClaims)
        {
            var permissionClaims = claimsIdentity.FindAll(claimType).ToList();
            if (permissionClaims.Any())
            {
                foreach (var permissionClaim in permissionClaims)
                {
                    var mappedRole = permissionClaim.Value switch
                    {
                        "admin" => "tracker-admin",
                        "write" => "tracker-write",
                        "read" => "tracker-read",
                        _ => permissionClaim.Value
                    };
                    
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, mappedRole));
                }
                foundPermissions = true;
                break;
            }
        }

        // Act
        var standardRoleClaims = claimsIdentity.FindAll(ClaimTypes.Role).ToList();

        // Assert
        foundPermissions.Should().BeTrue();
        standardRoleClaims.Should().HaveCount(3);
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-admin");
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-write");
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-read");
    }

    [Fact]
    public void Auth0PermissionClaimMapping_ShouldPrioritizePermissionsOverRoles()
    {
        // Arrange
        var claimsIdentity = new ClaimsIdentity();
        // Add both permissions and roles
        claimsIdentity.AddClaim(new Claim("https://gsc-tracking.com/permissions", "admin"));
        claimsIdentity.AddClaim(new Claim("https://gsc-tracking.com/roles", "tracker-read"));

        // Simulate the claim transformation logic from Program.cs
        var possiblePermissionClaims = new[]
        {
            "https://gsc-tracking.com/permissions",
            "http://gsc-tracking.com/permissions",
            "permissions"
        };

        bool foundPermissions = false;
        foreach (var claimType in possiblePermissionClaims)
        {
            var permissionClaims = claimsIdentity.FindAll(claimType).ToList();
            if (permissionClaims.Any())
            {
                foreach (var permissionClaim in permissionClaims)
                {
                    var mappedRole = permissionClaim.Value switch
                    {
                        "admin" => "tracker-admin",
                        "write" => "tracker-write",
                        "read" => "tracker-read",
                        _ => permissionClaim.Value
                    };
                    
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, mappedRole));
                }
                foundPermissions = true;
                break;
            }
        }

        // Only check roles if no permissions were found
        if (!foundPermissions)
        {
            var possibleRoleClaims = new[]
            {
                "https://gsc-tracking.com/roles",
                "http://gsc-tracking.com/roles",
                "roles"
            };

            foreach (var roleClaimType in possibleRoleClaims)
            {
                var roleClaims = claimsIdentity.FindAll(roleClaimType).ToList();
                if (roleClaims.Any())
                {
                    foreach (var roleClaim in roleClaims)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }
                    break;
                }
            }
        }

        // Act
        var standardRoleClaims = claimsIdentity.FindAll(ClaimTypes.Role).ToList();

        // Assert
        standardRoleClaims.Should().HaveCount(1);
        standardRoleClaims.Should().Contain(c => c.Value == "tracker-admin");
        standardRoleClaims.Should().NotContain(c => c.Value == "tracker-read");
    }
}
