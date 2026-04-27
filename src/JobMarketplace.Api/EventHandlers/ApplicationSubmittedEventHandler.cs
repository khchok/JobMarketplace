using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class ApplicationSubmittedEventHandler(ILogger<ApplicationSubmittedEventHandler> logger)
    : IDomainEventHandler<ApplicationSubmittedEvent>
{
    public Task Handle(ApplicationSubmittedEvent notification, System.Threading.CancellationToken cancellationToken)
    {
        logger.LogInformation("Application {Id} submitted for job {JobId}", notification.ApplicationId, notification.JobId);
        return Task.CompletedTask;
    }
}
