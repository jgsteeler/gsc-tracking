using FluentAssertions;
using GscTracking.Api.Utils;

namespace GscTracking.Api.Tests.Utils;

public class ConnectionStringHelperTests
{
    [Fact]
    public void BuildNpgsqlConnectionString_WithValidUrl_ReturnsCorrectConnectionString()
    {
        // Arrange
        var connectionUrl = "postgresql://testuser:testpass@localhost:5432/testdb";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Host=localhost");
        result.Should().Contain("Port=5432");
        result.Should().Contain("Username=testuser");
        result.Should().Contain("Password=testpass");
        result.Should().Contain("Database=testdb");
        result.Should().Contain("SSL Mode=Require");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithPostgresScheme_ReturnsCorrectConnectionString()
    {
        // Arrange
        var connectionUrl = "postgres://user123:pass456@myhost.com:5433/mydb";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Host=myhost.com");
        result.Should().Contain("Port=5433");
        result.Should().Contain("Username=user123");
        result.Should().Contain("Password=pass456");
        result.Should().Contain("Database=mydb");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithDefaultPort_UsesPort5432()
    {
        // Arrange
        var connectionUrl = "postgresql://user:pass@hostname/database";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Port=5432");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithSpecialCharactersInPassword_MustBeUrlEncoded()
    {
        // Arrange - Some special characters like @ must be URL-encoded
        // because they have special meaning in URLs
        var connectionUrl = "postgresql://admin:P%40ssw0rd!%23%24%25@server.com:5432/proddb";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Username=admin");
        result.Should().Contain("Password=P%40ssw0rd!%23%24%25");
        result.Should().Contain("Host=server.com");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithUrlEncodedPassword_PreservesEncoding()
    {
        // Arrange - password with URL-encoded special characters
        // Note: The Uri class does NOT automatically decode userInfo
        // Users must provide passwords in the format expected by their database
        var connectionUrl = "postgresql://user:pass%40word%23test@example.com:5432/db";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Username=user");
        // URL encoding is preserved as-is in the connection string
        result.Should().Contain("Password=pass%40word%23test");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithCloudProviderUrl_ParsesCorrectly()
    {
        // Arrange - simulating a Neon/Supabase/Heroku style URL
        var connectionUrl = "postgresql://neondb_owner:AbCdEf123456@ep-cool-cloud-123456.us-east-2.aws.neon.tech/neondb";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Host=ep-cool-cloud-123456.us-east-2.aws.neon.tech");
        result.Should().Contain("Username=neondb_owner");
        result.Should().Contain("Password=AbCdEf123456");
        result.Should().Contain("Database=neondb");
        result.Should().Contain("SSL Mode=Require");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithNullUrl_ThrowsArgumentNullException()
    {
        // Arrange
        string? connectionUrl = null;

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Connection URL cannot be null or empty*")
            .And.ParamName.Should().Be("connectionUrl");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithEmptyUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var connectionUrl = "";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Connection URL cannot be null or empty*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithWhitespaceUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var connectionUrl = "   ";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Connection URL cannot be null or empty*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithInvalidUrlFormat_ThrowsInvalidOperationException()
    {
        // Arrange
        var connectionUrl = "not-a-valid-url";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Invalid connection URL format*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithMissingPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var connectionUrl = "postgresql://username@localhost:5432/database";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Connection URL must include both username and password*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithMissingUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var connectionUrl = "postgresql://:password@localhost:5432/database";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Connection URL must include both username and password*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithEmptyPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var connectionUrl = "postgresql://username:@localhost:5432/database";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Connection URL must include both username and password*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithNoUserInfo_ThrowsInvalidOperationException()
    {
        // Arrange
        var connectionUrl = "postgresql://localhost:5432/database";

        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Connection URL must include both username and password*");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithMultipleColonsInPassword_ParsesCorrectly()
    {
        // Arrange - password contains multiple colons
        // The helper splits on first colon only (using Split(':', 2))
        // so remaining colons become part of the password
        var connectionUrl = "postgresql://user:pass:word:123@localhost:5432/db";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Username=user");
        result.Should().Contain("Password=pass:word:123");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithDatabaseNameContainingSpecialChars_ParsesCorrectly()
    {
        // Arrange
        var connectionUrl = "postgresql://user:pass@localhost:5432/my-database_123";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Database=my-database_123");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithIPv4Address_ParsesCorrectly()
    {
        // Arrange
        var connectionUrl = "postgresql://user:pass@192.168.1.100:5432/database";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Host=192.168.1.100");
        result.Should().Contain("Port=5432");
    }

    [Theory]
    [InlineData("postgresql://user:pass@localhost/db")]
    [InlineData("postgres://user:pass@localhost/db")]
    [InlineData("postgresql://user:pass@localhost:5432/db")]
    [InlineData("postgres://user:pass@localhost:5432/db")]
    public void BuildNpgsqlConnectionString_WithVariousValidFormats_Succeeds(string connectionUrl)
    {
        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().NotThrow();
        var result = act();
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Host=localhost");
        result.Should().Contain("Database=db");
    }

    [Theory]
    [InlineData("invalid-format")]
    [InlineData("postgresql://")]
    public void BuildNpgsqlConnectionString_WithInvalidSchemeOrFormat_ThrowsInvalidOperationException(string connectionUrl)
    {
        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData("http://user:pass@localhost/db")]
    [InlineData("mysql://user:pass@localhost/db")]
    public void BuildNpgsqlConnectionString_WithNonPostgresScheme_StillParsesSuccessfully(string connectionUrl)
    {
        // Note: The Uri class doesn't validate the scheme, so any valid URI format will work
        // This test documents that behavior, though users should still use postgresql:// or postgres://
        // Act
        var act = () => ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert - Should not throw, but may not work correctly with PostgreSQL
        act.Should().NotThrow();
    }

    [Fact]
    public void BuildNpgsqlConnectionString_AlwaysEnforcesSSL()
    {
        // Arrange
        var connectionUrl = "postgresql://user:pass@localhost:5432/db";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("SSL Mode=Require");
    }

    [Fact]
    public void BuildNpgsqlConnectionString_WithComplexRealWorldUrl_ParsesCorrectly()
    {
        // Arrange - Real-world example similar to cloud providers
        var connectionUrl = "postgresql://u7n2q3p8r1m4k5:x9y8z7w6v5u4t3s2r1@ec2-52-1-115-6.compute-1.amazonaws.com:5432/d8j2k5l3m9n1";

        // Act
        var result = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionUrl);

        // Assert
        result.Should().Contain("Host=ec2-52-1-115-6.compute-1.amazonaws.com");
        result.Should().Contain("Port=5432");
        result.Should().Contain("Username=u7n2q3p8r1m4k5");
        result.Should().Contain("Password=x9y8z7w6v5u4t3s2r1");
        result.Should().Contain("Database=d8j2k5l3m9n1");
    }
}
