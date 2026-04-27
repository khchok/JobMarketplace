using JobMarketplace.Identity.Domain.Enums;

namespace JobMarketplace.Identity.Application.DTOs;

public sealed record LoginResultDto(
    Guid UserId,
    string Email,
    string FullName,
    UserRole Role);
