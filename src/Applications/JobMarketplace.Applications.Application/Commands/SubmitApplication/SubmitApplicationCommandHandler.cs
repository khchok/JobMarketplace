using JobMarketplace.Applications.Application.Interfaces;
using JobMarketplace.Applications.Domain.Repositories;
using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.SubmitApplication;

public sealed class SubmitApplicationCommandHandler(
    IApplicationRepository repository,
    IJobExistenceChecker jobExistenceChecker,
    IApplicationsUnitOfWork unitOfWork)
    : IRequestHandler<SubmitApplicationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var jobId = JobId.From(request.JobId);

        var isPublished = await jobExistenceChecker.IsJobPublishedAsync(jobId, cancellationToken);
        if (!isPublished)
            return Result<Guid>.Failure(Error.Conflict("The job is not currently accepting applications."));

        var alreadyApplied = await repository.ExistsAsync(jobId, request.CandidateId, cancellationToken);
        if (alreadyApplied)
            return Result<Guid>.Failure(Error.Conflict("You have already applied to this job."));

        var result = Domain.Aggregates.Application.Create(jobId, request.CandidateId, request.CoverLetter);
        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        repository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value.Id.Value);
    }
}