
using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Queries.GetUserProfile;

public sealed class GetUserProfileQueryHandler(IUserProfileRepository repository)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result<UserProfileDto>.Failure(
                Error.NotFound($"UserProfile {request.UserId} not found."));

        return Result<UserProfileDto>.Success(
            new UserProfileDto(profile.Id.Value, profile.Email.Value, profile.FullName, profile.Role));
    }
}