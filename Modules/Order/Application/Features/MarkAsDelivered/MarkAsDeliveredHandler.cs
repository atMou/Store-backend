namespace Order.Application.Features.MarkAsDelivered;

public class MarkAsDeliveredCommand : ICommand<Fin<Unit>>
{
	public OrderId OrderId { get; init; }

}

internal class MarkAsDeliveredCommandHandler(OrderDBContext dbContext, IClock clock)
	: ICommandHandler<MarkAsDeliveredCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(MarkAsDeliveredCommand command, CancellationToken cancellationToken)
	{
		var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
			order => order.Id == command.OrderId,
				NotFoundError.New($"Order with ID {command.OrderId} not found"),
			null,
			o => o.MarkAsDelivered(clock.UtcNow)
			).Map(_ => unit);

		return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}

