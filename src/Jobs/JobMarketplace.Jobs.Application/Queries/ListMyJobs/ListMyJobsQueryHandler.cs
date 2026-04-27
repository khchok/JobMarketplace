using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Queries.ListMyJobs;

public sealed record ListMyJobsQuery(
    UserId EmployerId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedList<MyJobSummaryDto>>>;

public interface IMyJobListReadRepository
{
    Task<PagedList<MyJobSummaryDto>> ListByEmployerAsync(
        UserId employerId, int page, int pageSize, CancellationToken ct = default);
}

public sealed class ListMyJobsQueryHandler(IMyJobListReadRepository readRepository)
    : IRequestHandler<ListMyJobsQuery, Result<PagedList<MyJobSummaryDto>>>
{
    public async Task<Result<PagedList<MyJobSummaryDto>>> Handle(
        ListMyJobsQuery request, CancellationToken cancellationToken)
    {
        var result = await readRepository.ListByEmployerAsync(
            request.EmployerId, request.Page, request.PageSize, cancellationToken);

        return Result<PagedList<MyJobSummaryDto>>.Success(result);
    }
}
