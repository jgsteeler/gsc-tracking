using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GscTracking.Core.Interfaces;
using GscTracking.Infrastructure.Data;
using GscTracking.Infrastructure.Repositories;

namespace GscTracking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        // Note: Connection string configuration is handled in the API layer
        // This method assumes DbContext is already registered
        
        // Register Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IJobUpdateRepository, JobUpdateRepository>();
        
        return services;
    }
}
