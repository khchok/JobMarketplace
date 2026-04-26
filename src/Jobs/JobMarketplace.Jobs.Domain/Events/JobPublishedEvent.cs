using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Jobs.Domain.Events;

public record JobPublishedEvent(JobId JobId, UserId EmployerId, DateTime PublishedAt) : IDomainEvent;