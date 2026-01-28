using System.ComponentModel.DataAnnotations.Schema;

namespace Basket.Domain.Models;

public class Cart : Aggregate<CartId>
{
    private const decimal FreeShippingThreshold = 29.50m;
    private const decimal StandardShippingCost = 4.50m;

    private Cart() : base(CartId.New)
    {
    }

    private Cart(UserId userId, Tax tax, Address deliveryAddress)
        : base(CartId.New)
    {
        UserId = userId;
        Tax = tax;
        DeliveryAddress = deliveryAddress;

    }

    public UserId UserId { get; }
    public Tax Tax { get; }
    public Money TotalSub { get; private set; } = Money.Zero;
    public Money ShipmentCost { get; private set; } = Money.Zero;
    public Address DeliveryAddress { get; private set; }
    public Money TotalAfterDiscounted => TotalSub - TotalDiscount;

    public Money TotalDiscount { get; private set; } = Money.Zero;

    public Money TaxValue => Tax * TotalAfterDiscounted;
    public Money Total => TotalAfterDiscounted + TaxValue + ShipmentCost;
    public bool IsCheckedOut { get; private set; }
    public bool IsActive { get; private set; } = true;

    [NotMapped]
    public List<Discount> Discounts { get; private set; } = [];
    public ICollection<CouponId> CouponIds { get; private set; } = new List<CouponId>();
    public ICollection<LineItem> LineItems { get; private set; } = new List<LineItem>();


    public static Fin<Cart> Create(
        UserId userId,
        decimal tax,
        Address deliveryAddress) =>
        Tax.From(tax).Map(tx => new Cart(userId, tx, deliveryAddress));
    private Money RecalculateTotalDiscount()
    {
        return Discounts.Aggregate(Money.Zero, (totalDiscount, discount) => 
            totalDiscount + Money.FromDecimal(discount.Apply(TotalSub)));
    }

    public Cart SetCartCheckedOut()
    {
        IsCheckedOut = true;
        return this;
    }

    public Fin<Cart> SetIsActiveToFalse()
    {
        if (!IsActive)
        {
            return FinFail<Cart>(InvalidOperationError.New("Cart is already inactive."));
        }
        IsActive = false;
        return this;
    }

    public Fin<Cart> AddDiscount(CouponId couponId, Discount discount)
    {

        if (CouponIds.Contains(couponId)) return FinFail<Cart>(InvalidOperationError.New("Coupon is already applied."));
        CouponIds.Add(couponId);
        Discounts.Add(discount);
        TotalDiscount = RecalculateTotalDiscount();
        ShipmentCost = UpdateShippingCost();
        return this;
    }

    public Fin<Cart> RemoveDiscount(Discount discount, CouponId couponId)
    {
        return DeleteCouponId(couponId).Map(c =>
        {
            c.Discounts.Remove(discount);
            c.TotalDiscount = c.RecalculateTotalDiscount();
            c.ShipmentCost = c.UpdateShippingCost();
            return c;
        });
    }

    public Cart ChangeDeliveryAddress(Address newAddress)
    {
        DeliveryAddress = newAddress;
        return this;
    }

    public Cart LoadDiscountsFromCoupons(IEnumerable<Coupon> coupons)
    {
        Discounts = coupons.Select(c => c.Discount).ToList();
        TotalDiscount = RecalculateTotalDiscount();
        return this;
    }

    public Cart AddLineItems(params LineItem[] lineItems)
    {
        foreach (var lineItem in lineItems)
        {
            LineItems.Add(lineItem);
        }
        TotalSub = GetSubTotal(LineItems);
        TotalDiscount = RecalculateTotalDiscount();
        ShipmentCost = UpdateShippingCost();
        return this;
    }


    public Fin<Cart> UpdateLineItemQuantity(ColorVariantId colorVariantId, Guid sizeVariant, int quantity)
    {
        if (quantity <= 0)
        {
            return DeleteLineItem(colorVariantId);
        }
        LineItems = LineItems.Select(li =>
            li.ColorVariantId.Value == colorVariantId.Value && li.SizeVariantId == sizeVariant
                ? li.UpdateQuantity(quantity)
                : li).ToList();
        TotalSub = GetSubTotal(LineItems);
        TotalDiscount = RecalculateTotalDiscount();
        ShipmentCost = UpdateShippingCost();
        return this;
    }

    public Fin<Cart> DeleteLineItem(ColorVariantId colorVariantId)
    {
        var itemToDelete = LineItems.FirstOrDefault(li => li.ColorVariantId.Value == colorVariantId.Value);

        if (itemToDelete == null)
            return FinFail<Cart>(NotFoundError.New($"Line item with variant id '{colorVariantId.Value}' not found in cart."));

        var lineItems = LineItems.Where(i => i.ColorVariantId.Value != colorVariantId.Value).ToList();
        LineItems = lineItems;
        TotalSub = GetSubTotal(lineItems);
        TotalDiscount = RecalculateTotalDiscount();
        ShipmentCost = UpdateShippingCost();
        return this;
    }

    private Money GetSubTotal(IEnumerable<LineItem> lineItems)
    {
        return lineItems.Aggregate(Money.Zero, (acc, li) => acc + li.LineTotal);
    }

    private Money UpdateShippingCost()
    {
        return ShipmentCost = TotalAfterDiscounted.Value >= FreeShippingThreshold
            ? Money.Zero
            : Money.FromDecimal(StandardShippingCost);
    }

    private Fin<Cart> DeleteCouponId(CouponId couponId)
    {
        return Optional(CouponIds.FirstOrDefault(id => id == couponId))
             .ToFin(InvalidOperationError.New($"Coupon with id '{couponId}' does not exist in your cart."))
             .Map(id =>
             {
                 CouponIds.Remove(couponId);
                 return this;
             });

    }
}