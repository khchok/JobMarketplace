using JobMarketplace.Jobs.Domain.Enums;

namespace JobMarketplace.Jobs.Application.DTOs;

public sealed record JobDetailDto(
    Guid Id,
    string Title,
    string Description,
    string City,
    string Country,
    decimal SalaryMin,
    decimal SalaryMax,
    string Currency,
    JobStatus Status,
    Guid EmployerId,
    DateTime CreatedAt,
    DateTime? PublishedAt);