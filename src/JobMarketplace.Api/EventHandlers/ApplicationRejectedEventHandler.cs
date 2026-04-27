using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class ApplicationRejectedEventHandler(ILogger<ApplicationRejectedEventHandler> logger)
    : IDomainEventHandler<ApplicationRejectedEvent>
{
    public Task Handle(ApplicationRejectedEvent notification, System.Threading.CancellationToken cancellationToken)
    {
        logger.LogInformation("Application {Id} rejected", notification.ApplicationId);
        return Task.CompletedTask;
    }
}