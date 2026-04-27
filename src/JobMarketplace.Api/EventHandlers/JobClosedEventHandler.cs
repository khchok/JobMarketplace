using JobMarketplace.Jobs.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class JobClosedEventHandler(ILogger<JobClosedEventHandler> logger)
    : IDomainEventHandler<JobClosedEvent>
{
    public Task Handle(JobClosedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Job {JobId} closed", notification.JobId);
        return Task.CompletedTask;
    }
}