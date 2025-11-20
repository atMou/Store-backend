namespace Basket.Application.Features.Cart.GetCart;



public record GetCartByCartIdQuery(CartId CartId) : IQuery<Fin<CartDto>>;

internal class GetCartByCartIdQueryHandler(
    BasketDbContext dbContext,
    ICartRepository cartRepository,
    ICouponRepository couponRepository)
    : IQueryHandler<GetCartByCartIdQuery, Fin<CartDto>>
{
    public Task<Fin<CartDto>> Handle(GetCartByCartIdQuery request, CancellationToken cancellationToken)
    {
        var loadCart = from cart in Db<BasketDbContext>.liftIO(ctx => cartRepository.GetCartById(request.CartId, ctx,
                opt =>
                {
                    opt.AsNoTracking = true;
                    opt.AsSplitQuery = true;
                    opt.AddInclude(c => c.LineItems);
                    opt.AddInclude(c => c.CouponIds);
                }))
                       select cart;

        var loadCoupons =
            from coupons in Db<BasketDbContext>.liftIO(ctx => couponRepository.GetCouponsByCartId(request.CartId, ctx))
            select coupons;

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToDto(coupons));

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}