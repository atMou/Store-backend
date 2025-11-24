namespace Order.Application.Features.GetOrderById;

public class GetOrderByIdCommand : ICommand<Fin<OrderResult>>
{
    public OrderId OrderId { get; init; }
}

internal class GetOrderByIdCommandHandler(OrderDBContext dbContext)
    : ICommandHandler<GetOrderByIdCommand, Fin<OrderResult>>
{
    public Task<Fin<OrderResult>> Handle(GetOrderByIdCommand command, CancellationToken cancellationToken)
    {
        var db =
            from o in GetEntity<OrderDBContext, Domain.Models.Order>(order => order.Id == command.OrderId,
                opt =>
            {
                opt.AsNoTracking = true;
                opt.AddInclude(o => o.OrderItems);
                return opt;
            }, NotFoundError.New($"Order with Id {command.OrderId} was not found"))

            select o.ToResult();

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

