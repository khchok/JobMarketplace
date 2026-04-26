using JobMarketplace.Applications.Domain.Enums;

namespace JobMarketplace.Applications.Application.DTOs;

public sealed record ApplicationDetailDto(
    Guid Id,
    Guid JobId,
    Guid CandidateId,
    string CoverLetter,
    ApplicationStatus Status,
    DateTime SubmittedAt,
    DateTime UpdatedAt);