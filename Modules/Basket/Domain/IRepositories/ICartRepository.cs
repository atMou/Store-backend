using Basket.Domain.Models;
using Basket.Persistence.Extensions;

namespace Basket.Domain.Repositories;

public interface ICartRepository
{
    IO<Cart> GetCartById(CartId id, BasketDbContext ctx, Action<QueryableOptions<Cart>>? options = null);

    IO<Cart> GetCartByUserId(UserId id, BasketDbContext ctx, Action<QueryableOptions<Cart>>? options);

    IO<Cart> CreateCart(Cart cart, UserId userId, BasketDbContext ctx);

    IO<Cart> AddCartItemCart(CartItem item, CartId cartId, BasketDbContext ctx);


    public IO<Cart> DeleteCartItem(CartItemId cartItemId, CartId cartId, BasketDbContext ctx);

    public IO<Cart> DeleteCart(CartId cartId, BasketDbContext ctx);
    public IO<int> UpdateCartItemPrice(ProductId productId, decimal newPrice, BasketDbContext ctx);
    public IO<Coupon> GetCouponById(CouponId couponId, BasketDbContext ctx);

}
