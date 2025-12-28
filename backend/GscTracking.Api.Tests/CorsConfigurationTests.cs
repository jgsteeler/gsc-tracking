using FluentAssertions;
using GscTracking.Api.Extensions;

namespace GscTracking.Api.Tests;

public class CorsConfigurationTests
{
    [Fact]
    public void CORS_ALLOWED_ORIGINS_ThrowsWhenMissing()
    {
        Action act = () => CorsExtensions.ParseAllowedOrigins(null);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*CORS_ALLOWED_ORIGINS is required*");
    }

    [Fact]
    public void CORS_ALLOWED_ORIGINS_ParsesCommaSeparatedValuesAndTrims()
    {
        var raw = "http://localhost:5173, https://example.com ,http://localhost:3000";

        var origins = CorsExtensions.ParseAllowedOrigins(raw);

        origins.Should().BeEquivalentTo(new[]
        {
            "http://localhost:5173",
            "https://example.com",
            "http://localhost:3000"
        });
    }

    [Fact]
    public void CORS_ALLOWED_ORIGINS_NormalizesToOriginFormat()
    {
        var raw = "https://example.com/,https://example.com/some/path,http://localhost:5173/";

        var origins = CorsExtensions.ParseAllowedOrigins(raw);

        origins.Should().BeEquivalentTo(new[]
        {
            "https://example.com",
            "http://localhost:5173"
        });
    }

    [Fact]
    public void CORS_ALLOWED_ORIGINS_DeduplicatesCaseInsensitive()
    {
        var raw = "https://Example.com,https://example.com,HTTPS://EXAMPLE.COM";

        var origins = CorsExtensions.ParseAllowedOrigins(raw);

        origins.Should().BeEquivalentTo(new[] { "https://example.com" });
    }

    [Fact]
    public void CORS_ALLOWED_ORIGINS_ThrowsOnInvalidEntries()
    {
        var raw = "https://example.com,not-a-url,ftp://localhost:5173";

        Action act = () => CorsExtensions.ParseAllowedOrigins(raw);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*contains invalid origin values*");
    }

    [Theory]
    [InlineData("http://localhost:5173", true)]
    [InlineData("https://app.example.com", true)]
    [InlineData("https://app.example.com/", false)]
    [InlineData("https://evil.example.com", false)]
    public void OriginCheck_UsesExactOriginMatches(string origin, bool expectedResult)
    {
        var allowed = CorsExtensions.ParseAllowedOrigins("http://localhost:5173,https://app.example.com");
        var allowedSet = allowed.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var isAllowed = allowedSet.Contains(origin);

        isAllowed.Should().Be(expectedResult);
    }
}
