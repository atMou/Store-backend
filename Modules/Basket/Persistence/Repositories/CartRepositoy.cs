using Basket.Domain.Models;
using Basket.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;

using Shared.Domain.Errors;

namespace Basket.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    public IO<Cart> GetCartById(CartId id, BasketDbContext ctx, Action<QueryableOptions<Cart>>? options)
    {
        return from c in IO<Cart?>.LiftAsync(async e => await
                ctx.Carts.WithOptions(options)
                    .FirstOrDefaultAsync(cart => cart.Id.Value == id.Value, e.Token))
               from a in when(c is null, IO.fail<Unit>(NotFoundError.New($"Cart with id '{id}' was not found.")))
               select c;
    }

    public IO<Cart> GetCartByUserId(UserId id, BasketDbContext ctx, Action<QueryableOptions<Cart>>? options)
    {
        return from c in IO<Cart?>.LiftAsync(async e =>
                await ctx.Carts.WithOptions(options)
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

    public IO<Cart> AddCartItemCart(CartItem item, CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.CartItems); })
               from x in IO.lift(_ => cart.AddCartItem(item))
               select x;
    }

    public IO<Cart> DeleteCartItem(CartItemId cartItemId, CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.CartItems); })
               from x in IO.lift(_ => cart.DeleteCartItem(cartItemId))
               select cart;
    }

    public IO<Cart> DeleteCart(CartId cartId, BasketDbContext ctx)
    {
        return from cart in GetCartById(cartId, ctx, options => { options.AddInclude(cart => cart.CartItems); })
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