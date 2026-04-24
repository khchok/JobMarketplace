using MediatR;

namespace JobMarketplace.SharedKernel.Primitives;


public interface IDomainEventHandler<T> : INotificationHandler<T>
    where T : IDomainEvent
{
}