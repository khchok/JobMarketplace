using JobMarketplace.Applications.Domain.Aggregates;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Applications.Domain.Repositories;

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(SharedKernel.Ids.ApplicationId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(JobId jobId, UserId candidateId, CancellationToken ct = default);
    void Add(Application application);
}