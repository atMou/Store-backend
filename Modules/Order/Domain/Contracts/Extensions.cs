using Shared.Application.Contracts.Order.Dtos;

namespace Order.Domain.Contracts;

public static class Extensions
{
	public static OrderDto ToDto(this Models.Order order)
	{

		return new OrderDto
		{
			CartId = order.CartId,
			UserId = order.UserId,
			Total = order.Total,
			Subtotal = order.Subtotal,
			Tax = order.Tax,
			Discount = order.Discount,
			TotalAfterDiscounted = order.TotalAfterDiscounted,
			DeliveryAddress = order.ShippingAddress,
			ShipmentCost = order.ShipmentCost,
			CouponIds = order.CouponIds,
			OrderItemsDtos = order.OrderItems.Select(item => new OrderItemDto
			{
				ProductId = item.ProductId.Value,
				Sku = item.Sku,
				Slug = item.Slug,
				ImageUrl = item.ImageUrl,
				Quantity = item.Quantity,
				UnitPrice = item.UnitPrice,
				LineTotal = item.LineTotal
			})
		};
	}
}
