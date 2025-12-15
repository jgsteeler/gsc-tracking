using Npgsql;

namespace GscTracking.Api.Utils;

/// <summary>
/// Helper class for building and parsing database connection strings
/// </summary>
public static class ConnectionStringHelper
{
    /// <summary>
    /// Parses a PostgreSQL connection URL and builds a standard Npgsql connection string
    /// </summary>
    /// <param name="connectionUrl">The PostgreSQL URL (e.g., postgresql://user:pass@host:port/dbname)</param>
    /// <returns>A properly formatted Npgsql connection string</returns>
    /// <exception cref="ArgumentNullException">Thrown when connectionUrl is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when the URL cannot be parsed</exception>
    public static string BuildNpgsqlConnectionString(string connectionUrl)
    {
        if (string.IsNullOrWhiteSpace(connectionUrl))
        {
            throw new ArgumentNullException(nameof(connectionUrl), "Connection URL cannot be null or empty");
        }

        try
        {
            var databaseUri = new Uri(connectionUrl);
            // Split userInfo on first colon to separate username and password
            // Remaining colons after the first one are part of the password
            var userInfo = databaseUri.UserInfo.Split(':', 2);

            if (userInfo.Length < 2 || string.IsNullOrWhiteSpace(userInfo[0]) || string.IsNullOrWhiteSpace(userInfo[1]))
            {
                throw new InvalidOperationException("Connection URL must include both username and password in the format: postgresql://username:password@host/database");
            }

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require, // Enforce SSL for security
            };

            return builder.ToString();
        }
        catch (UriFormatException ex)
        {
            throw new InvalidOperationException($"Invalid connection URL format. Expected format: postgresql://username:password@host:port/database. Error: {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Could not parse the database URL. Please check the format. Error: {ex.Message}", ex);
        }
    }
}
