using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Queries.ListJobs;

public interface IJobListReadRepository
{
    Task<PagedList<JobSummaryDto>> ListAsync(
        string? keyword, string? country, string? city,
        decimal? salaryMin, JobStatus status,
        int page, int pageSize,
        CancellationToken ct = default);
}

public sealed class ListJobsQueryHandler(IJobListReadRepository readRepository)
    : IRequestHandler<ListJobsQuery, Result<PagedList<JobSummaryDto>>>
{
    public async Task<Result<PagedList<JobSummaryDto>>> Handle(
        ListJobsQuery request, CancellationToken cancellationToken)
    {
        var result = await readRepository.ListAsync(
            request.Keyword, request.Country, request.City,
            request.SalaryMin, request.Status,
            request.Page, request.PageSize,
            cancellationToken);

        return Result<PagedList<JobSummaryDto>>.Success(result);
    }
}