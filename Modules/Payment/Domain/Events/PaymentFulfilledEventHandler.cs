using MassTransit;

using Shared.Application.Features.Payment.Events;

namespace Payment.Domain.Events;
internal class PaymentFulfilledEventHandler(IPublishEndpoint endpoint) : INotificationHandler<PaymentFulfilledEvent>
{
    public async Task Handle(PaymentFulfilledEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new PaymentFulfilledIntegrationEvent()
        {
            UserId = notification.UserId,
            OrderId = notification.OrderId,
            PaymentId = notification.PaymentId,
            CartId = notification.CartId,
            PaymentPaidAt = notification.PaymentPaidAt,
            PaymentTransactionId = notification.PaymentTransactionId
        }, cancellationToken);
    }
}
