using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.Applications.Domain.Services;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;

public interface IApplicationListReadRepository
{
    Task<PagedList<ApplicationSummaryDto>> ListForJobAsync(
        Guid jobId, int page, int pageSize, CancellationToken ct = default);

    Task<PagedList<ApplicationSummaryDto>> ListForCandidateAsync(
        Guid candidateId, int page, int pageSize, CancellationToken ct = default);
}

public sealed class ListApplicationsForJobQueryHandler(
    IApplicationListReadRepository readRepository,
    IJobOwnershipChecker ownershipChecker)
    : IRequestHandler<ListApplicationsForJobQuery, Result<PagedList<ApplicationSummaryDto>>>
{
    public async Task<Result<PagedList<ApplicationSummaryDto>>> Handle(
        ListApplicationsForJobQuery request, CancellationToken cancellationToken)
    {
        var isOwner = await ownershipChecker.IsOwnedByEmployerAsync(
            JobId.From(request.JobId), request.RequestingEmployerId, cancellationToken);

        if (!isOwner)
            return Result<PagedList<ApplicationSummaryDto>>.Failure(
                Error.Unauthorized("Only the job's employer can list its applications."));

        var result = await readRepository.ListForJobAsync(
            request.JobId, request.Page, request.PageSize, cancellationToken);

        return Result<PagedList<ApplicationSummaryDto>>.Success(result);
    }
}