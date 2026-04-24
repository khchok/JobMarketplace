using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    UserId UserId,
    string FullName,
    string? Role)
    : IRequest<Result>;