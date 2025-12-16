using System.Reflection;
using System.Text.RegularExpressions;
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

// Load .env file in development
if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
    if (File.Exists(envPath))
    {
        foreach (var line in File.ReadAllLines(envPath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                // Trim quotes from the value
                var value = parts[1].Trim().Trim('"');
                Environment.SetEnvironmentVariable(parts[0], value);
            }
        }
    }
}

// Add controllers
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerRequestDtoValidator>();

// Add DbContext with PostgreSQL support for all environments
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No database connection string found. Set the DATABASE_URL environment variable or configure 'DefaultConnection' in appsettings.json.");
}

// Parse the URL for PostgreSQL if needed
var npgsqlConnectionString = connectionString;
if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
    connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
{
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

// Add services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobUpdateService, JobUpdateService>();

// Add CORS policy with pattern matching for Netlify deploy previews
// Compile regex once for performance
var netlifyPreviewRegex = new Regex(
    @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedLocalPorts = builder.Configuration["AllowedLocalPorts"]?.Split(',') ?? new[] { "5173" };
        
        policy.SetIsOriginAllowed(origin =>
        {
            // Allow configured localhost ports
            foreach (var port in allowedLocalPorts)
            {
                if (origin == $"http://localhost:{port.Trim()}")
                    return true;
            }
            
            // Allow production
            if (origin == "https://gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow staging
            if (origin == "https://staging--gsc-tracking-ui.netlify.app")
                return true;
            
            // Allow Netlify deploy previews with pattern matching
            if (netlifyPreviewRegex.IsMatch(origin))
                return true;
            
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Run database migrations on startup (for all environments)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        throw; // Re-throw to prevent app from starting with failed migration
    }
}

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
app.MapGet("/api/hello", () => 
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "unknown";
    var informationalVersion = assembly
        .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? version;
    
    return new { 
        message = "Hello from GSC Tracking API!", 
        version = informationalVersion,
        assemblyVersion = version,
        timestamp = DateTime.UtcNow 
    };
})
.WithName("GetHello")
.WithTags("System");

app.Run();
