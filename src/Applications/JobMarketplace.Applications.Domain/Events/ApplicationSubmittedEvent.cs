using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Applications.Domain.Events;

public sealed record ApplicationSubmittedEvent(
    SharedKernel.Ids.ApplicationId ApplicationId,
    JobId JobId,
    UserId CandidateId) : IDomainEvent;