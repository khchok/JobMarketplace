using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.ListMyApplications;

public sealed record ListMyApplicationsQuery(
    UserId CandidateId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedList<ApplicationSummaryDto>>>;