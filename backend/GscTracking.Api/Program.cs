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

// Determine database provider based on connection string
if (connectionString?.StartsWith("Data Source=") == true)
{
    // SQLite for local development
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    // PostgreSQL for staging/production
    var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
    // The following line can be removed if you don't need channel binding or if it's causing issues.
    // dataSourceBuilder.ConnectionStringBuilder.Add("channel_binding", "require");
    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(dataSource));
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
