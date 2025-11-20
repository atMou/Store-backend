
namespace Order.Application.Features.CreateOrder;

public class CreateOrderCommand : ICommand<Fin<CreateOrderResult>>
{
    public Guid UserId { get; set; }
    public IEnumerable<CreateOrderItemDto> OrderItemsDtos { get; set; }
}

internal class CreateOrderCommandHandler(OrderDBContext dbContext)
    : ICommandHandler<CreateOrderCommand, Fin<CreateOrderResult>>
{
    public Task<Fin<CreateOrderResult>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
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
            UserId.From(command.UserId),
            items
        ));

        var db =
            from o in order
            from _ in Db<OrderDBContext>.lift(ctx =>
                ctx.Orders.Add(o))
            select new CreateOrderResult
            {
                OrderId = o.Id.Value,
                OrderItems = o.OrderItems.Select(item => new CreateOrderItemResult
                {
                    ProductId = item.ProductId.Value,
                    Sku = item.Sku,
                    Slug = item.Slug,
                    ImageUrl = item.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice.Value
                }).ToList()
            };

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}