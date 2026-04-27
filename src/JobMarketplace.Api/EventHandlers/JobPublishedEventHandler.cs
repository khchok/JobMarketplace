using JobMarketplace.Jobs.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class JobPublishedEventHandler(ILogger<JobPublishedEventHandler> logger)
    : IDomainEventHandler<JobPublishedEvent>
{
    public Task Handle(JobPublishedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Job {JobId} published by employer {EmployerId}", notification.JobId, notification.EmployerId);
        return Task.CompletedTask;
    }
}