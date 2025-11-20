using Shared.Messaging.Events;

namespace Basket.Application.EventHandlers;
internal class CartCheckedOutIntegrationEventHandler(BasketDbContext dbContext) : IConsumer<CartCheckedOutIntegrationEvent>
{
    public Task Consume(ConsumeContext<CartCheckedOutIntegrationEvent> context)
    {
        var cartId = context.Message.CartId;
        // Handle the event (e.g., update the database, send a notification, etc.)
        var db = from coupon in Db<BasketDbContext>.liftIO(async (ctx, e) => await
                ctx.Coupons.FirstOrDefaultAsync(coupon => coupon.CartId == CartId.From(cartId), e.Token))

                 from updatedCoupon in coupon.ChangeCouponStatus(CouponStatus.Redeemed)
                 from x in Db<BasketDbContext>.lift(ctx =>
                 {
                     ctx.Coupons.Entry(coupon).CurrentValues.SetValues(updatedCoupon);
                     return unit;
                 })
                 select x;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));
    }
}
