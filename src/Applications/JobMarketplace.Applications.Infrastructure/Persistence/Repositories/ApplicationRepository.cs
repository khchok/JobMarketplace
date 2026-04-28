using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.Applications.Application.Queries.GetApplication;
using JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;
using JobMarketplace.Applications.Domain.Aggregates;
using JobMarketplace.Applications.Domain.Enums;
using JobMarketplace.Applications.Domain.Repositories;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;
using ApplicationId = JobMarketplace.SharedKernel.Ids.ApplicationId;

namespace JobMarketplace.Applications.Infrastructure.Persistence.Repositories;

public sealed class ApplicationRepository(ApplicationsDbContext dbContext)
    : IApplicationRepository, IApplicationReadRepository, IApplicationListReadRepository
{
    public async Task<Domain.Aggregates.Application?> GetByIdAsync(ApplicationId id, CancellationToken ct = default) =>
        await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<bool> ExistsAsync(JobId jobId, UserId candidateId, CancellationToken ct = default) =>
        await dbContext.Applications.AnyAsync(
            a => a.JobId == jobId && a.CandidateId == candidateId, ct);

    public void Add(Domain.Aggregates.Application application) => dbContext.Applications.Add(application);

    public async Task<ApplicationDetailDto?> GetDetailByIdAsync(Guid applicationId, CancellationToken ct = default) =>
        await dbContext.Applications
            .Where(a => a.Id == ApplicationId.From(applicationId))
            .Select(a => new ApplicationDetailDto(
                a.Id.Value,
                a.JobId.Value,
                a.CandidateId.Value,
                a.CoverLetter.Value,
                a.Status,
                a.SubmittedAt,
                a.UpdatedAt))
            .FirstOrDefaultAsync(ct);

    public async Task<PagedList<ApplicationSummaryDto>> ListForJobAsync(
        Guid jobId, int page, int pageSize, CancellationToken ct = default)
    {
        var total = await dbContext.Applications
            .Where(a => a.JobId == JobId.From(jobId))
            .CountAsync(ct);

        var offset = (page - 1) * pageSize;
        var rows = await dbContext.Database
            .SqlQuery<ApplicationJobRow>($"""
                SELECT a.id          AS Id,
                       a.job_id      AS JobId,
                       a.candidate_id AS CandidateId,
                       j.title       AS JobTitle,
                       j.city        AS JobCity,
                       j.country     AS JobCountry,
                       a.status      AS Status,
                       a.submitted_at AS SubmittedAt
                FROM applications.applications a
                INNER JOIN jobs.jobs j ON j.id = a.job_id
                WHERE a.job_id = {jobId}
                ORDER BY a.submitted_at DESC
                OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY
                """)
            .ToListAsync(ct);

        var items = rows.Select(r => r.ToDto()).ToList();
        return new PagedList<ApplicationSummaryDto>(items, page, pageSize, total);
    }

    public async Task<PagedList<ApplicationSummaryDto>> ListForCandidateAsync(
        Guid candidateId, int page, int pageSize, CancellationToken ct = default)
    {
        var total = await dbContext.Applications
            .Where(a => a.CandidateId == UserId.From(candidateId))
            .CountAsync(ct);

        var offset = (page - 1) * pageSize;
        var rows = await dbContext.Database
            .SqlQuery<ApplicationJobRow>($"""
                SELECT a.id           AS Id,
                       a.job_id       AS JobId,
                       a.candidate_id AS CandidateId,
                       j.title        AS JobTitle,
                       j.city         AS JobCity,
                       j.country      AS JobCountry,
                       a.status       AS Status,
                       a.submitted_at AS SubmittedAt
                FROM applications.applications a
                INNER JOIN jobs.jobs j ON j.id = a.job_id
                WHERE a.candidate_id = {candidateId}
                ORDER BY a.submitted_at DESC
                OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY
                """)
            .ToListAsync(ct);

        var items = rows.Select(r => r.ToDto()).ToList();
        return new PagedList<ApplicationSummaryDto>(items, page, pageSize, total);
    }

    private sealed class ApplicationJobRow
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid CandidateId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobCity { get; set; } = string.Empty;
        public string JobCountry { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }

        public ApplicationSummaryDto ToDto() => new(
            Id, JobId, CandidateId,
            JobTitle, JobCity, JobCountry,
            Enum.Parse<ApplicationStatus>(Status),
            SubmittedAt);
    }
}
