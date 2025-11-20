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
                await sender.Send(new GetProductByIdQuery(command.ProductId, false), e.Token))
                          select product;

        var combined =
            (loadCart, loadProduct).Apply((c, fin) =>
            {
                var cart = fin.Map(p => c.AddLineItems(
                    LineItem.Create(
                        ProductId.From(p.Id),
                        c.Id,
                        p.Slug,
                        p.Sku,
                        p.Images.FirstOrDefault(dto => dto.IsMain)?.Url ?? p.Images.First().Url,
                        command.Quantity,
                        p.Price
                    )));
                return cart;
            });


        var db =
            from res in combined
            from x in res.Match(cart => Db<BasketDbContext>.pure(unit), Db<BasketDbContext>.fail<Unit>)
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


