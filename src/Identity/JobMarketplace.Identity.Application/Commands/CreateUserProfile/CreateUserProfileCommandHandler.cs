
using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Interfaces;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.CreateUserProfile;

public sealed class CreateUserProfileCommandHandler(
    IUserProfileRepository repository,
    IIdentityUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserProfileCommand, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetBySupabaseUserIdAsync(request.SupabaseUserId, cancellationToken);
        if (existing is not null)
            return Result<UserProfileDto>.Failure(
                Error.Conflict($"A profile already exists for user '{request.SupabaseUserId}'."));

        var profileResult = UserProfile.Create(
            request.SupabaseUserId,
            request.Email,
            request.FullName,
            request.Role);

        if (profileResult.IsFailure)
            return Result<UserProfileDto>.Failure(profileResult.Error);

        repository.Add(profileResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var profile = profileResult.Value;
        return Result<UserProfileDto>.Success(
            new UserProfileDto(profile.Id.Value, profile.Email.Value, profile.FullName, profile.Role));
    }
}