using Basket.Enums;

using Shared.Domain.Abstractions;
using Shared.Domain.Errors;

namespace Basket.Domain.Models;

public record Cart : Aggregate<CartId>
{
    private Cart(UserId userId, Coupon coupon, double taxRate)
        : base(CartId.New)
    {
        UserId = userId;
        TaxRate = taxRate;
        Coupons = [coupon];
    }

    private Cart(UserId userId, double taxRate)
        : base(CartId.New)
    {
        UserId = userId;
        TaxRate = taxRate;
    }

    public UserId UserId { get; }
    public double TaxRate { get; }
    public bool IsCheckedOut { get; private set; } = true;
    public ICollection<CartItem> CartItems { get; private init; } = [];

    public ICollection<Coupon> Coupons { get; private init; } = [];
    public Money Total => CartItems.Sum(i => i.LineTotal.Value);
    public Money TotalTax => (TotalAfterDiscount * TaxRate);

    public Money TotalAfterDiscount => Coupons.Aggregate(Money.Zero, (acc, c) => acc + GetSubTotal(c));

    public Money TotalDiscount => Total - TotalAfterDiscount;

    private Money GetSubTotal(Coupon coupon)
    {
        var total = Money.FromDecimal(CartItems.Sum(i => i.LineTotal.Value));
        return coupon.Discount.DiscountType switch
        {
            DiscountType.Percentage => total * (1 - coupon.Discount.DiscountValue),
            DiscountType.Amount => Money.FromDecimal(Math.Max(0, (total - coupon.Discount.DiscountValue).Value)),
            _ => total
        };
    }

    public static Cart Create(
        UserId userId,
        double taxRate,
        Coupon? coupon = null)
    {
        return coupon is not null ? new Cart(userId, coupon, taxRate) : new Cart(userId, taxRate);
    }

    public Cart AddCartItems(params CartItem[] items)
    {
        return this with { CartItems = [.. CartItems, .. items] };
    }

    public Cart AddCartItem(CartItem item)
    {
        var itemExists = CartItems.Any(i => i.ProductId.Value == item.ProductId.Value);
        if (itemExists)
        {
            var updatedItems = CartItems.Select(i =>
                i.ProductId.Value == item.ProductId.Value
                            ? i.UpdateQuantity(i.Quantity + item.Quantity)
                    : i).ToList();
            return this with { CartItems = updatedItems };
        }
        return this with { CartItems = [.. CartItems, item] };
    }

    public Cart DeleteCartItem(CartItem item)
    {
        return this with { CartItems = [.. CartItems.Where(i => i.Id != item.Id)] };
    }

    public Cart DeleteCartItem(CartItemId cartItemId)
    {
        return this with { CartItems = [.. CartItems.Where(i => i.Id != cartItemId)] };
    }


    public Fin<Cart> AddCoupon(Coupon coupon)
    {
        if (Coupons.Any(c => c.Id == coupon.Id))
        {
            return Fin<Cart>.Fail(InvalidOperationError.New($"Coupon with id '{coupon.Id}' is already applied to the cart."));
        }

        return this with
        {
            Coupons = [.. Coupons, coupon],
        };
    }
    public Cart SetCartCheckedOut(bool isCheckedOut)
    {
        return this with { IsCheckedOut = !isCheckedOut };
    }




}