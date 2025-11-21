namespace Basket.Application.Features.Cart.AddCouponToCart;



public record AddCouponToCartCommand(Guid CartId, Guid CouponId) : ICommand<Fin<Unit>>;


internal class AddCouponToCartCommandHandler(
    BasketDbContext dbContext,
    ICartRepository cartRepository,
    IUserContext userContext,
    IClock clock)
    : ICommandHandler<AddCouponToCartCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AddCouponToCartCommand command, CancellationToken cancellationToken)
    {
        var loadCart = from c in Db<BasketDbContext>.liftIO(ctx =>
            cartRepository.GetCartById(CartId.From(command.CartId), ctx,
                options => options.AddInclude(cart => cart.CouponIds)))
                       from _ in userContext.IsSameUser<IO>(c.UserId,
                                 UnAuthorizedError.New("You are not authorized to add a coupon to this cart."))
                             | userContext.IsInRole<IO>(Role.Admin, UnAuthorizedError.New("You are not authorized to add a coupon to this cart.")).As()
                       select c;

        var loadCoupon = from c in Db<BasketDbContext>.liftIO(ctx =>
            cartRepository.GetCouponById(CouponId.From(command.CouponId), ctx))
                         select c;

        Db<BasketDbContext, Unit> db =
            from x in (loadCart, loadCoupon).Apply((cart, coupon) =>
                from co in coupon.ApplyToCart(cart.Id, cart.UserId, clock.UtcNow)
                let discount = coupon.Discount.Apply(cart.TotalSub)
                from ca in cart.AddDiscount(coupon.Id, discount)
                select (cart, ca))
            from res in Db<BasketDbContext>.lift(x)
            from a in Db<BasketDbContext>.lift(ctx =>
            {
                ctx.Carts.Entry(res.cart).CurrentValues.SetValues(res.ca);
                return res;

            })
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }




}
