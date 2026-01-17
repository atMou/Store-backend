using Shared.Application.Contracts.Order.Results;

namespace Order.Application.Contracts;

public static class Extensions
{
    public static OrderResult ToResult(this Domain.Models.Order order)
    {
        return new OrderResult
        {
            OrderId = order.Id.Value,
            OrderStatus = order.OrderStatus.Name,
            UserId = order.UserId.Value,
            Subtotal = order.Subtotal,
            Total = order.Total,
            Tax = order.Tax,
            Discount = order.Discount,
            Email = order.Email,
            Phone = order.Phone,
            CouponIds = order.CouponIds.Select(c => c.Value),
            Notes = order.Notes,
            ShippingAddress = order.ShippingAddress,
            TrackingCode = order.TrackingCode.Value,
            PaymentId = order.PaymentId?.Value,
            ShipmentId = order.ShipmentId?.Value,
            OrderItems = order.OrderItems.Select(item => new OrderItemResult()
            {
                Slug = item.Slug,
                ProductId = item.ProductId.Value,
                SizeVariantId = item.SizeVariantId,
                ColorVariantId = item.ColorVariantId.Value,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ImageUrl = item.ImageUrl,
                Sku = item.Sku,
                LineTotal = item.LineTotal,
                OrderItemId = item.Id.Value,
                Size = item.Size,
                Color = item.Color
            })
        };
    }

    public static IEnumerable<OrderResult> ToResult(this IEnumerable<Domain.Models.Order> orders)
    {
        return orders.Select(o => o.ToResult());
    }

}
