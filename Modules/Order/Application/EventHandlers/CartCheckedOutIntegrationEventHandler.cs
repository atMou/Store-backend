using MassTransit;

using Order.Application.Features.CreateOrder;

using Shared.Messaging.Events;

namespace Order.Application.EventHandlers;

public class CartCheckedOutIntegrationEventHandler(ISender sender) : IConsumer<CartCheckedOutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CartCheckedOutIntegrationEvent> context)
    {
        await sender.Send(new CreateOrderCommand
        {
            UserId = context.Message.UserId,
            OrderItemsDtos = context.Message.LineItems.Select(item => new CreateOrderItemDto
            {
                ProductId = ProductId.From(item.ProductId),
                Slug = item.Slug,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            })
        });
    }
}
