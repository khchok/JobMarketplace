using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;

public sealed record ListApplicationsForJobQuery(
    Guid JobId,
    UserId RequestingEmployerId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedList<ApplicationSummaryDto>>>;