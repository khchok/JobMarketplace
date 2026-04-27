using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FullName,
    UserRole Role) : IRequest<Result<UserProfileDto>>;
