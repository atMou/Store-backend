using Basket.Domain.Models;

using Shared.Domain.Contracts.Cart;

namespace Basket.Domain.Contracts;

public static class Extensions
{
    public static CartDto ToDto(this Cart cart)
    {
        return new CartDto
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            TotalAfterDiscount = cart.TotalAfterDiscount.Value,
            TotalTax = cart.TotalTax.Value,
            Total = cart.Total.Value,
            TotalDiscount = cart.TotalDiscount.Value,
            LineItems = cart.CartItems.Select(i => new CartLineItemDto
            {
                ProductId = i.ProductId,
                Sku = i.Sku,
                Slug = i.Slug,
                ImageUrl = i.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Value,
                LineTotal = i.LineTotal.Value,
            }).ToList()
        };
    }
}
