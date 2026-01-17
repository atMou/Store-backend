namespace Basket.Application.Features.Cart.AddCouponToCart;

public record AddCouponToCartCommand(CartId CartId, string CouponCode) : ICommand<Fin<Unit>>;

internal class AddCouponToCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    IClock clock)
    : ICommandHandler<AddCouponToCartCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddCouponToCartCommand command, CancellationToken cancellationToken)
    {
        var loadCart =
            GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."));

        var loadCoupon = GetEntity<BasketDbContext, Domain.Models.Coupon>(
            coupon => coupon.Code == command.CouponCode,
            NotFoundError.New($"Coupon with code {command.CouponCode} not found."));


        var result = (loadCart, loadCoupon)
            .Apply((cart, coupon) =>
                (cart, coupon));

        var db = from res in result
                 from user in userContext.GetCurrentUser<IO>().As()
                 from _ in UpdateEntity<BasketDbContext, Domain.Models.Coupon>(res.coupon,
                     coupon => coupon.ApplyToCart(res.cart.Id, UserId.From(user.Id), clock.UtcNow))
                 from __ in UpdateEntity<BasketDbContext, Domain.Models.Cart>(res.cart,
                     c => c.AddDiscount(res.coupon.Id, res.coupon.Discount))
                 select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}