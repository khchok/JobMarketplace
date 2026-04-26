using FluentAssertions;
using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.Jobs.Domain.Events;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Jobs.Domain.Tests.ValueObjects;

public class JobTests
{
    private static readonly UserId EmployerId = UserId.NewId();

    private static Job CreateValidJob(UserId? employerId = null) =>
        Job.Create(
            employerId ?? EmployerId,
            "Senior .NET Developer",
            "Build cloud-native APIs using .NET 10 and DDD patterns.",
            "Kuala Lumpur", "Malaysia",
            8000m, 15000m, "MYR")
        .Value;

    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        var result = Job.Create(
            EmployerId,
            "Senior .NET Developer",
            "Build cloud-native APIs.",
            "Kuala Lumpur", "Malaysia",
            8000m, 15000m, "MYR");

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(JobStatus.Draft);
        result.Value.EmployerId.Should().Be(EmployerId);
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsFailure()
    {
        var result = Job.Create(EmployerId, "", "Some description.", "KL", "Malaysia", 1000m, 5000m, "MYR");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidSalary_ReturnsFailure()
    {
        var result = Job.Create(EmployerId, "Title", "Desc", "KL", "Malaysia", 9000m, 5000m, "MYR");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Publish_ByOwner_TransitionsToDraftPublished()
    {
        var job = CreateValidJob();
        var result = job.Publish(EmployerId);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Published);
        job.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_ByOwner_RaisesJobPublishedEvent()
    {
        var job = CreateValidJob();
        job.Publish(EmployerId);

        var events = job.PopDomainEvents();
        events.Should().ContainSingle(e => e is JobPublishedEvent);
    }

    [Fact]
    public void Publish_ByNonOwner_ReturnsUnauthorized()
    {
        var job = CreateValidJob();
        var otherId = UserId.NewId();

        var result = job.Publish(otherId);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Unauthorized");
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ReturnsFailure()
    {
        var job = CreateValidJob();
        job.Publish(EmployerId);

        var result = job.Publish(EmployerId);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Close_ByOwner_TransitionsToPublishedClosed()
    {
        var job = CreateValidJob();
        job.Publish(EmployerId);

        var result = job.Close(EmployerId);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Closed);
    }

    [Fact]
    public void Close_ByOwner_RaisesJobClosedEvent()
    {
        var job = CreateValidJob();
        job.Publish(EmployerId);
        job.PopDomainEvents(); // clear publish event

        job.Close(EmployerId);
        var events = job.PopDomainEvents();
        events.Should().ContainSingle(e => e is JobClosedEvent);
    }

    [Fact]
    public void Close_WhenAlreadyClosed_ReturnsFailure()
    {
        var job = CreateValidJob();
        job.Publish(EmployerId);
        job.Close(EmployerId);

        var result = job.Close(EmployerId);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Close_WhenDraft_ReturnsFailure()
    {
        var job = CreateValidJob();
        var result = job.Close(EmployerId);
        result.IsFailure.Should().BeTrue();
    }
}