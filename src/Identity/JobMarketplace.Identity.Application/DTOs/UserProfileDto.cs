using JobMarketplace.Identity.Domain.Enums;

namespace JobMarketplace.Identity.Application.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    UserRole Role);