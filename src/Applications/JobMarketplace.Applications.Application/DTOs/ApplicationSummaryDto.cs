using JobMarketplace.Applications.Domain.Enums;

namespace JobMarketplace.Applications.Application.DTOs;

public sealed record ApplicationSummaryDto(
    Guid Id,
    Guid JobId,
    Guid CandidateId,
    string JobTitle,
    string JobCity,
    string JobCountry,
    ApplicationStatus Status,
    DateTime SubmittedAt);