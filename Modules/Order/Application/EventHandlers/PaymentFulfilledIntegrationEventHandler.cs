using MassTransit;

using Shared.Application.Features.Payment.Events;

namespace Order.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
	OrderDBContext dbContext,
	ILogger<PaymentFulfilledIntegrationEventHandler> logger,
	IClock clock
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
	public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
	{
		var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
			order => order.Id == context.Message.OrderId,
			NotFoundError.New($"Order with ID {context.Message.OrderId} not found"),
			null,
			o => o.MarkAsPaid(context.Message.PaymentId, clock.UtcNow)
		).Map(_ => unit);

		var result = await db.RunAsync(dbContext, EnvIO.New(null, context.CancellationToken));
		result.IfFail(err => logger.LogError("Error marking order {orderId} as paid. {err}", context.Message.OrderId, err));
	}
}
