using Basket.Application.Features.Cart.UpdateItemsPrice;

using Microsoft.Extensions.Logging;

using Shared.Messaging.Events;

namespace Basket.Application.EventHandlers;
internal class ProductPriceChangedIntegrationEventHandler(
    ILogger<ProductPriceChangedIntegrationEventHandler> logger,
    ISender sender,
    IUserContext userContext
) : IConsumer<ChangeCartItemsPriceIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ChangeCartItemsPriceIntegrationEvent> context)
    {
        var command =
            new UpdateCartItemsPriceCommand(ProductId.From(context.Message.ProductId), context.Message.NewPrice);

        var io =
            from u in userContext.GetCurrentUser<IO>()
            from res in IO.liftAsync(async _ => await sender.Send(command))
            select res.Match(
                result => { logger.LogInformation("SubTotal count of carts: {count} updated", result.UpdatedCount); },
                  async e =>
                {
                    logger.LogError("Error updating cart items price: {Error}", e);
                    await context.Publish(
                        new FailedChangeCartItemsPriceIntegrationEvent(ProductId.From(context.Message.ProductId), e), context.CancellationToken);
                });
        await io.RunAsync(EnvIO.New(null, context.CancellationToken));
    }
}


