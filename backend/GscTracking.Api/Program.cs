using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using GscTracking.Api.Data;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;
using GscTracking.Api.Utils;

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

// Determine database provider based on connection string format
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No database connection string found. Set the DATABASE_URL environment variable or configure 'DefaultConnection' in appsettings.json.");
}

// Determine database provider based on connection string
if (connectionString.StartsWith("Data Source="))
{
    // SQLite for local development
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else // Assume PostgreSQL for all other cases
{
    var npgsqlConnectionString = connectionString;
    if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
        connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
    {
        // Parse the URL for PostgreSQL
        npgsqlConnectionString = ConnectionStringHelper.BuildNpgsqlConnectionString(connectionString);
    }
    
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
