using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Contracts.Order.Results;

namespace Order.Application.Features.GetOrderById;

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

