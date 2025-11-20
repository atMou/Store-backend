using Shared.Persistence.Extensions;

namespace Basket.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    public IO<Cart> GetCartById(CartId id, BasketDbContext ctx, Action<QueryOptions<Cart>>? options)
    {
        return from c in IO<Cart?>.LiftAsync(async e => await
                ctx.Carts.WithQueryOptions(options)
                    .FirstOrDefaultAsync(cart => cart.Id == id, e.Token))
               from a in when(c is null, IO.fail<Unit>(NotFoundError.New($"Cart with id '{id}' was not found.")))
               select c;
    }

    public IO<Unit> ProductIncludedInAnyCart(ProductId productId, BasketDbContext ctx,
        Action<QueryOptions<Cart>>? options = null)
    {
        return from exists in IO.liftAsync(e => ctx.Carts
                .Where(c => c.LineItems.Any(ci => ci.ProductId == productId))
                .AnyAsync(e.Token))
               from a in when(exists,
                   IO.fail<Unit>(NotFoundError.New($"Product with id '{productId}' is included some carts.")))
               select unit;
    }

    public IO<Cart> GetCartByUserId(UserId id, BasketDbContext ctx, Action<QueryOptions<Cart>>? options)
    {
        return from c in IO<Cart?>.LiftAsync(async e =>
                await ctx.Carts.WithQueryOptions(options)
                    .FirstOrDefaultAsync(cart => cart.UserId.Value == id.Value, e.Token))
               select c;
    }

    public IO<Cart> CreateCart(Cart cart, UserId userId, BasketDbContext ctx)
    {
        return from c in GetCartByUserId(userId, ctx, null)
               from _ in iff(c is not null,
                   IO.fail<Unit>(InvalidOperationError.New("Cart for user with id: '{c.UserId.Value}' already exists")),
                   IO.lift(_ =>
                   {
                       ctx.Carts.Add(cart);
                       return unit;
                   }))
               select c;
    }

    public IO<Cart> AddCartItemCart(LineItem item, CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.LineItems); })
               from x in IO.lift(_ => cart.AddLineItems(item))
               select x;
    }

    public IO<Cart> DeleteLineItem(ProductId productId, CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.LineItems); })
               from x in IO.lift(_ => cart.DeleteLineItem(productId))
               select cart;
    }

    public IO<Cart> DeleteCart(CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.LineItems); })
               from x in IO.lift(_ => ctx.Carts.Remove(cart))
               select cart;
    }

    public IO<int> UpdateCartItemPrice(ProductId productId, decimal newPrice, BasketDbContext ctx)
    {
        return from res in IO.liftAsync(async e =>
                await ctx.CartItems.Where(ci => ci.ProductId == productId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(ci => ci.UnitPrice.Value, _ => newPrice), e.Token))
               select res;
    }

    public IO<Coupon> GetCouponById(CouponId? couponId, BasketDbContext ctx)
    {
        return from c in IO<Coupon?>.LiftAsync(async e =>
                await ctx.Coupons.FirstOrDefaultAsync(c => c.Id == couponId, e.Token))
               from a in when(c is null, IO.fail<Unit>(NotFoundError.New($"Coupon with id '{couponId}' was not found.")))
               select c;
    }
}