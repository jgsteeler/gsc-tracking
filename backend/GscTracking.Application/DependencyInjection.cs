using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace GscTracking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Register AutoMapper with configuration
        services.AddAutoMapper(cfg =>
        {
            // AutoMapper profiles will be automatically discovered from this assembly
        }, Assembly.GetExecutingAssembly());
        
        return services;
    }
}
