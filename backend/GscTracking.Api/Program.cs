using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using GscTracking.Api.Data;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GSC Tracking API",
        Description = "API for GSC Small Engine Repair business management application",
        Contact = new OpenApiContact
        {
            Name = "GSC Development Team",
            Url = new Uri("https://github.com/jgsteeler/gsc-tracking")
        }
    });

    // Include XML comments for documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add controllers
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerRequestDtoValidator>();

// Add DbContext with support for multiple database providers
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Helper function to parse PostgreSQL URL and build a standard connection string
static string BuildNpgsqlConnectionString(string connectionUrl)
{
    try
    {
        var databaseUri = new Uri(connectionUrl);
        var userInfo = databaseUri.UserInfo.Split(':');

        // Validate that userInfo contains both username and password
        if (userInfo.Length < 2)
        {
            throw new InvalidOperationException(
                "Invalid database URL format. Expected format: postgresql://username:password@host:port/database");
        }
        
        if (string.IsNullOrEmpty(userInfo[0]) || string.IsNullOrEmpty(userInfo[1]))
        {
            throw new InvalidOperationException(
                "Database URL must contain both username and password. Expected format: postgresql://username:password@host:port/database");
        }

        var builder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Require, // Enforce SSL for security
            TrustServerCertificate = true, // Trust the server certificate (common for cloud providers)
        };

        return builder.ToString();
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Could not parse the database URL. Please check the format. Error: {ex.Message}", ex);
    }
}

// Determine database provider based on connection string format
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "No database connection string found. Please set DATABASE_URL environment variable or DefaultConnection in appsettings.json");
}

if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    // SQLite for local development
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
         connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
{
    // Parse the URL for PostgreSQL
    var npgsqlConnectionString = BuildNpgsqlConnectionString(connectionString);
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(npgsqlConnectionString, npgsqlOptions =>
        {
            // Enable retry on failure for transient errors
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
            
            // Set command timeout (30 seconds)
            npgsqlOptions.CommandTimeout(30);
        });
        
        // Enable detailed errors in development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });
}
else if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
{
    // Standard PostgreSQL connection string format
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // Enable retry on failure for transient errors
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
            
            // Set command timeout (30 seconds)
            npgsqlOptions.CommandTimeout(30);
        });
        
        // Enable detailed errors in development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });
}
else
{
    throw new InvalidOperationException(
        $"Unsupported database connection string format. Expected SQLite (Data Source=...) or PostgreSQL (postgresql://... or Host=...)");
}

// Add services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IJobService, JobService>();

// Add CORS policy for frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite default port (HTTP)
                "https://localhost:5173"  // Vite with HTTPS
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in Development and Staging environments (not Production)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GSC Tracking API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Hello World endpoint for GSC Tracking API
app.MapGet("/api/hello", () => new { 
    message = "Hello from GSC Tracking API!", 
    version = "1.0.0",
    timestamp = DateTime.UtcNow 
})
.WithName("GetHello");

app.Run();
