using JobMarketplace.Applications.Application;
using JobMarketplace.Applications.Application.Interfaces;
using JobMarketplace.Applications.Application.Queries.GetApplication;
using JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;
using JobMarketplace.Applications.Domain.Repositories;
using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.Applications.Infrastructure.Persistence;
using JobMarketplace.Applications.Infrastructure.Persistence.Repositories;
using JobMarketplace.Applications.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobMarketplace.Applications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationsApplication();

        services.AddDbContext<ApplicationsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Database")));

        services.AddScoped<IApplicationsUnitOfWork>(sp => sp.GetRequiredService<ApplicationsDbContext>());
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationReadRepository, ApplicationRepository>();
        services.AddScoped<IApplicationListReadRepository, ApplicationRepository>();
        services.AddScoped<IJobExistenceChecker, JobExistenceChecker>();
        services.AddScoped<IJobOwnershipChecker, JobOwnershipChecker>();

        return services;
    }
}