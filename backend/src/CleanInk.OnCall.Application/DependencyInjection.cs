using CleanInk.OnCall.Application.Billing.Validators;
using CleanInk.OnCall.Application.CallCenter.Validators;
using CleanInk.OnCall.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanInk.OnCall.Application;

/// <summary>
/// Extension methods to register the Application layer services into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application-layer services: MediatR handlers, FluentValidation validators,
    /// and the validation pipeline behavior.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
