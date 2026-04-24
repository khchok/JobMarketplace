using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Interfaces;
using JobMarketplace.Identity.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler(IUserProfileRepository repository, IIdentityUnitOfWork unitOfWork) : IRequestHandler<UpdateUserProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (userProfile is null)
        {
            return Result.Failure(Error.NotFound("User profile not found."));
        }

        var updateResult = userProfile.UpdateFullName(request.FullName);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}