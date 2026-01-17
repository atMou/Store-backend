
namespace Order.Application.EventHandlers;

public class CartCheckedOutIntegrationEventHandler(
    IPublishEndpoint endpoint,
    OrderDBContext dbContext,
    ILogger<CartCheckedOutIntegrationEventHandler> logger)
    : IConsumer<CartCheckedOutIntegrationEvent>
{

    public async Task Consume(ConsumeContext<CartCheckedOutIntegrationEvent> context)
    {
        var db = AddEntityIfNotExists<OrderDBContext, Domain.Models.Order, CreateOrderDto>(
            o => o.CartId == CartId.From(context.Message.CartDto.CartId),
            ConflictError.New($"Order with cart id {context.Message.CartDto.CartId} already exists."),
            GetCreateOrderDto(context.Message),
            Domain.Models.Order.Create);

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnSuccess(async order => await endpoint.Publish(new OrderCreatedIntegrationEvent() { OrderDto = order.ToDto() }, context.CancellationToken));

        result.IfFail(err => logger.LogError(LogEvents.OrderCreateFail, err, "Failed to create order"));
    }


    private CreateOrderDto GetCreateOrderDto(CartCheckedOutIntegrationEvent e)
    {
        var message = e.CartDto;
        return new()
        {
            UserId = UserId.From(message.UserId),
            CartId = CartId.From(message.CartId),
            Total = message.Total,
            Subtotal = message.TotalSub,
            Tax = message.Tax,
            Discount = message.Discount,
            TotalAfterDiscounted = message.TotalAfterDiscounted,
            ShipmentCost = message.ShipmentCost,
            DeliveryAddress = message.DeliveryAddress,
            CouponIds = message.CouponIds.Select(CouponId.From),
            OrderItems = message.LineItems.Select(item => new CreateOrderItemDto()
            {
                ProductId = ProductId.From(item.ProductId),
                ColorVariantId = ColorVariantId.From(item.ColorVariantId),
                SizeVariantId = item.SizeVariantId,
                Slug = item.Slug,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal,
                Sku = item.Sku,
                Color = item.Color,
                Size = item.Size
            })
        };
    }

}

