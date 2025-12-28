using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using GscTracking.Infrastructure.Data;
using GscTracking.Api.Services;
using GscTracking.Api.Validators;
using GscTracking.Api.Utils;
using GscTracking.Api.Extensions;
using GscTracking.Application;
using GscTracking.Infrastructure;
using DotNetEnv;
using System.Security.Claims;

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

// Add Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, npgsqlConnectionString, builder.Environment.IsDevelopment());

// Legacy services are being phased out - using CQRS pattern instead
// Keeping CSV service for now until migrated
builder.Services.AddScoped<ICsvService, CsvService>();

// CORS is configured solely via CORS_ALLOWED_ORIGINS (comma-separated)
builder.Services.AddGscCors();

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
    
    // Transform claims to map Auth0 custom permissions/roles to standard role claims
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // Check for permissions claim (primary method)
                var possiblePermissionClaims = new[]
                {
                    "https://gsc-tracking.com/permissions",
                    "http://gsc-tracking.com/permissions",
                    "permissions"
                };

                bool foundPermissions = false;
                foreach (var permissionClaimType in possiblePermissionClaims)
                {
                    var permissionClaims = claimsIdentity.FindAll(permissionClaimType).ToList();
                    if (permissionClaims.Any())
                    {
                        // Map permissions to tracker-* role format
                        foreach (var permissionClaim in permissionClaims)
                        {
                            var mappedRole = permissionClaim.Value switch
                            {
                                "admin" => "tracker-admin",
                                "write" => "tracker-write",
                                "read" => "tracker-read",
                                _ => permissionClaim.Value // Keep as-is if not recognized
                            };
                            
                            // Add as standard role claim type for .NET authorization
                            claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                System.Security.Claims.ClaimTypes.Role, 
                                mappedRole));
                        }
                        foundPermissions = true;
                        break; // Found permissions, no need to check other claim types
                    }
                }

                // Fall back to checking for roles in various Auth0 claim formats
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
                            // Add each role as a standard role claim with mapping
                            foreach (var roleClaim in roleClaims)
                            {
                                // Map common role values to tracker-* format
                                var mappedRole = roleClaim.Value switch
                                {
                                    "admin" => "tracker-admin",
                                    "write" => "tracker-write",
                                    "read" => "tracker-read",
                                    _ => roleClaim.Value // Keep as-is if already in correct format or not recognized
                                };
                                
                                // Add as standard role claim type for .NET authorization
                                claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                    System.Security.Claims.ClaimTypes.Role, 
                                    mappedRole));
                            }
                            break; // Found roles, no need to check other claim types
                        }
                    }
                }

            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    // Admin role has full access to all functionality
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("tracker-admin"));
    
    // Write access for adding expenses and job updates (admin + write roles)
    options.AddPolicy("WriteAccess", policy =>
        policy.RequireRole("tracker-admin", "tracker-write"));
    
    // Read access for viewing data (admin + write + read roles)
    options.AddPolicy("ReadAccess", policy =>
        policy.RequireRole("tracker-admin", "tracker-write", "tracker-read"));
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

app.UseCors(CorsExtensions.PolicyName);
app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint for monitoring (no authentication required)
app.MapGet("/api/health", () => 
{
    return Results.Ok(new { status = "healthy" });
})
.WithName("GetHealth")
.WithTags("System")
.AllowAnonymous();

// Version endpoint for admins to check API version
app.MapGet("/api/version", () => 
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "unknown";
    var informationalVersion = assembly
        .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? version;
    
    return new { 
        message = "GSC Tracking API", 
        version = informationalVersion,
        assemblyVersion = version,
        timestamp = DateTime.UtcNow 
    };
})
.WithName("GetVersion")
.WithTags("System")
.RequireAuthorization("AdminOnly");

app.Run();
