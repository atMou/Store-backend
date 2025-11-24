
namespace Order.Application.Features.MarkAsPaid;

public class MarkAsPaidCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }
    public PaymentId PaymentId { get; init; }

}

internal class MarkAsPaidCommandHandler(OrderDBContext dbContext, IClock clock)
    : ICommandHandler<MarkAsPaidCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(MarkAsPaidCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == command.OrderId,
            NotFoundError.New($"Order with ID {command.OrderId} not found"),
            o => o.MarkAsPaid(command.PaymentId, clock.UtcNow)
            );

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

