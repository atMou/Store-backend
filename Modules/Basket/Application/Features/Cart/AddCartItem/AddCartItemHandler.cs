namespace Basket.Application.Features.Cart.AddCartItem;

public record AddLineItemCommand : ICommand<Fin<Unit>>
{
    public CartId CartId { get; init; }
    public ProductId ProductId { get; init; }
    public int Quantity { get; init; }
}

internal class AddLineItemCommandHandler(
    ISender sender,
    BasketDbContext dbContext,
    ICartRepository cartRepository
) : ICommandHandler<AddLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var loadCart = from cart in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.GetCartById(command.CartId, ctx, opts =>
                {
                    opts.AsSplitQuery = true;
                    opts.AddInclude(cart => cart.LineItems);
                }))
                       select cart;

        var loadProduct = from product in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductByIdQuery(command.ProductId), e.Token))
                          select product;

        var db =
            from t in (loadCart, loadProduct).Apply((c, product) => (c, fin: product))
            from cart in t.fin.Map(p => t.c.AddLineItems(
            LineItem.Create(
                ProductId.From(p.Id),
                t.c.Id,
                p.Slug,
                p.Sku,
                p.Images.FirstOrDefault(dto => dto.IsMain)?.Url ?? p.Images.First().Url,
                command.Quantity,
                p.Price
            )))
            from a in Db<BasketDbContext>.lift(ctx =>
                {
                    ctx.Carts.Entry(t.c).CurrentValues.SetValues(cart);
                    return unit;
                })
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


