using MassTransit;

using Microsoft.Extensions.Logging;

using Payment.Application.Features.PaymentStart;

using Shared.Application.Features.Order.Events;

namespace Payment.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(ISender sender, ILogger<OrderCreatedIntegrationEventHandler> logger) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var orderId = context.Message.OrderDto.OrderId;
        var userId = context.Message.OrderDto.UserId;
        var total = context.Message.OrderDto.TotalAfterDiscounted;
        var tax = context.Message.OrderDto.Tax;
        var cartId = context.Message.OrderDto.CartId;

        var results = await sender.Send(new PaymentStartCommand()
        {
            OrderId = orderId,
            UserId = userId,
            CartId = cartId,
            Total = total,
            Tax = tax,
        });

        results.IfFail(err => logger.LogCritical("Failed to add pending order. {err}", err));
    }
}
