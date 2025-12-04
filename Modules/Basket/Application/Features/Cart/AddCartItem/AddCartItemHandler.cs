namespace Basket.Application.Features.Cart.AddCartItem;

public record AddLineItemCommand : ICommand<Fin<Unit>>
{
    public CartId CartId { get; init; }
    public ProductId ProductId { get; init; }
    public VariantId VariantId { get; init; }
    public int Quantity { get; init; }
}

internal class AddLineItemCommandHandler(
    ISender sender,
    BasketDbContext dbContext
) : ICommandHandler<AddLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var loadCart = GetEntity<BasketDbContext, Domain.Models.Cart>(
            cart => cart.Id == command.CartId,
            NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
            opt =>
            {
                opt.AsSplitQuery = true;
                opt.AddInclude(cart => cart.LineItems);
                return opt;
            });

        var loadProduct =
            from product in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductByIdQuery(command.ProductId, $"{Variants}"), e.Token))
            select product;

        var db =
            from t in (loadCart, loadProduct).Apply((c, product) => (c, product))
            from v in t.product.Bind(p =>
            {
                return Optional(p.Variants.FirstOrDefault(v => v.Id == command.VariantId.Value)).ToFin(
                    NotFoundError.New($"Variant with Id {command.VariantId.Value} not found in Product {p.Id}."));
            })
            from cart in t.product.Map(p => t.c.AddLineItems(
                LineItem.Create(
                    ProductId.From(p.Id),
                    t.c.Id,
                    command.VariantId,
                    p.Slug,
                    v.Sku,
                    v.Images.FirstOrDefault(dto => dto.IsMain)?.Url ?? p.Images.First().Url,
                    command.Quantity,
                    p.Price
                )))
            from _ in UpdateEntity<BasketDbContext, Domain.Models.Cart>(t.c, _ => cart)
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}