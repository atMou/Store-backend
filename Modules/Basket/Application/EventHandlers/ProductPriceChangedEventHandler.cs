using Basket.Application.Features.Cart.UpdateItemsPrice;

using MediatR;

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

        var io =
            from _1 in IO.lift(() => logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name))
            let command = new UpdateCartItemsPriceCommand(ProductId.From(context.Message.ProductId), context.Message.NewPrice)
            from u in userContext.GetCurrentUser<IO>()
            from a in IO.liftAsync(async _ => await sender.Send(command))
            select a.Match(
                result => { logger.LogInformation("SubTotal count of carts: {count} updated", result.updatedCount); },
                  e =>
                {
                    logger.LogError("Error updating cart items price: {Error}", e);
                    context.Publish(
                       new FailedChangeCartItemsPriceIntegrationEvent(ProductId.From(context.Message.ProductId), e), context.CancellationToken);
                });
        await io.RunAsync(EnvIO.New(null, context.CancellationToken));
    }
}


