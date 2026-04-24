using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Identity.Domain.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<UserProfile?> GetBySupabaseUserIdAsync(string supabaseUserId, CancellationToken ct = default);
    void Add(UserProfile userProfile);
}