using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Applications.Domain.Services;

public interface IJobExistenceChecker
{
    Task<bool> IsJobPublishedAsync(JobId jobId, CancellationToken ct = default);
}