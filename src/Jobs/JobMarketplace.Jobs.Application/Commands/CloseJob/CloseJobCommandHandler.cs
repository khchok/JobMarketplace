using JobMarketplace.Jobs.Application.Interfaces;
using JobMarketplace.Jobs.Domain.Repositories;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.CloseJob;

public sealed class CloseJobCommandHandler(
    IJobRepository repository,
    IJobsUnitOfWork unitOfWork)
    : IRequestHandler<CloseJobCommand, Result>
{
    public async Task<Result> Handle(CloseJobCommand request, CancellationToken cancellationToken)
    {
        var job = await repository.GetByIdAsync(JobId.From(request.JobId), cancellationToken);
        if (job is null)
            return Result.Failure(Error.NotFound($"Job {request.JobId} not found."));

        var result = job.Close(request.RequestingUserId);
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}