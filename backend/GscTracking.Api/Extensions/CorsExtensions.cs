using Microsoft.Extensions.DependencyInjection;

namespace GscTracking.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "AllowFrontend";

    public static IServiceCollection AddGscCors(this IServiceCollection services)
    {
        var rawAllowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
        var allowedOrigins = ParseAllowedOrigins(rawAllowedOrigins);

        // Use SetIsOriginAllowed so we can keep AllowCredentials with an explicit allow-list.
        var allowedOriginSet = allowedOrigins.ToHashSet(StringComparer.OrdinalIgnoreCase);

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => allowedOriginSet.Contains(origin))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IReadOnlyCollection<string> ParseAllowedOrigins(string? rawAllowedOrigins)
    {
        if (string.IsNullOrWhiteSpace(rawAllowedOrigins))
        {
            throw new InvalidOperationException(
                "CORS_ALLOWED_ORIGINS is required. " +
                "Set it to a comma-separated list of allowed origins (e.g. 'http://localhost:5173,https://app.example.com')."
            );
        }

        var invalidEntries = new List<string>();
        var allowedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in rawAllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!Uri.TryCreate(entry, UriKind.Absolute, out var uri))
            {
                invalidEntries.Add(entry);
                continue;
            }

            if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                invalidEntries.Add(entry);
                continue;
            }

            // Normalize to the browser origin format (scheme + host + optional port), dropping paths/trailing slash.
            var origin = uri.GetLeftPart(UriPartial.Authority);
            if (string.IsNullOrWhiteSpace(origin))
            {
                invalidEntries.Add(entry);
                continue;
            }

            allowedOrigins.Add(origin);
        }

        if (invalidEntries.Count > 0)
        {
            throw new InvalidOperationException(
                "CORS_ALLOWED_ORIGINS contains invalid origin values: " + string.Join(", ", invalidEntries)
            );
        }

        if (allowedOrigins.Count == 0)
        {
            throw new InvalidOperationException(
                "CORS_ALLOWED_ORIGINS did not contain any valid http/https origins."
            );
        }

        return allowedOrigins.ToArray();
    }
}
