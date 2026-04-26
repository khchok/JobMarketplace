
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.Jobs.Domain.Events;
using JobMarketplace.Jobs.Domain.ValueObjects;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Jobs.Domain.Aggregates;

public sealed class Job : AggregateRoot<JobId>
{
    private Job(
        JobId id,
        UserId employerId,
        JobTitle title,
        JobDescription description,
        Location location,
        SalaryRange salaryRange,
        DateTime createdAt)
        : base(id)
    {
        EmployerId = employerId;
        Title = title;
        Description = description;
        Location = location;
        SalaryRange = salaryRange;
        Status = JobStatus.Draft;
        CreatedAt = createdAt;
    }

    private Job() : base(default) { }

    public UserId EmployerId { get; private set; }
    public JobTitle Title { get; private set; } = null!;
    public JobDescription Description { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public SalaryRange SalaryRange { get; private set; } = null!;
    public JobStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    public static Result<Job> Create(
        UserId employerId,
        string title,
        string description,
        string city,
        string country,
        decimal salaryMin,
        decimal salaryMax,
        string currency)
    {
        var titleResult = JobTitle.Create(title);
        if (titleResult.IsFailure) return Result<Job>.Failure(titleResult.Error);

        var descResult = JobDescription.Create(description);
        if (descResult.IsFailure) return Result<Job>.Failure(descResult.Error);

        var locationResult = Location.Create(city, country);
        if (locationResult.IsFailure) return Result<Job>.Failure(locationResult.Error);

        var salaryResult = SalaryRange.Create(salaryMin, salaryMax, currency);
        if (salaryResult.IsFailure) return Result<Job>.Failure(salaryResult.Error);

        var job = new Job(
            JobId.NewId(),
            employerId,
            titleResult.Value,
            descResult.Value,
            locationResult.Value,
            salaryResult.Value,
            DateTime.UtcNow);

        return Result<Job>.Success(job);
    }

    public Result Publish(UserId requestingUserId)
    {
        if (requestingUserId != EmployerId)
            return Result.Failure(Error.Unauthorized("Only the owning employer can publish this job."));

        if (Status != JobStatus.Draft)
            return Result.Failure(Error.Conflict($"Cannot publish a job in '{Status}' status."));

        Status = JobStatus.Published;
        PublishedAt = DateTime.UtcNow;
        RaiseDomainEvent(new JobPublishedEvent(Id, EmployerId, PublishedAt.Value));

        return Result.Success();
    }

    public Result Close(UserId requestingUserId)
    {
        if (requestingUserId != EmployerId)
            return Result.Failure(Error.Unauthorized("Only the owning employer can close this job."));

        if (Status != JobStatus.Published)
            return Result.Failure(Error.Conflict($"Cannot close a job in '{Status}' status."));

        Status = JobStatus.Closed;
        RaiseDomainEvent(new JobClosedEvent(Id, EmployerId));

        return Result.Success();
    }
}