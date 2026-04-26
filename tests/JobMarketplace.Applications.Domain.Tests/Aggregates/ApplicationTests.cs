using FluentAssertions;
using JobMarketplace.Applications.Domain.Aggregates;
using JobMarketplace.Applications.Domain.Enums;
using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Applications.Domain.Tests.Aggregates;

public class ApplicationTests
{
    private static readonly JobId JobId = JobId.NewId();
    private static readonly UserId CandidateId = UserId.NewId();

    private static Application CreateValidApplication() =>
        Application.Create(JobId, CandidateId, "I am a great fit because...").Value;

    [Fact]
    public void Create_SetsStatusToSubmitted()
    {
        var result = Application.Create(JobId, CandidateId, "Cover letter text.");
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(ApplicationStatus.Submitted);
    }

    [Fact]
    public void Create_RaisesApplicationSubmittedEvent()
    {
        var result = Application.Create(JobId, CandidateId, "Cover letter text.");
        var events = result.Value.PopDomainEvents();
        events.Should().ContainSingle(e => e is ApplicationSubmittedEvent);
    }

    [Fact]
    public void Create_WithEmptyCoverLetter_Fails()
    {
        var result = Application.Create(JobId, CandidateId, "");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void MarkReviewed_WhenSubmitted_TransitionsToReviewed()
    {
        var app = CreateValidApplication();
        var result = app.MarkReviewed();
        result.IsSuccess.Should().BeTrue();
        app.Status.Should().Be(ApplicationStatus.Reviewed);
    }

    [Fact]
    public void MarkReviewed_WhenSubmitted_RaisesApplicationReviewedEvent()
    {
        var app = CreateValidApplication();
        app.PopDomainEvents(); // clear create event
        app.MarkReviewed();
        var events = app.PopDomainEvents();
        events.Should().ContainSingle(e => e is ApplicationReviewedEvent);
    }

    [Fact]
    public void MarkReviewed_WhenNotSubmitted_Fails()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        var result = app.MarkReviewed(); // second call
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Accept_WhenReviewed_TransitionsToAccepted()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        var result = app.Accept();
        result.IsSuccess.Should().BeTrue();
        app.Status.Should().Be(ApplicationStatus.Accepted);
    }

    [Fact]
    public void Accept_WhenNotReviewed_Fails()
    {
        var app = CreateValidApplication();
        var result = app.Accept(); // still Submitted
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Reject_WhenReviewed_TransitionsToRejected()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        var result = app.Reject();
        result.IsSuccess.Should().BeTrue();
        app.Status.Should().Be(ApplicationStatus.Rejected);
    }

    [Fact]
    public void Reject_WhenNotReviewed_Fails()
    {
        var app = CreateValidApplication();
        var result = app.Reject();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Accept_WhenAlreadyAccepted_Fails()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        app.Accept();
        var result = app.Accept();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Reject_WhenAlreadyRejected_Fails()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        app.Reject();
        var result = app.Reject();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Accept_WhenRejected_Fails()
    {
        var app = CreateValidApplication();
        app.MarkReviewed();
        app.Reject();
        var result = app.Accept();
        result.IsFailure.Should().BeTrue();
    }
}