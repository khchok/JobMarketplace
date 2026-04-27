using JobMarketplace.Jobs.Application;
using JobMarketplace.Jobs.Application.Interfaces;
using JobMarketplace.Jobs.Application.Queries.GetJob;
using JobMarketplace.Jobs.Application.Queries.ListJobs;
using JobMarketplace.Jobs.Domain.Repositories;
using JobMarketplace.Jobs.Infrastructure.Persistence;
using JobMarketplace.Jobs.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobMarketplace.Jobs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJobsApplication();

        services.AddDbContext<JobsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Database")));

        services.AddScoped<IJobsUnitOfWork>(sp => sp.GetRequiredService<JobsDbContext>());
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobReadRepository, JobRepository>();
        services.AddScoped<IJobListReadRepository, JobRepository>();

        return services;
    }
}