using Shared.Application.Contracts.Carts.Dtos;

namespace Basket.Domain.Contracts;

public static class Extensions
{
    public static CartDto ToDto(this Cart cart) => new()
    {
        CartId = cart.Id.Value,
        UserId = cart.UserId.Value,
        Tax = cart.TaxValue.Value,
        TotalSub = cart.TotalSub.Value,
        Discount = cart.TotalDiscount.Value,
        TotalAfterDiscounted = cart.TotalAfterDiscounted.Value,
        ShipmentCost = cart.ShipmentCost.Value,
        CouponIds = cart.CouponIds.Select(c => c.Value).ToList(),
        DeliveryAddress = cart.DeliveryAddress,
        Total = cart.Total.Value,
        LineItems = cart.LineItems.Select(li => new LineItemDto
        {
            ProductId = li.ProductId.Value,
            ColorVariantId = li.ColorVariantId.Value,
            SizeVariantId = li.SizeVariantId,
            Slug = li.Slug,
            ImageUrl = li.ImageUrl,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice.Value,
            LineTotal = li.LineTotal.Value,
            Sku = li.Sku,
            Color = li.Color,
            Size = li.Size,
        }).ToList()
    };

}

