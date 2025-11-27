namespace Basket.Application.Features.Cart.DeleteCouponFromCart;

public record DeleteCouponFromCartCommand(CartId CartId, CouponId CouponId) : ICommand<Fin<Unit>>;

internal class DeleteCouponFromCartCommandHandler(
    BasketDbContext dbContext,
    IClock clock)
    : ICommandHandler<DeleteCouponFromCartCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteCouponFromCartCommand command, CancellationToken cancellationToken)
    {
        var loadCart =
            GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                opt =>
                {
                    opt.AddInclude(cart => cart.CouponIds);
                    return opt;
                },
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."));

        var loadCoupon = GetEntity<BasketDbContext, Domain.Models.Coupon>(
            coupon => coupon.Id == command.CouponId,
            NotFoundError.New($"Coupon with Id {command.CouponId.Value} not found."));


        var result = (loadCart, loadCoupon)
            .Apply((cart, coupon) =>
                (cart, coupon));

        var db = from t in result
                 from res in UpdateEntity<BasketDbContext, Domain.Models.Cart>(t.cart,
                     c => c.RemoveDiscount(t.coupon.Discount.Apply(t.cart.TotalSub), t.coupon.Id))
                 select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}