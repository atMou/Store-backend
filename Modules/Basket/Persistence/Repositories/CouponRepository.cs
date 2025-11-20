using Shared.Persistence.Extensions;

namespace Basket.Persistence.Repositories;
internal class CouponRepository : ICouponRepository
{
    public IO<Coupon> GetCouponById(CouponId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null)
    {
        return from x in IO<Coupon?>.LiftAsync(async e =>
                   await ctx.Coupons.WithQueryOptions(options)
                       .FirstOrDefaultAsync(coupon => coupon.Id == id, e.Token))
               from a in when(x is null, IO.fail<Unit>(NotFoundError.New($"Coupon with id '{id.Value}' not found")))
               select x;
    }

    public IO<IEnumerable<Coupon>> GetCouponsByCartId(CartId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null)
    {
        return from x in IO<IEnumerable<Coupon>>.LiftAsync(async e =>
                 await ctx.Coupons.WithQueryOptions(options)
                     .Where(coupon => coupon.CartId == id)
                     .ToListAsync(e.Token))
               select x;
    }

    public IO<IEnumerable<Coupon>> GetCouponsByUserId(UserId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null)
    {
        return from x in IO<IEnumerable<Coupon>>.LiftAsync(async e =>
                 await ctx.Coupons.WithQueryOptions(options)
                     .Where(coupon => coupon.UserId == id)
                     .ToListAsync(e.Token))
               select x;
    }
}
