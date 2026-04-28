using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.Applications.Application.Queries.GetApplication;
using JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;
using JobMarketplace.Applications.Domain.Aggregates;
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
        var query = dbContext.Applications
            .Where(a => a.JobId == JobId.From(jobId))
            .Select(a => new { AppId = a.Id.Value, JobId = a.JobId.Value, CandidateId = a.CandidateId.Value, a.Status, a.SubmittedAt });

        var total = await query.CountAsync(ct);
        var items = await query
            .Join(dbContext.JobReadModels,
                a => a.JobId,
                j => j.Id,
                (a, j) => new ApplicationSummaryDto(
                    a.AppId, a.JobId, a.CandidateId,
                    j.Title, j.City, j.Country,
                    a.Status, a.SubmittedAt))
            .OrderByDescending(a => a.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<ApplicationSummaryDto>(items, page, pageSize, total);
    }

    public async Task<PagedList<ApplicationSummaryDto>> ListForCandidateAsync(
        Guid candidateId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = dbContext.Applications
            .Where(a => a.CandidateId == UserId.From(candidateId))
            .Select(a => new { AppId = a.Id.Value, JobId = a.JobId.Value, CandidateId = a.CandidateId.Value, a.Status, a.SubmittedAt });

        var total = await query.CountAsync(ct);
        var items = await query
            .Join(dbContext.JobReadModels,
                a => a.JobId,
                j => j.Id,
                (a, j) => new ApplicationSummaryDto(
                    a.AppId, a.JobId, a.CandidateId,
                    j.Title, j.City, j.Country,
                    a.Status, a.SubmittedAt))
            .OrderByDescending(a => a.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<ApplicationSummaryDto>(items, page, pageSize, total);
    }
}