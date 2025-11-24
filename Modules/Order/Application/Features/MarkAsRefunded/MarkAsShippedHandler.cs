namespace Order.Application.Features.MarkAsRefunded;

public class MarkAsRefundedCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }

}

internal class MarkAsRefundedCommandHandler(OrderDBContext dbContext, IClock clock)
    : ICommandHandler<MarkAsRefundedCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(MarkAsRefundedCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == command.OrderId,
            NotFoundError.New($"Order with ID {command.OrderId} not found"),
            o => o.MarkAsRefunded(clock.UtcNow)
            );

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

