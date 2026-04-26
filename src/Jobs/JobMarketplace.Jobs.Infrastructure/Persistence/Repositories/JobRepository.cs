using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.Jobs.Application.Queries.GetJob;
using JobMarketplace.Jobs.Application.Queries.ListJobs;
using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.Jobs.Domain.Repositories;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Jobs.Infrastructure.Persistence.Repositories;

public sealed class JobRepository(JobsDbContext dbContext)
    : IJobRepository, IJobReadRepository, IJobListReadRepository
{
    public async Task<Job?> GetByIdAsync(JobId id, CancellationToken ct = default) =>
        await dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id, ct);

    public void Add(Job job) => dbContext.Jobs.Add(job);

    public async Task<JobDetailDto?> GetDetailByIdAsync(Guid jobId, CancellationToken ct = default) =>
        await dbContext.Jobs
            .Where(j => j.Id == JobId.From(jobId))
            .Select(j => new JobDetailDto(
                j.Id.Value,
                j.Title.Value,
                j.Description.Value,
                j.Location.City,
                j.Location.Country,
                j.SalaryRange.Min,
                j.SalaryRange.Max,
                j.SalaryRange.Currency,
                j.Status,
                j.EmployerId.Value,
                j.CreatedAt,
                j.PublishedAt))
            .FirstOrDefaultAsync(ct);

    public async Task<PagedList<JobSummaryDto>> ListAsync(
        string? keyword, string? country, string? city,
        decimal? salaryMin, JobStatus status,
        int page, int pageSize,
        CancellationToken ct = default)
    {
        var query = dbContext.Jobs.Where(j => j.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(j =>
                EF.Functions.ILike(j.Title.Value, $"%{keyword}%") ||
                EF.Functions.ILike(j.Description.Value, $"%{keyword}%"));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(j => j.Location.Country == country);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(j => j.Location.City == city);

        if (salaryMin.HasValue)
            query = query.Where(j => j.SalaryRange.Min >= salaryMin.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(j => j.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobSummaryDto(
                j.Id.Value,
                j.Title.Value,
                j.Location.City,
                j.Location.Country,
                j.SalaryRange.Min,
                j.SalaryRange.Max,
                j.SalaryRange.Currency,
                j.PublishedAt))
            .ToListAsync(ct);

        return new PagedList<JobSummaryDto>(items, page, pageSize, total);
    }
}