using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace JobMarketplace.Applications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}