using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserProfileRepository(IdentityDbContext dbContext) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByIdAsync(UserId id, CancellationToken ct = default) =>
        await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<UserProfile?> GetBySupabaseUserIdAsync(string supabaseUserId, CancellationToken ct = default) =>
        await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.SupabaseUserId == supabaseUserId, ct);

    public void Add(UserProfile userProfile) =>
        dbContext.UserProfiles.Add(userProfile);
}
