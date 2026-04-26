using JobMarketplace.Applications.Domain.Enums;

namespace JobMarketplace.Applications.Application.DTOs;

public sealed record ApplicationSummaryDto(
    Guid Id,
    Guid CandidateId,
    ApplicationStatus Status,
    DateTime SubmittedAt);