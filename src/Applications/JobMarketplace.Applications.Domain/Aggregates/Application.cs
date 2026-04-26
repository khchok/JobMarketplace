using JobMarketplace.Applications.Domain.Enums;
using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.Applications.Domain.ValueObjects;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;
using ApplicationId = JobMarketplace.SharedKernel.Ids.ApplicationId;

namespace JobMarketplace.Applications.Domain.Aggregates;

public sealed class Application : AggregateRoot<ApplicationId>
{
    private Application(
        ApplicationId id,
        JobId jobId,
        UserId candidateId,
        CoverLetter coverLetter,
        DateTime submittedAt)
        : base(id)
    {
        JobId = jobId;
        CandidateId = candidateId;
        CoverLetter = coverLetter;
        Status = ApplicationStatus.Submitted;
        SubmittedAt = submittedAt;
        UpdatedAt = submittedAt;
    }

    private Application() : base(default) { }

    public JobId JobId { get; private set; }
    public UserId CandidateId { get; private set; }
    public CoverLetter CoverLetter { get; private set; } = null!;
    public ApplicationStatus Status { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<Application> Create(
        JobId jobId,
        UserId candidateId,
        string coverLetter)
    {
        var coverLetterResult = CoverLetter.Create(coverLetter);
        if (coverLetterResult.IsFailure)
            return Result<Application>.Failure(coverLetterResult.Error);

        var id = ApplicationId.NewId();
        var now = DateTime.UtcNow;

        var application = new Application(id, jobId, candidateId, coverLetterResult.Value, now);
        application.RaiseDomainEvent(new ApplicationSubmittedEvent(id, jobId, candidateId));

        return Result<Application>.Success(application);
    }

    public Result MarkReviewed()
    {
        if (Status != ApplicationStatus.Submitted)
            return Result.Failure(Error.Conflict($"Cannot review an application in '{Status}' status."));

        Status = ApplicationStatus.Reviewed;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ApplicationReviewedEvent(Id, JobId));

        return Result.Success();
    }

    public Result Accept()
    {
        if (Status != ApplicationStatus.Reviewed)
            return Result.Failure(Error.Conflict($"Cannot accept an application in '{Status}' status."));

        Status = ApplicationStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ApplicationAcceptedEvent(Id, JobId, CandidateId));

        return Result.Success();
    }

    public Result Reject()
    {
        if (Status != ApplicationStatus.Reviewed)
            return Result.Failure(Error.Conflict($"Cannot reject an application in '{Status}' status."));

        Status = ApplicationStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ApplicationRejectedEvent(Id, JobId, CandidateId));

        return Result.Success();
    }
}