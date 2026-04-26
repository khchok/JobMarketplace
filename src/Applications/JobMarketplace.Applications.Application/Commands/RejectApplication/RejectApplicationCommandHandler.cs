using JobMarketplace.Applications.Application.Interfaces;
using JobMarketplace.Applications.Domain.Repositories;
using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.RejectApplication;

public sealed class RejectApplicationCommandHandler(
    IApplicationRepository repository,
    IJobOwnershipChecker ownershipChecker,
    IApplicationsUnitOfWork unitOfWork)
    : IRequestHandler<RejectApplicationCommand, Result>
{
    public async Task<Result> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await repository.GetByIdAsync(
            SharedKernel.Ids.ApplicationId.From(request.ApplicationId), cancellationToken);

        if (application is null)
            return Result.Failure(Error.NotFound($"Application {request.ApplicationId} not found."));

        var isOwner = await ownershipChecker.IsOwnedByEmployerAsync(
            application.JobId, request.RequestingUserId, cancellationToken);

        if (!isOwner)
            return Result.Failure(Error.Unauthorized("Only the job's employer can reject applications."));

        var result = application.Reject();
        if (result.IsFailure) return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}