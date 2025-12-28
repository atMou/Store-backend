using Basket.Domain.Events;

using Shared.Application.Contracts.Carts.Results;

namespace Basket.Domain.Models;

public record Cart : Aggregate<CartId>
{
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
    public Discount Discount { get; private set; }
    public Money ShipmentCost { get; set; } = Money.Zero;
    public Address DeliveryAddress { get; set; }
    public Money TotalAfterDiscounted => Money.FromDecimal(Math.Max(0, TotalSub.Value - Discount.Apply(TotalSub)));
    public Money TotalDiscount => TotalSub - TotalAfterDiscounted;
    public Money TaxValue => Tax * TotalAfterDiscounted;
    public Money Total => TotalAfterDiscounted + TaxValue + ShipmentCost;
    public bool IsCheckedOut { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<CouponId> CouponIds { get; private set; } = [];
    public ICollection<LineItem> LineItems { get; private set; } = [];


    public static Fin<Cart> Create(
        UserId userId,
        decimal tax,
        Address deliveryAddress) =>
        Tax.From(tax).Map(tx => new Cart(userId, tx, deliveryAddress));

    public Cart SetCartCheckedOut()
    {
        AddDomainEvent(new CartCheckedOutDomainEvent
        {
            CartId = Id.Value,
            UserId = UserId.Value,
            Total = Total.Value,
            TotalSub = TotalSub.Value,
            Tax = TaxValue.Value,
            Discount = TotalDiscount,
            TotalAfterDiscounted = TotalAfterDiscounted.Value,
            CouponIds = CouponIds.Select(c => c.Value).ToList(),
            ShipmentCost = ShipmentCost.Value,
            DeliveryAddress = DeliveryAddress,
            LineItems = LineItems.Select(li => new LineItemResult
            {
                ProductId = li.ProductId.Value,
                Quantity = li.Quantity,
                ImageUrl = li.ImageUrl,
                LineTotal = li.LineTotal.Value,
                Slug = li.Slug,
                UnitPrice = li.UnitPrice.Value,
                VariantId = li.VariantId.Value

            }).ToList()
        });


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

    public Cart AddDiscount(CouponId couponId, Discount discount)
    {
        if (Discount.IsNull())
        {
            Discount = discount;
            return this;
        }
        var existingCoupons = CouponIds.ToHashSet();
        if (existingCoupons.Contains(couponId)) return this;
        CouponIds = [.. CouponIds, couponId];
        Discount += discount;
        return this;
    }
    public Fin<Cart> RemoveDiscount(Discount discount, CouponId couponId)
    {
        if (discount > Discount)
            return FinFail<Cart>(InvalidOperationError.New("Cannot remove more discount than applied."));
        var restDiscount = Discount - discount;
        return DeleteCouponId(couponId).Map(c =>
        {
            AddDomainEvent(new CouponRemovedFromCartDomainEvent(couponId));
            c.Discount = restDiscount;
            return c;
        });
    }

    public Cart ChangeDeliveryAddress(Address newAddress)
    {
        DeliveryAddress = newAddress;
        return this;
    }


    public Cart AddLineItem(LineItem lineItem)
    {
        //var itemsToAdd = Seq([.. ls]);
        //LineItems = itemsToAdd.Fold(LineItems, ((items, itemToAdd) =>
        //  {
        //      var existing = items.FirstOrDefault(item => item.VariantId.Value == itemToAdd.VariantId.Value);
        //      if (existing != null)
        //      {
        //          existing.AddQuantity(itemToAdd.Quantity);
        //          return items;
        //      }
        //      return [.. items, itemToAdd];
        //  }));

        LineItems = [.. LineItems, lineItem];
        TotalSub = GetSubTotal(LineItems);
        return this;
    }


    public Fin<Cart> UpdateLineItemQuantity(VariantId variantId, int quantity)
    {
        if (quantity <= 0)
        {
            return DeleteLineItem(variantId);
        }
        LineItems = LineItems.Select(li =>
            li.VariantId.Value == variantId.Value
                ? li.UpdateQuantity(quantity)
                : li).ToList();
        TotalSub = GetSubTotal(LineItems);
        return this;
    }

    public Fin<Cart> DeleteLineItem(VariantId variantId)
    {
        var itemToDelete = LineItems.FirstOrDefault(li => li.VariantId.Value == variantId.Value);

        if (itemToDelete == null)
            return FinFail<Cart>(NotFoundError.New($"Line item with variant id '{variantId.Value}' not found in cart."));

        var lineItems = LineItems.Where(i => i.VariantId.Value != variantId.Value).ToList();
        LineItems = lineItems;
        TotalSub = GetSubTotal(lineItems);
        return this;
    }

    private Money GetSubTotal(IEnumerable<LineItem> lineItems)
    {
        return lineItems.Aggregate(Money.Zero, (acc, li) => acc + li.LineTotal);
    }




    public Fin<Cart> DeleteCouponId(CouponId couponId)
    {
        if (CouponIds.All(cId => cId != couponId))
            return Fin<Cart>.Fail(
                InvalidOperationError.New($"Coupon with id '{couponId}' does not exist in your cart."));

        CouponIds = [.. CouponIds.Where(cid => cid.Value != couponId.Value)];

        return this;
    }
}