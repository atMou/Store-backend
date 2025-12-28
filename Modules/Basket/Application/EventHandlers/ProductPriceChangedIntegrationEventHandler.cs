using Basket.Application.Features.Cart.UpdateItemsPrice;
using Microsoft.Extensions.Logging;
using Shared.Application.Features.Cart.Events;

namespace Basket.Application.EventHandlers;

internal class ProductPriceChangedIntegrationEventHandler(
	ILogger<ProductPriceChangedIntegrationEventHandler> logger,
	ISender sender,
	IUserContext userContext,
    IPublishEndpoint endpoint
) : IConsumer<CartItemsPriceChangedIntegrationEvent>
{
	public async Task Consume(ConsumeContext<CartItemsPriceChangedIntegrationEvent> context)
	{
        logger.LogInformation(
            "Processing CartItemsPriceChangedIntegrationEvent for VariantId: {VariantId}, NewPrice: {NewPrice}", 
            context.Message.ProductId, 
            context.Message.NewPrice);

		var command = new UpdateCartItemsPriceCommand(
            ProductId.From(context.Message.ProductId), 
            context.Message.NewPrice);

		var result = await sender.Send(command, context.CancellationToken);
        
        result.Match(
            Succ: _ => logger.LogInformation(
                "Successfully updated cart items price for VariantId: {VariantId}", 
                context.Message.ProductId),
            Fail: async err =>
            {
                logger.LogError(
                    "Failed to update cart items price for VariantId: {VariantId}. Error: {@Error}", 
                    context.Message.ProductId, 
                    err);
                await endpoint.Publish(
                    new CartItemsPriceChangeFailIntegrationEvent(
                        context.Message.ProductId, 
                        err.ToString()), 
                    context.CancellationToken);
            });
	}
}


