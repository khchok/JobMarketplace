using JobMarketplace.Jobs.Domain.Enums;

namespace JobMarketplace.Jobs.Application.DTOs;

public sealed record MyJobSummaryDto(
    Guid Id,
    string Title,
    string City,
    string Country,
    decimal SalaryMin,
    decimal SalaryMax,
    string Currency,
    JobStatus Status,
    DateTime CreatedAt,
    DateTime? PublishedAt);
