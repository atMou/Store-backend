using Shared.Application.Contracts.Carts.Results;

namespace Basket.Domain.Contracts;

public static class Extensions
{
    public static CartResult ToResult(this Cart cart, IEnumerable<Coupon> coupons)
    {
        return new CartResult
        {
            CartId = cart.Id.Value,
            UserId = cart.UserId.Value,
            Tax = cart.TaxValue.Value,
            Total = cart.Total.Value,
            TotalSub = cart.TotalSub.Value,
            Discount = cart.Discount.Value,
            TotalDiscounted = cart.TotalAfterDiscounted.Value,
            LineItems = cart.LineItems.ToResult(),
            Coupons = coupons.ToResult()
        };
    }

    public static CouponResult ToResult(this Coupon coupon)
    {
        return new CouponResult
        {
            Id = coupon.Id.Value,
            Code = coupon.Code,
            Description = coupon.Description.Value,
            DiscountValue = coupon.Discount.DiscountValue,
            MinimumPurchaseAmount = coupon.MinimumPurchaseAmount,
            DiscountType = coupon.Discount.DiscountType.ToString(),
            ExpiryDate = coupon.ExpiryDate.Value,
            Status = coupon.CouponStatus.Name,
            UserId = coupon.UserId?.Value,
            CartId = coupon.CartId?.Value,

        };
    }

    public static IEnumerable<CouponResult> ToResult(this IEnumerable<Coupon> coupons)
    {
        return coupons.Select(coupon => coupon.ToResult());
    }


    public static LineItemResult ToResult(this LineItem lineItem)
    {
        return new LineItemResult
        {
            ProductId = lineItem.ProductId.Value,
            Slug = lineItem.Slug,
            ImageUrl = lineItem.ImageUrl,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Value,
            LineTotal = lineItem.LineTotal.Value,
        };
    }

    public static IEnumerable<LineItemResult> ToResult(this IEnumerable<LineItem> lineItems)
    {
        return lineItems.Select(lineItem => lineItem.ToResult());
    }


}
