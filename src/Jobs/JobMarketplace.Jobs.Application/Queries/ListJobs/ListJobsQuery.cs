using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Queries.ListJobs;

public sealed record ListJobsQuery(
    string? Keyword,
    string? Country,
    string? City,
    decimal? SalaryMin,
    JobStatus Status = JobStatus.Published,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedList<JobSummaryDto>>>;