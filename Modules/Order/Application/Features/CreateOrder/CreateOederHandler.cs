

namespace Order.Application.Features.CreateOrder;

public class CreateOrderCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public Money Subtotal { get; init; }
    public Money Total { get; init; }
    public Money Tax { get; init; }
    public Money Discount { get; set; }
    public IEnumerable<CreateOrderItemDto> OrderItemsDtos { get; init; }
}

internal class CreateOrderCommandHandler(OrderDBContext dbContext)
    : ICommandHandler<CreateOrderCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderItems = command.OrderItemsDtos.AsIterable().Traverse(item =>
            OrderItem.Create(
                item.ProductId,
                item.Slug,
                item.Sku,
                item.ImageUrl,
                item.Quantity,
                item.UnitPrice
            )
        ).Map(items => items.AsEnumerable()).As();

        var order = orderItems.Bind(items => Domain.Models.Order.Create(
            command.UserId,
            items,
            command.Subtotal,
            command.Total,
            command.Tax,
            command.Discount


        ));


        var db =
            from o in order
            from _ in Db<OrderDBContext>.lift(ctx =>
                ctx.Orders.Add(o))
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

