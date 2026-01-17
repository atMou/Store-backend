namespace Order.Application.EventHandlers;

public class DeliveryAddressChangedIntegrationEventHandler(
    OrderDBContext dbContext,
    ILogger<DeliveryAddressChangedIntegrationEventHandler> logger)
    : IConsumer<DeliveryAddressChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DeliveryAddressChangedIntegrationEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
               "Processing DeliveryAddressChangedIntegrationEvent for CartId: {CartId}",
               message.CartId);

        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
            order => order.CartId == CartId.From(message.CartId),
            NotFoundError.New($"Order with CartId '{message.CartId}' not found."),
            null,
            order => order.UpdateShippingAddress(message.Address)
        ).Map(_ => unit);

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                "Successfully updated delivery address for Order with CartId: {CartId}",
                message.CartId),
            err => logger.LogError(
                "Failed to update delivery address for Order with CartId: {CartId}. Error: {@Error}",
                message.CartId,
                err)
        );


    }
}
