using MassTransit;

using Shared.Application.Features.Payment.Events;
using Shared.Application.Contracts.Order.Queries;

namespace Payment.Domain.Events;
internal class PaymentFulfilledEventHandler(IPublishEndpoint endpoint, ISender sender) : INotificationHandler<PaymentFulfilledEvent>
{
    public async Task Handle(PaymentFulfilledEvent notification, CancellationToken cancellationToken)
    {
        var orderQuery = new GetOrderByIdQuery
        {
            OrderId = notification.OrderId
        };

        var orderResult = await sender.Send(orderQuery, cancellationToken);

        await orderResult.Match(
            async order =>
            {
                await endpoint.Publish(new PaymentFulfilledIntegrationEvent()
                {
                    UserId = notification.UserId.Value,
                    OrderId = notification.OrderId.Value,
                    PaymentId = notification.PaymentId.Value,
                    CartId = notification.CartId.Value,
                    PaymentPaidAt = notification.PaymentPaidAt,
                    PaymentTransactionId = notification.PaymentTransactionId,
                    OrderItems = order.OrderItems.Select(item => new Shared.Application.Contracts.Order.Dtos.OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ColorVariantId = item.ColorVariantId,
                        SizeVariantId = item.SizeVariantId,
                        Size = item.Size,
                        Sku = item.Sku,
                        Slug = item.Slug,
                        ImageUrl = item.ImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal,
                        OrderItemId = item.OrderItemId
                    })
                }, cancellationToken);
            },
            error =>
            {
                // Log error if needed
                return Task.CompletedTask;
            }
        );
    }
}
