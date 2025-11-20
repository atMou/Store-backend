namespace Basket.Application.Features.Cart.GetCart;

public record GetCartByUserIdQuery(UserId UserId) : IQuery<Fin<CartDto>>;

internal class GetCartByUserIdQueryHandler(BasketDbContext dbContext, ICartRepository cartRepository, ICouponRepository couponRepository)
    : IQueryHandler<GetCartByUserIdQuery, Fin<CartDto>>
{
    public Task<Fin<CartDto>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        var loadCart = from cart in Db<BasketDbContext>.liftIO(ctx => cartRepository.GetCartByUserId(request.UserId, ctx,
                opt =>
                {
                    opt.AsNoTracking = true;
                    opt.AsSplitQuery = true;
                    opt.AddInclude(c => c.LineItems);
                    opt.AddInclude(c => c.CouponIds);
                }))
                       select cart;

        var loadCoupons =
            from coupons in Db<BasketDbContext>.liftIO(ctx => couponRepository.GetCouponsByUserId(request.UserId, ctx))
            select coupons;

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToDto(coupons));

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}