
namespace Inventory.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(
    InventoryDbContext dbContext,
    ILogger<OrderCreatedIntegrationEventHandler> logger)
    : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        var orderId = message.OrderDto.OrderId;

        logger.LogInformation(
            LogEvents.StockReserved,
            "Processing stock reservation for Order {OrderId} with {ItemCount} items",
            orderId,
            message.OrderDto.OrderItemsDtos.Count());

        var db = message.OrderDto.OrderItemsDtos.AsIterable().Traverse(orderItem =>
            GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
                inv => inv.ProductId == ProductId.From(orderItem.ProductId),
                NotFoundError.New($"Inventory for product {orderItem.ProductId} not found"),
                null,
                inv => inv.ReserveStock(
                    ColorVariantId.From(orderItem.ColorVariantId),
                    orderItem.Size,
                    orderItem.Quantity))
        ).As();

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                LogEvents.StockReserved,
                "Successfully reserved stock for order {OrderId} with {ItemCount} items",
                orderId,
                message.OrderDto.OrderItemsDtos.Count()),
            err => logger.LogError(
                LogEvents.StockReserved,
                err,
                "Failed to reserve stock for order {OrderId}. Error: {Error}",
                orderId,
                err.Message)
        );
    }
}
