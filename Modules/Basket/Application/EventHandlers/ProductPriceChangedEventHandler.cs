using Basket.Application.Features.Cart.UpdateItemsPrice;

using Microsoft.Extensions.Logging;

using Shared.Application.Features.Cart.Events;

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

        var result = await sender.Send(command, context.CancellationToken);
        result.Match(
            res => { logger.LogInformation("SubTotal count of carts: {count} updated", res.UpdatedCount); },
            async e =>
            {
                logger.LogError("Error updating cart items price: {Error}", e);
                await context.Publish(
                    new FailedChangeCartItemsPriceIntegrationEvent(ProductId.From(context.Message.ProductId), e), context.CancellationToken);
            });
    }
}


