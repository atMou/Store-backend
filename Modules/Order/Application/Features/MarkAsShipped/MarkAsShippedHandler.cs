

namespace Order.Application.Features.MarkAsShipped;

public class MarkAsShippedCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }
    public ShipmentId ShipmentId { get; init; }

}

internal class MarkAsShippedCommandHandler(OrderDBContext dbContext, IClock clock)
    : ICommandHandler<MarkAsShippedCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(MarkAsShippedCommand command, CancellationToken cancellationToken)
    {

        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == command.OrderId,
            NotFoundError.New($"Order with ID {command.OrderId} not found"),
            o => o.MarkAsShipped(command.ShipmentId, clock.UtcNow));

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

