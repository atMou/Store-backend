namespace Basket.Application.Features.Cart.UpdateCartItem;

public record UpdateLineItemCommand : ICommand<Fin<Unit>>
{
    public CartId CartId { get; init; }
    public ProductId ProductId { get; init; }
    public int Quantity { get; init; }
}

internal class UpdateLineItemCommandHandler(
    ISender sender,
    BasketDbContext dbContext
) : ICommandHandler<UpdateLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpdateLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var loadCart = GetEntity<BasketDbContext, Domain.Models.Cart>(
            cart => cart.Id == command.CartId,
            opt =>
            {
                opt.AsSplitQuery = true;
                opt.AddInclude(cart => cart.LineItems);
                return opt;
            },
            NotFoundError.New($"Cart with Id {command.CartId.Value} not found."));

        var loadProduct = from product in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductByIdQuery(command.ProductId), e.Token))
                          select product;

        var db =
            from t in (loadCart, loadProduct).Apply((c, product) => (c, fin: product))
            from cart in t.fin.Map(p => t.c.UpdateLineItem(
            LineItem.Create(
                ProductId.From(p.Id),
                t.c.Id,
                p.Slug,
                p.Sku,
                p.Images.FirstOrDefault(dto => dto.IsMain)?.Url ?? p.Images.First().Url,
                command.Quantity,
                p.Price
            )))
            from _ in UpdateEntity<BasketDbContext, Domain.Models.Cart>(t.c, _ => cart)
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


