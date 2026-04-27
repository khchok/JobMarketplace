using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Identity.Domain.Events;

public sealed record UserProfileCreatedEvent(UserId UserId) : IDomainEvent;
