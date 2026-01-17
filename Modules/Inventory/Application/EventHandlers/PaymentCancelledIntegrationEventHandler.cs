using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Application.Features.Payment.Events;
using Shared.Infrastructure.Logging;
using Shared.Domain.Errors;

namespace Inventory.Application.EventHandlers;

public class PaymentCancelledIntegrationEventHandler(
    InventoryDbContext dbContext,
    ILogger<PaymentCancelledIntegrationEventHandler> logger
) : IConsumer<PaymentCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentCancelledIntegrationEvent> context)
    {
        var message = context.Message;
        var orderId = message.OrderId;

        logger.LogInformation(
            LogEvents.StockReleased,
            "Processing payment cancelled event for Order {OrderId} - releasing inventory reservations for {ItemCount} items",
            orderId,
            message.OrderItems.Count());

        // Release reservations for each order item
        var releases = message.OrderItems.AsIterable().Traverse(orderItem =>
            GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
                inv => inv.ProductId == ProductId.From(orderItem.ProductId),
                NotFoundError.New($"Inventory for product {orderItem.ProductId} not found"),
                opt =>
                {
                    opt = opt.AddInclude(i => i.ColorVariants);
                    return opt;
                },
                inv => inv.ReleaseReservation(
                    ColorVariantId.From(orderItem.ColorVariantId),
                    orderItem.Size,
                    orderItem.Quantity))
        ).As();

        var result = await releases.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                LogEvents.StockReleased,
                "Successfully released inventory reservations for order {OrderId}",
                orderId),
            err => logger.LogError(
                err,
                "Failed to release inventory reservations for order {OrderId}: {Error}",
                orderId,
                err.Message)
        );
    }
}
