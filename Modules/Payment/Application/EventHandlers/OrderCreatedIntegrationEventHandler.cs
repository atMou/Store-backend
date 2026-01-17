using MassTransit;

using Microsoft.Extensions.Logging;

using Shared.Application.Features.Order.Events;

namespace Payment.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger, PaymentDbContext dbContext)
    : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var orderId = context.Message.OrderDto.OrderId;
        var userId = context.Message.OrderDto.UserId;
        var total = context.Message.OrderDto.TotalAfterDiscounted;
        var tax = context.Message.OrderDto.Tax;
        var cartId = context.Message.OrderDto.CartId;


        var db =
            AddEntity<PaymentDbContext, Domain.Models.Payment>(
                Domain.Models.Payment.Create(OrderId.From(orderId), UserId.From(userId), CartId.From(cartId),
                    total, tax));


        var results = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        results.IfFail(err => logger.LogWarning("Pending order added but there was an issue. {err}", err));
    }
}
