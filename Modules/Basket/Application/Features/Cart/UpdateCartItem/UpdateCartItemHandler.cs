using Shared.Application.Contracts.Product.Results;

namespace Basket.Application.Features.Cart.UpdateCartItem;

public record UpdateLineItemCommand : ICommand<Fin<Unit>>
{
    public CartId CartId { get; init; }
    public ProductId ProductId { get; init; }
    public VariantId VariantId { get; init; }
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
            NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
            opt =>
            {
                opt.AsSplitQuery = true;
                opt.AddInclude(cart => cart.LineItems);
                return opt;
            });

        var loadProduct = from product in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductByIdQuery(command.ProductId), e.Token))
                          select product;

        var db =
            from t in (loadCart, loadProduct).Apply((c, product) => (c, fin: product))
            from _ in UpdateEntity<BasketDbContext, Domain.Models.Cart>(t.c, _ =>
                t.fin
                .Bind(pr => GetUpdatedCart(pr, command.VariantId, t.c, command.Quantity)))
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<Domain.Models.Cart> GetUpdatedCart(ProductResult productResult, VariantId variantId, Domain.Models.Cart cart, int quantity)
    {
        var v = productResult.Variants.FirstOrDefault(var => var.Id == variantId.Value);
        if (v is null)
        {
            return NotFoundError.New($"Variant with Id {variantId.Value} not found in Product {productResult.Id}.");
        }
        return cart.UpdateLineItem(
            LineItem.Create(
                ProductId.From(productResult.Id),
                cart.Id,
                variantId,
                productResult.Slug,
                v.Sku,
                productResult.Images.FirstOrDefault(dto => dto.IsMain)?.Url ?? productResult.Images.First().Url,
                quantity,
                productResult.Price
            ));
    }
}


