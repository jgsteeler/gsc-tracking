using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using GscTracking.Api.Data;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;
using GscTracking.Api.Utils;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file in development
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// Configure Auth0 settings (used by both Swagger and Authentication)
var auth0Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN") ?? builder.Configuration["Auth0:Domain"];
var auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE") ?? builder.Configuration["Auth0:Audience"];

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

    // Configure OAuth2 for Auth0
    if (!string.IsNullOrEmpty(auth0Domain))
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://{auth0Domain}/authorize"),
                    TokenUrl = new Uri($"https://{auth0Domain}/oauth/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "OpenID" },
                        { "profile", "Profile" },
                        { "email", "Email" }
                    }
                }
            }
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new[] { "openid", "profile", "email" }
            }
        });
    }

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
builder.Services.AddScoped<IExpenseService, ExpenseService>();

// Add CORS policy with pattern matching for Netlify deploy previews
// Compile regex once for performance
var netlifyPreviewRegex = new Regex(
    @"^https:\/\/deploy-preview-\d+--gsc-tracking-ui\.netlify\.app$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase
);

// Parse allowed ports once
var allowedLocalPorts = builder.Configuration["AllowedLocalPorts"]?.Split(',') ?? new[] { "5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
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

// Configure Auth0 Authentication
// Only configure Auth0 if domain and audience are provided
if (string.IsNullOrEmpty(auth0Domain) || string.IsNullOrEmpty(auth0Audience))
{
    throw new InvalidOperationException(
        "Auth0 configuration is required for application security. " +
        "Please set AUTH0_DOMAIN and AUTH0_AUDIENCE environment variables or configure 'Auth0:Domain' and 'Auth0:Audience' in appsettings.json. " +
        "See docs/AUTH0-SETUP.md for configuration instructions."
    );
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"https://{auth0Domain}/";
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://{auth0Domain}/",
        ValidateAudience = true,
        ValidAudience = auth0Audience,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("tracker-admin"));
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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Hello World endpoint for GSC Tracking API (no authentication required)
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
.WithTags("System")
.AllowAnonymous();

app.Run();
