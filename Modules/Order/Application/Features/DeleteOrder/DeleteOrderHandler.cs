
namespace Order.Application.Features.DeleteOrder;

public class DeleteOrderCommand : ICommand<Fin<Unit>>
{
	public OrderId OrderId { get; init; }

}

internal class DeleteOrderCommandHandler(OrderDBContext dbContext)
	: ICommandHandler<DeleteOrderCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
	{
		var db =
			from o in Db<OrderDBContext>.liftIO(async (ctx, e) =>
				await ctx.Orders.FirstOrDefaultAsync(order => order.Id == command.OrderId, e.Token))
			from _1 in when(o.IsNull(),
				IO.fail<Unit>(NotFoundError.New($"Order with OrderId {command.OrderId.Value} not found.")))
			from a in o.MarkAsDeleted()
			select unit;

		return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}

