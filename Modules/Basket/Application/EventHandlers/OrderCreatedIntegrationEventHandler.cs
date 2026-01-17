using Microsoft.Extensions.Logging;

using Shared.Application.Features.Order.Events;

namespace Basket.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(
    BasketDbContext dbContext,
    ILogger<OrderCreatedIntegrationEventHandler> logger)
    : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        var cartId = CartId.From(message.OrderDto.CartId);

        logger.LogInformation(
            "Processing OrderCreatedIntegrationEvent for User {UserId} - deactivating cart {CartId} and creating new cart",
            message.OrderDto.UserId,
            message.OrderDto.CartId);

        var db = GetUpdateEntity<BasketDbContext, Cart>(
            c => c.Id == cartId,
            NotFoundError.New($"Cart with ID {cartId.Value} not found"),
            null,
            cart => cart.SetIsActiveToFalse());
        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            cart => logger.LogInformation(
                "Successfully set cart with id {CartId}, for user {UserId},: to unactive",
                cart.Id.Value,
                message.OrderDto.UserId),
            err => logger.LogError(
                err,
                "Failed to set cart for user {UserId} to unactive: {Error}",
                message.OrderDto.UserId,
                err.Message));
    }
}
