using Order.Application.Features.CreateOrder;

using Shared.Domain.Contracts.Order;

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
            Subtotal = order.Subtotal.Value,
            Total = order.Total.Value,
            Tax = order.Tax.Value,
            Discount = order.Discount.Value,
            Email = order.Email.Value,
            Phone = order.Phone?.Value,
            CouponIds = order.CouponIds.Select(c => c.Value),
            Notes = order.Notes,
            TrackingCode = order.TrackingCode.Value,
            OrderItems = order.OrderItems.Select(item => new OrderItemResult()
            {
                ProductId = item.ProductId.Value,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
        };
    }

    public static IEnumerable<OrderResult> ToResult(this IEnumerable<Domain.Models.Order> orders)
    {
        return orders.Select(o => o.ToResult());
    }

}
