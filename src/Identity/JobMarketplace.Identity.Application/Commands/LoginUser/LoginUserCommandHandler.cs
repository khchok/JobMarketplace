using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Application.Interfaces;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.LoginUser;

public sealed class LoginUserCommandHandler(
    IUserProfileRepository repository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginUserCommand, Result<LoginResultDto>>
{
    public async Task<Result<LoginResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByEmailAsync(request.Email, cancellationToken);
        if (profile is null || !passwordHasher.Verify(request.Password, profile.PasswordHash))
            return Result<LoginResultDto>.Failure(Error.Validation("Invalid email or password."));

        return Result<LoginResultDto>.Success(
            new LoginResultDto(profile.Id.Value, profile.Email.Value, profile.FullName, profile.Role));
    }
}
