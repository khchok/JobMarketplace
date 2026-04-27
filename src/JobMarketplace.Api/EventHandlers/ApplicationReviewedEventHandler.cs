using JobMarketplace.Applications.Domain.Events;
using JobMarketplace.SharedKernel.Primitives;

namespace JobMarketplace.Api.EventHandlers;

public sealed class ApplicationReviewedEventHandler(ILogger<ApplicationReviewedEventHandler> logger)
    : IDomainEventHandler<ApplicationReviewedEvent>
{
    public Task Handle(ApplicationReviewedEvent notification, System.Threading.CancellationToken cancellationToken)
    {
        logger.LogInformation("Application {Id} marked reviewed", notification.ApplicationId);
        return Task.CompletedTask;
    }
}
