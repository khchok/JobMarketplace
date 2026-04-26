using JobMarketplace.Jobs.Application.Interfaces;
using JobMarketplace.Jobs.Domain.Repositories;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.PublishJob;

public sealed class PublishJobCommandHandler(
    IJobRepository repository,
    IJobsUnitOfWork unitOfWork)
    : IRequestHandler<PublishJobCommand, Result>
{
    public async Task<Result> Handle(PublishJobCommand request, CancellationToken cancellationToken)
    {
        var job = await repository.GetByIdAsync(JobId.From(request.JobId), cancellationToken);
        if (job is null)
            return Result.Failure(Error.NotFound($"Job {request.JobId} not found."));

        var result = job.Publish(request.RequestingUserId);
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}