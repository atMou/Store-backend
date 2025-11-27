using MassTransit;

using Order.Domain.Events;

using Shared.Application.Features.Order.Events;

namespace Order.Application.EventHandlers;

public class OrderCreatedDomainEventHandler(IPublishEndpoint endpoint) : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new OrderCreatedIntegrationEvent()
        {
            OrderDto = notification.OrderDto
        }, cancellationToken);
    }
}
