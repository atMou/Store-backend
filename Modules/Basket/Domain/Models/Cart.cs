using Basket.Domain.Events;

using Shared.Application.Contracts.Carts.Results;

namespace Basket.Domain.Models;

public record Cart : Aggregate<CartId>
{

    private Cart(UserId userId, Tax tax, Money shipmentCost, Address deliveryAddress)
        : base(CartId.New)
    {
        UserId = userId;
        Tax = tax;
        ShipmentCost = shipmentCost;
        DeliveryAddress = deliveryAddress;
        AddDomainEvent(new CartCreatedDomainEvent(Id, userId));
    }

    public UserId UserId { get; private init; }
    public Tax Tax { get; private init; }
    public Money TotalSub { get; private set; } = Money.Zero;
    public Money Discount { get; private set; } = Money.Zero;
    public Money ShipmentCost { get; private set; } = Money.Zero;
    public Address DeliveryAddress { get; set; } = null!;
    public Money TotalAfterDiscounted => Money.FromDecimal(Math.Max(0, TotalSub.Value - Discount.Value));
    public Money TaxValue => Tax * TotalAfterDiscounted;
    public Money Total => TotalAfterDiscounted + TaxValue + ShipmentCost;
    public bool IsCheckedOut { get; private set; }
    public bool IsActive { get; private set; } = true;
    public IEnumerable<CouponId> CouponIds { get; private set; } = [];
    public List<LineItem> LineItems { get; private init; } = [];


    public static Fin<Cart> Create(
        UserId userId,
        decimal tax,
        decimal shipmentCost,
        Address deliveryAddress) =>
        Tax.From(tax).Map(tx => new Cart(userId, tx, shipmentCost, deliveryAddress));

    public Cart SetCartCheckedOut()
    {
        AddDomainEvent(new CartCheckedOutDomainEvent
        {
            CartId = Id.Value,
            UserId = UserId.Value,
            Total = Total.Value,
            TotalSub = TotalSub.Value,
            Tax = TaxValue.Value,
            Discount = Discount.Value,
            TotalAfterDiscounted = TotalAfterDiscounted.Value,
            CouponIds = CouponIds.Select(c => c.Value).ToList(),
            ShipmentCost = ShipmentCost.Value,
            DeliveryAddress = DeliveryAddress,
            LineItems = LineItems.Select(li => new LineItemResult
            {
                ProductId = li.ProductId.Value,
                Quantity = li.Quantity
            }).ToList()
        });


        return this with { IsCheckedOut = true };
    }


    public Fin<Cart> AddDiscount(CouponId couponId, decimal discount)
    {
        return AddCouponIds(couponId)
            .Map(c => c with { Discount = Discount + discount });
    }

    public Fin<Cart> SetIsActiveToFalse()
    {
        return IsActive is false ?
            FinFail<Cart>(InvalidOperationError.New("Cart is already inactive.")) :
           this with { IsActive = false };
    }
    public Fin<Cart> RemoveDiscount(decimal discount, CouponId couponId)
    {
        if (discount > Discount)
        {
            return FinFail<Cart>(InvalidOperationError.New($"Cannot remove more discount than applied."));
        }
        var restDiscount = Discount - discount;
        return DeleteCouponId(couponId).Map(c =>
        {
            AddDomainEvent(new CouponRemovedFromCartDomainEvent(couponId));
            return c with { Discount = restDiscount };
        });

    }

    public Cart ChangeDeliveryAddress(Address newAddress)
    {
        return this with { DeliveryAddress = newAddress };
    }
    private List<LineItem> AddLineItem(LineItem item) =>
        LineItems.Select(li =>
            li.ProductId.Value == item.ProductId.Value
                ? li.AddQuantity(item.Quantity)
                : li).ToList();

    public Cart AddLineItems(params LineItem[] ls)
    {
        List<LineItem> lineItems = [];
        foreach (var li in ls)
        {
            lineItems = AddLineItem(li);
        }


        return this with
        {
            LineItems = lineItems,
            TotalSub = GetSubTotal(lineItems)
        };
    }


    public Cart UpdateLineItem(LineItem item)
    {
        var lineItems = LineItems.Select(li =>
            li.ProductId.Value == item.ProductId.Value
                ? item
                : li).ToList();
        return this with { LineItems = lineItems, TotalSub = GetSubTotal(lineItems) };
    }

    public Cart DeleteLineItem(ProductId productId)
    {
        var lineItems = LineItems.Where(i => i.ProductId != productId).ToList();
        return this with { LineItems = lineItems, TotalSub = GetSubTotal(lineItems) };
    }

    private Money GetSubTotal(IEnumerable<LineItem> lineItems)
    {
        return lineItems.Aggregate(Money.Zero, (acc, li) => acc + li.LineTotal);
    }

    //private Fin<Cart> AddCouponId(CouponId couponId)
    //{
    //    if (CouponIds.Any(cId => cId == couponId))
    //    {
    //        return Fin<Cart>.Fail(InvalidOperationError.New($"Coupon with id '{couponId}' is already in your cart."));
    //    }

    //    return this with { CouponIds = [.. CouponIds, couponId] };
    //}


    private Fin<Cart> AddCouponIds(params CouponId[] couponIds)
    {
        var existingCoupons = CouponIds.ToHashSet();

        foreach (var couponId in couponIds)
        {
            if (existingCoupons.Contains(couponId))
            {
                return Fin<Cart>.Fail(
                    InvalidOperationError.New($"Coupon with id '{couponId}' is already in your cart."));
            }
        }


        return this with { CouponIds = [.. CouponIds, .. couponIds] };
    }

    public Fin<Cart> DeleteCouponId(CouponId couponId)
    {

        if (CouponIds.All(cId => cId != couponId))
        {
            return Fin<Cart>.Fail(
                InvalidOperationError.New($"Coupon with id '{couponId}' does not exist in your cart."));
        }

        CouponIds = [.. CouponIds.Where(cid => cid.Value != couponId.Value)];

        return this;
    }
}