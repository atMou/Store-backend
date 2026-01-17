using MassTransit;
using Shared.Application.Features.Payment.Events;
using Shared.Application.Contracts.Order.Queries;

namespace Payment.Domain.Events;

internal class PaymentCancelledEventHandler(IPublishEndpoint endpoint, ISender sender)
    : INotificationHandler<PaymentCancelledEvent>
{
    public async Task Handle(PaymentCancelledEvent notification, CancellationToken cancellationToken)
    {
        // Get order details to include items for inventory release
        var orderQuery = new GetOrderByIdQuery
        {
            OrderId = notification.OrderId
        };

        var orderResult = await sender.Send(orderQuery, cancellationToken);

        await orderResult.Match(
            async order =>
            {
                await endpoint.Publish(new PaymentCancelledIntegrationEvent
                {
                    PaymentId = notification.PaymentId.Value,
                    OrderId = notification.OrderId.Value,
                    UserId = notification.UserId.Value,
                    CartId = notification.CartId.Value,
                    CancelledAt = notification.CancelledAt,
                    Reason = "Payment was cancelled or failed",
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
