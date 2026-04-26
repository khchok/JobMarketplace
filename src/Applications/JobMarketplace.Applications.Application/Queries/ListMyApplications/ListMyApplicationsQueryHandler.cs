using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.ListMyApplications;

public sealed class ListMyApplicationsQueryHandler(IApplicationListReadRepository readRepository)
    : IRequestHandler<ListMyApplicationsQuery, Result<PagedList<ApplicationSummaryDto>>>
{
    public async Task<Result<PagedList<ApplicationSummaryDto>>> Handle(
        ListMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var result = await readRepository.ListForCandidateAsync(
            request.CandidateId.Value, request.Page, request.PageSize, cancellationToken);

        return Result<PagedList<ApplicationSummaryDto>>.Success(result);
    }
}