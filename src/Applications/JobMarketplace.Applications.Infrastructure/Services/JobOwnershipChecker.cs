using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.Applications.Infrastructure.Persistence;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Applications.Infrastructure.Services;

public sealed class JobOwnershipChecker(ApplicationsDbContext dbContext) : IJobOwnershipChecker
{
    public async Task<bool> IsOwnedByEmployerAsync(JobId jobId, UserId employerId, CancellationToken ct = default) =>
        await dbContext.JobReadModels
            .AnyAsync(j => j.Id == jobId.Value && j.EmployerId == employerId.Value, ct);
}