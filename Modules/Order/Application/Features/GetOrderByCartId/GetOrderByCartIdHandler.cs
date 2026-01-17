using Order.Persistence;

using Shared.Application.Contracts.Order.Results;

namespace Order.Application.Features.GetOrderByCartId;

public record GetOrderByCartIdQuery : ICommand<Fin<OrderResult>>
{
    public CartId CartId { get; init; }
}
internal class GetOrderByCartIdCommandHandler(OrderDBContext dbContext)
    : ICommandHandler<GetOrderByCartIdQuery, Fin<OrderResult>>
{
    public Task<Fin<OrderResult>> Handle(GetOrderByCartIdQuery command, CancellationToken cancellationToken)
    {
        var db =
            from o in GetEntity<OrderDBContext, Domain.Models.Order>(
                order => order.CartId == command.CartId,
                NotFoundError.New($"Order with CartId {command.CartId} was not found"),
                opt =>
                {
                    opt.AsNoTracking = true;
                    opt = opt.AddInclude(o => o.OrderItems);
                    return opt;
                }
            )

            select o.ToResult();

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

