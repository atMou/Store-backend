namespace Order.Domain.Contracts;

public static class Extensions
{
    public static OrderDto ToDto(this Models.Order order)
    {

        return new OrderDto
        {
            CartId = order.CartId.Value,
            UserId = order.UserId.Value,
            Total = order.Total,
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Discount = order.Discount,
            TotalAfterDiscounted = order.TotalAfterDiscounted,
            DeliveryAddress = order.ShippingAddress,
            ShipmentCost = order.ShipmentCost,
            CouponIds = order.CouponIds.Select(id => id.Value),
            OrderId = order.Id.Value,

            OrderItemsDtos = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId.Value,
                Sku = item.Sku,
                Slug = item.Slug,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal,
                SizeVariantId = item.SizeVariantId,
                Size = item.Size,
                ColorVariantId = item.ColorVariantId.Value,
                OrderItemId = item.Id.Value,



            })
        };
    }
}
