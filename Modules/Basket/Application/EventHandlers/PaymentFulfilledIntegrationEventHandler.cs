using Microsoft.Extensions.Logging;

using Shared.Application.Features.Payment.Events;

namespace Basket.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    BasketDbContext dbContext,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger,
    IClock clock
    ) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var db = from cart in GetUpdateEntity<BasketDbContext, Cart>(
                cart => cart.Id == CartId.From(context.Message.CartId),
                 NotFoundError.New($"Cart not found for ID: {context.Message.CartId}"),
                null,
                cart => cart.SetIsActiveToFalse().As())

                 from a in GetUpdateEntities<BasketDbContext, Coupon>(
                     coupon => cart.CouponIds.Contains(coupon.Id),
                     null,
                     (coupon) => coupon.MarkAsRedeemed(clock.UtcNow)
                 )
                 from c in AddEntity<BasketDbContext, Cart>(Cart.Create(cart.UserId, cart.Tax.Rate * 100, cart.DeliveryAddress))
                 select unit;
        await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
           .RaiseOnFail(err => logger.LogError($"Error processing payment fulfilled for cart ID: {context.Message.CartId}. Error: {err}"));
    }
}
