
using JobMarketplace.Identity.Application;
using JobMarketplace.Identity.Domain.Interfaces;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.Identity.Infrastructure.Persistence;
using JobMarketplace.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobMarketplace.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityApplication();

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddScoped<IIdentityUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}