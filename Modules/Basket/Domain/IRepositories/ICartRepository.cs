using Shared.Persistence.Extensions;

namespace Basket.Domain.IRepositories;

public interface ICartRepository
{
    IO<Cart> GetCartById(CartId id, BasketDbContext ctx, Action<QueryOptions<Cart>>? options = null);
    IO<Unit> ProductIncludedInAnyCart(ProductId productId, BasketDbContext ctx, Action<QueryOptions<Cart>>? options = null);

    IO<Cart> GetCartByUserId(UserId id, BasketDbContext ctx, Action<QueryOptions<Cart>>? options);

    IO<Cart> CreateCart(Cart cart, UserId userId, BasketDbContext ctx);

    IO<Cart> AddCartItemCart(LineItem item, CartId cartId, BasketDbContext ctx);


    public IO<Cart> DeleteLineItem(ProductId productId, CartId cartId, BasketDbContext ctx);

    public IO<Cart> DeleteCart(CartId cartId, BasketDbContext ctx);
    public IO<int> UpdateCartItemPrice(ProductId productId, decimal newPrice, BasketDbContext ctx);
    public IO<Coupon> GetCouponById(CouponId couponId, BasketDbContext ctx);

}
