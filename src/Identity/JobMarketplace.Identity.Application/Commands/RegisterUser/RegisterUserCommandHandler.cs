using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Application.Interfaces;
using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Interfaces;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserProfileRepository repository,
    IIdentityUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<UserProfileDto>.Failure(
                Error.Conflict($"An account with email '{request.Email}' already exists."));

        var passwordHash = passwordHasher.Hash(request.Password);

        var profileResult = UserProfile.Create(request.Email, request.FullName, request.Role, passwordHash);
        if (profileResult.IsFailure)
            return Result<UserProfileDto>.Failure(profileResult.Error);

        repository.Add(profileResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var profile = profileResult.Value;
        return Result<UserProfileDto>.Success(
            new UserProfileDto(profile.Id.Value, profile.Email.Value, profile.FullName, profile.Role));
    }
}
