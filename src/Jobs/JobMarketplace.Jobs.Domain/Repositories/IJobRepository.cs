using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Jobs.Domain.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(JobId id, CancellationToken ct = default);
    void Add(Job job);
}