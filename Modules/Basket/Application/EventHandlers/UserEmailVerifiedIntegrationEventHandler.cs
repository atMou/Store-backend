using Basket.Domain.Enums;

using Microsoft.Extensions.Logging;

using Shared.Application.Features.Cart.Events;
using Shared.Application.Features.User.Events;

namespace Basket.Application.EventHandlers;

public class UserEmailVerifiedIntegrationEventHandler(
    ISender sender,
    IPublishEndpoint endpoint,
    IClock clock,
    BasketDbContext dbContext,
    ILogger<UserEmailVerifiedIntegrationEventHandler> logger) :
    IConsumer<UserEmailVerifiedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserEmailVerifiedIntegrationEvent> context)
    {
        logger.LogInformation(
            "Processing UserEmailVerifiedIntegrationEvent for UserId: {UserId}",
            context.Message.UserId);

        var tax = 0.15M;
        var db = from cart in AddEntity<BasketDbContext, Cart>(
                Cart.Create(UserId.From(context.Message.UserId),
                    tax, context.Message.Address))
                 from coupon in AddEntity<BasketDbContext, Coupon>(
                     Coupon.Create("welcome coupon",
                         10M, clock.UtcNow.AddYears(1),
                         DiscountType.Percentage, clock.UtcNow, 0))
                 let c = cart.AddDiscount(coupon.Id, coupon.Discount)
                 from coupon2 in coupon.ApplyToCart(c.Id, UserId.From(context.Message.UserId), clock.UtcNow)
                 select c;

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseBiEvent(
            async c =>
            {
                logger.LogInformation(
                    "Cart created successfully for User {UserId}, CartId: {CartId}",
                    context.Message.UserId,
                    c.Id.Value);
                await endpoint.Publish(new CartCreatedIntegrationEvent(context.Message.UserId, c.Id.Value));
            },
            async err =>
            {
                logger.LogError(
                    "Failed to create cart for User {UserId}. Error: {@Error}",
                    context.Message.UserId,
                    err);
                await endpoint.Publish(new CartCreateFailIntegrationEvent(context.Message.UserId, err.Message));
            });
    }
}