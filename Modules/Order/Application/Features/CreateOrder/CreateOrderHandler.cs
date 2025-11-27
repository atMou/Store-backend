

using MassTransit;

using Order.Domain.Contracts;

using Shared.Application.Features.Order.Events;

namespace Order.Application.Features.CreateOrder;

public class CreateOrderCommand() : ICommand<Fin<Unit>>
{
    public CreateOrderDto CreateOrderDto { get; init; }
}

internal class CreateOrderCommandHandler(OrderDBContext dbContext, IPublishEndpoint endpoint)
    : ICommandHandler<CreateOrderCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {

        var order = Domain.Models.Order.Create(command.CreateOrderDto);

        var db = AddEntity<OrderDBContext, Domain.Models.Order>(order);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async o =>
            {
                await endpoint.Publish(new OrderCreatedIntegrationEvent
                {
                    OrderDto = o.ToDto()
                }, cancellationToken);
                return unit;
            });
    }
}