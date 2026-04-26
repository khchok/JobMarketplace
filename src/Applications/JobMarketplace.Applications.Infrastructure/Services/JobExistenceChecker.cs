using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.Applications.Infrastructure.Persistence;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Applications.Infrastructure.Services;

public sealed class JobExistenceChecker(ApplicationsDbContext dbContext) : IJobExistenceChecker
{
    public async Task<bool> IsJobPublishedAsync(JobId jobId, CancellationToken ct = default) =>
        await dbContext.JobReadModels
            .AnyAsync(j => j.Id == jobId.Value && j.Status == "Published", ct);
}