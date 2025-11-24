namespace Basket.Application.Features.Cart.AddCouponToCart;

public record AddCouponToCartCommand(CartId CartId, CouponId CouponId) : ICommand<Fin<Unit>>;

internal class AddCouponToCartCommandHandler(
    BasketDbContext dbContext,
    IClock clock)
    : ICommandHandler<AddCouponToCartCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AddCouponToCartCommand command, CancellationToken cancellationToken)
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

        Db<BasketDbContext, Unit> db = from t in result
                                       from co in t.coupon.ApplyToCart(t.cart.Id, t.cart.UserId, clock.UtcNow)
                                       let discount = t.coupon.Discount.Apply(t.cart.TotalSub)
                                       from res in UpdateEntity<BasketDbContext, Domain.Models.Cart>(t.cart,
                                           c => c.AddDiscount(t.coupon.Id, discount))
                                       select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}