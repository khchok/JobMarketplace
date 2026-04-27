using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Api.Auth;

public sealed class CurrentUserService : ICurrentUserService
{
    public UserId UserId { get; init; }
    public UserRole Role { get; init; }
}
