using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Jobs.Domain.Events;

public sealed record JobClosedEvent(JobId JobId, UserId EmployerId) : IDomainEvent;
