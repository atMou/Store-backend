namespace Order.Application.Features.MarkAsCancelled;

public class MarkAsCancelledCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }

}

internal class MarkAsCancelledCommandHandler(OrderDBContext dbContext, IClock clock)
    : ICommandHandler<MarkAsCancelledCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(MarkAsCancelledCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
           order => order.Id == command.OrderId,
           NotFoundError.New($"Order with ID {command.OrderId} not found"),
           null,
           o => o.MarkAsCancelled(clock.UtcNow)
       ).Map(_ => unit);

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

