using JobMarketplace.SharedKernel.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace JobMarketplace.SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelPipeline(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        return services;
    }
}