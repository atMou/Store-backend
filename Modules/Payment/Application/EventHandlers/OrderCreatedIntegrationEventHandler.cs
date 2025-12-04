using MassTransit;

using Microsoft.Extensions.Logging;

using Payment.Application.Features.PaymentStart;

using Shared.Application.Features.Order.Events;

namespace Payment.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(ISender sender, ILogger<OrderCreatedIntegrationEventHandler> logger) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var orderId = context.Message.OrderId;
        var userId = context.Message.UserId;
        var total = context.Message.TotalAfterDiscounted;
        var tax = context.Message.Tax;
        var cartId = context.Message.CartId;

        var results = await sender.Send(new PaymentStartCommand()
        {
            OrderId = OrderId.From(orderId),
            UserId = UserId.From(userId),
            CartId = CartId.From(cartId),
            Total = total,
            Tax = tax,
        });

        results.IfFail(err => logger.LogCritical("Failed to add pending order. {err}", err));
    }
}
