using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Api.Auth;

public interface ICurrentUserService
{
    UserId UserId { get; }
    UserRole Role { get; }
}