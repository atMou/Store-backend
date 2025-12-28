namespace Basket.Application.Features.Cart.AddCouponToCart;

public record AddCouponToCartCommand(CartId CartId, CouponId CouponId) : ICommand<Fin<Unit>>;

internal class AddCouponToCartCommandHandler(
    BasketDbContext dbContext,
    IClock clock)
    : ICommandHandler<AddCouponToCartCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddCouponToCartCommand command, CancellationToken cancellationToken)
    {
        var loadCart =
            GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                opt =>
                {
                    opt.AddInclude(cart => cart.CouponIds);
                    return opt;
                });

        var loadCoupon = GetEntity<BasketDbContext, Domain.Models.Coupon>(
            coupon => coupon.Id == command.CouponId,
            NotFoundError.New($"Coupon with Id {command.CouponId.Value} not found."));


        var result = (loadCart, loadCoupon)
            .Apply((cart, coupon) =>
                (cart, coupon));

        var db = from res in result
                 from _ in UpdateEntity<BasketDbContext, Domain.Models.Cart>(res.cart,
                     c => c.AddDiscount(res.coupon.Id, res.coupon.Discount))
                 select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}