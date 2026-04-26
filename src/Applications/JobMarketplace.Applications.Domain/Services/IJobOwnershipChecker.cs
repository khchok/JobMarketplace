using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Applications.Domain.Services;

public interface IJobOwnershipChecker
{
    Task<bool> IsOwnedByEmployerAsync(JobId jobId, UserId employerId, CancellationToken ct = default);
}