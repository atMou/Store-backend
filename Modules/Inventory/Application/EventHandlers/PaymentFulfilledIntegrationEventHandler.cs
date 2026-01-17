using Shared.Application.Features.Payment.Events;

namespace Inventory.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    InventoryDbContext dbContext,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var message = context.Message;
        var orderId = message.OrderId;

        logger.LogInformation(
            "Processing payment fulfilled event for Order {OrderId} - confirming inventory reservations for {ItemCount} items",
            orderId,
            message.OrderItems.Count());

        var confirmations = message.OrderItems.AsIterable().Traverse(orderItem =>
            GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
                inv => inv.ProductId == ProductId.From(orderItem.ProductId),
                NotFoundError.New($"Inventory for product {orderItem.ProductId} not found"),
                opt =>
                {
                    opt = opt.AddInclude(i => i.ColorVariants);
                    return opt;
                },
                inv => inv.ConfirmReservation(
                    ColorVariantId.From(orderItem.ColorVariantId),
                    orderItem.Size,
                    orderItem.Quantity))
        ).As();

        var result = await confirmations.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                "Successfully confirmed inventory reservations for order {OrderId}",
                orderId),
            err => logger.LogError(
                err,
                "Failed to confirm inventory reservations for order {OrderId}: {Error}",
                orderId,
                err.Message)
        );
    }
}