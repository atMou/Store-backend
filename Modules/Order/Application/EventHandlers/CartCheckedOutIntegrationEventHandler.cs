using MassTransit;

using Order.Application.Features.CreateOrder;
using Order.Domain.Contracts;

using Shared.Application.Features.Cart.Events;

namespace Order.Application.EventHandlers;

public class CartCheckedOutIntegrationEventHandler(ISender sender) : IConsumer<CartCheckedOutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CartCheckedOutIntegrationEvent> context)
    {
        await sender.Send(new CreateOrderCommand
        {
            CreateOrderDto = new CreateOrderDto()
            {
                UserId = UserId.From(context.Message.UserId),
                CartId = CartId.From(context.Message.CartId),
                Total = context.Message.Total,
                Subtotal = context.Message.TotalSub,
                Tax = context.Message.Tax,
                Discount = context.Message.Discount,
                TotalAfterDiscounted = context.Message.TotalAfterDiscounted,
                ShipmentCost = context.Message.ShipmentCost,
                DeliveryAddress = context.Message.DeliveryAddress,
                CouponIds = context.Message.CouponIds.Select(CouponId.From),
                OrderItems = context.Message.LineItems.Select(item => new CreateOrderItemDto()
                {
                    ProductId = ProductId.From(item.ProductId),
                    Slug = item.Slug,
                    ImageUrl = item.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                })
            }
        });
    }
}

