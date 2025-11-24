using Shared.Application.Contracts.Carts.Results;

namespace Basket.Domain.Models;

public record Cart : Aggregate<CartId>
{

    private Cart(UserId userId, Tax tax, Money shipmentCost)
        : base(CartId.New)
    {
        UserId = userId;
        Tax = tax;
        ShipmentCost = shipmentCost;
        AddDomainEvent(new CartCreatedDomainEvent(Id, userId));
    }

    public UserId UserId { get; }
    public Tax Tax { get; }
    public bool IsCheckedOut { get; private set; }
    public Money TotalSub { get; private set; } = Money.Zero;
    public Money Discount { get; private set; } = Money.Zero;
    public Money ShipmentCost { get; private set; }

    public Money TotalDiscounted => Money.FromDecimal(Math.Max(0, TotalSub.Value - Discount.Value));
    public Money TaxValue => Tax * TotalDiscounted;
    public Money Total => TotalDiscounted + TaxValue;
    public IEnumerable<CouponId> CouponIds { get; private set; } = [];
    public List<LineItem> LineItems { get; private set; } = [];


    public static Fin<Cart> Create(UserId userId, decimal tax, decimal shipmentCost) =>
        Tax.From(tax).Map(tx => new Cart(userId, tx, shipmentCost));

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
            TotalDiscounted = TotalDiscounted.Value,
            CouponIds = CouponIds.Select(c => c.Value).ToList(),
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

    public Fin<(decimal RestDiscount, Cart Cart)> RemoveDiscount(decimal discount, CouponId couponId)
    {
        var rest = Math.Abs((discount - (decimal)Discount));

        return DeleteCouponId(couponId).Map(c => (rest, c with { Discount = Math.Min(0M, (Discount + discount)) }));

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

    private Fin<Cart> AddCouponId(CouponId couponId)
    {
        if (CouponIds.Any(cId => cId == couponId))
        {
            return Fin<Cart>.Fail(InvalidOperationError.New($"Coupon with id '{couponId}' is already in your cart."));
        }

        return this with { CouponIds = [.. CouponIds, couponId] };
    }


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





