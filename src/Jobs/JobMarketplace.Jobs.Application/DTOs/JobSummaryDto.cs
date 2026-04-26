namespace JobMarketplace.Jobs.Application.DTOs;

public sealed record JobSummaryDto(
    Guid Id,
    string Title,
    string City,
    string Country,
    decimal SalaryMin,
    decimal SalaryMax,
    string Currency,
    DateTime? PublishedAt);