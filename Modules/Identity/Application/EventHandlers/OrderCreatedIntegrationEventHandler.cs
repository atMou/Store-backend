using Identity.Application.Features.AddPendingOrder;

using Shared.Application.Features.Order.Events;

namespace Identity.Application.EventHandlers;
internal class OrderCreatedIntegrationEventHandler(ISender sender, ILogger<OrderCreatedIntegrationEventHandler> logger) : IConsumer<OrderCreatedIntegrationEvent>
{
	public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
	{
		var orderId = context.Message.OrderId;
		var userId = context.Message.UserId;
		var results = await sender.Send(new AddPendingOrderCommand
		{
			OrderId = OrderId.From(orderId),
			UserId = UserId.From(userId)
		});

		results.IfFail(err => logger.LogCritical("Failed to add pending order. {err}", err));
	}
}
