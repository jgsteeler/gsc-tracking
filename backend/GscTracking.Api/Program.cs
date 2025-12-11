using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using GscTracking.Api.Data;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
         connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
{
    // PostgreSQL for staging and production
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    throw new InvalidOperationException(
        $"Unsupported database connection string format. Expected SQLite (Data Source=...) or PostgreSQL (postgresql://... or Host=...)");
}

// Add services
builder.Services.AddScoped<ICustomerService, CustomerService>();

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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
