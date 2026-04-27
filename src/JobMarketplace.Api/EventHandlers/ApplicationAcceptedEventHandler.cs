using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class ApplicationAcceptedEventHandler(ILogger<ApplicationAcceptedEventHandler> logger)
    : IDomainEventHandler<ApplicationAcceptedEvent>
{
    public Task Handle(ApplicationAcceptedEvent notification, System.Threading.CancellationToken cancellationToken)
    {
        logger.LogInformation("Application {Id} accepted", notification.ApplicationId);
        return Task.CompletedTask;
    }
}
