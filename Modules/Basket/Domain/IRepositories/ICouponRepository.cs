using Shared.Persistence.Extensions;

namespace Basket.Domain.IRepositories;

public interface ICouponRepository
{
    IO<Coupon> GetCouponById(CouponId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null);
    IO<IEnumerable<Coupon>> GetCouponsByCartId(CartId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null);
    IO<IEnumerable<Coupon>> GetCouponsByUserId(UserId id, BasketDbContext ctx, Action<QueryOptions<Coupon>>? options = null);

    //IO<Cart> GetCartByUserId(UserId id, BasketDbContext ctx, Action<QueryOptions<Cart>>? options);

    //IO<Cart> CreateCart(Cart cart, UserId userId, BasketDbContext ctx);

    //IO<Cart> AddCartItemCart(LineItem item, CartId cartId, BasketDbContext ctx);


    //public IO<Cart> DeleteLineItem(LineItemId lineItemId, CartId cartId, BasketDbContext ctx);

    //public IO<Cart> DeleteCart(CartId cartId, BasketDbContext ctx);
    //public IO<int> UpdateCartItemPrice(ProductId productId, decimal newPrice, BasketDbContext ctx);
}
