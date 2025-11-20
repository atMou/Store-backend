namespace Basket.Domain.Contracts;

public static class Extensions
{
    public static CartDto ToDto(this Models.Cart cart, IEnumerable<Coupon> coupons)
    {
        return new CartDto
        {
            CartId = cart.Id.Value,
            UserId = cart.UserId.Value,
            Tax = cart.TaxValue.Value,
            Total = cart.Total.Value,
            TotalSub = cart.TotalSub.Value,
            Discount = cart.Discount.Value,
            TotalDiscounted = cart.TotalDiscounted.Value,
            LineItems = cart.LineItems.ToDto(),
            Coupons = coupons.ToDto()
        };
    }

    public static CouponDto ToDto(this Coupon coupon)
    {
        return new CouponDto
        {
            Id = coupon.Id.Value,
            Code = coupon.Code,
            Description = coupon.Description,
            DiscountValue = coupon.Discount.DiscountValue,
            MinimumPurchaseAmount = coupon.MinimumPurchaseAmount,
            DiscountType = coupon.Discount.DiscountType.ToString(),
            ExpiryDate = coupon.ExpiryDate.Value,
            Status = coupon.Status.Name,
            IsDeleted = coupon.IsDeleted,
            UserId = coupon.UserId?.Value,
            CartId = coupon.CartId?.Value,

        };
    }

    public static IEnumerable<CouponDto> ToDto(this IEnumerable<Coupon> coupons)
    {
        return coupons.Select(coupon => coupon.ToDto());
    }


    public static LineItemDto ToDto(this LineItem lineItem)
    {
        return new LineItemDto
        {
            ProductId = lineItem.ProductId.Value,
            Slug = lineItem.Slug,
            ImageUrl = lineItem.ImageUrl,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Value,
            LineTotal = lineItem.LineTotal.Value,
        };
    }

    public static IEnumerable<LineItemDto> ToDto(this IEnumerable<LineItem> lineItems)
    {
        return lineItems.Select(lineItem => lineItem.ToDto());
    }


}
