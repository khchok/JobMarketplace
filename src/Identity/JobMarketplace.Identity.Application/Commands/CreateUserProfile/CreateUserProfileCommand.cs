using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    string SupabaseUserId,
    string Email,
    string FullName,
    UserRole Role) : IRequest<Result<UserProfileDto>>;