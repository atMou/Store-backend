
using Inventory.Application.Features.ReserveStock;

using MassTransit;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Application.Features.Order.Events;

namespace Inventory.Application.EventHandlers;

public class OrderCreatedIntegrationEventHandler(ISender sender, ILogger<OrderCreatedIntegrationEventHandler> logger)
	: IConsumer<OrderCreatedIntegrationEvent>
{
	public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
	{
		var orderId = context.Message.OrderId;
		var traverse = context.Message.OrderItemsDtos.AsIterable().Traverse(dto =>
			IO.liftAsync(async e => await sender.Send(new ReserveStockCommand { ProductId = ProductId.From(dto.ProductId), Quantity = dto.Quantity }))
		).As();

		var result = await traverse.RunSafeAsync(EnvIO.New(null, context.CancellationToken));

		result.Match(iterable =>
		{
			var res = iterable.Traverse(identity).As();
			res.IfFail(err => logger.LogCritical("Failed to process reservation for order {orderId}. {err}", orderId, err));
		}, err => logger.LogCritical("Failed to reserve stock for order {orderId}. {err}", orderId, err));

	}
}
