using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Applications.Domain.Events;

public sealed record ApplicationReviewedEvent(
    SharedKernel.Ids.ApplicationId ApplicationId,
    JobId JobId) : IDomainEvent;