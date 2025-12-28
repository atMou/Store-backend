using Shared.Application.Contracts.Product.Results;

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
        var db = from product in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductByIdQuery(command.ProductId, $"{Variants}"), e.Token))
                 from cart in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                     cart => cart.Id == command.CartId,
                     NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                     opt =>
                     {
                         opt.AsSplitQuery = true;
                         opt = opt.AddInclude(cart => cart.LineItems);
                         return opt;
                     },
                     cart => product.Bind(p => CreateLineItem(p, cart, command.VariantId, command.Quantity)).Map(
                         cart.AddLineItem

                     ))
                 select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<LineItem> CreateLineItem(ProductResult p, Domain.Models.Cart cart, VariantId variantId, int quantity)
    {
        var variant = p.Variants.FirstOrDefault(v => v.Id == variantId.Value);
        if (variant == null)
        {
            return FinFail<LineItem>(NotFoundError.New($"Variant with Id {variantId.Value} not found."));
        }
        return LineItem.Create(
            ProductId.From(p.Id),
            cart.Id,
            variantId,
            p.Slug,
            variant.Sku,
            variant.Color.Name,
            variant.Size.Name,
            variant.Images.FirstOrDefault(dto => dto.IsMain)?.Url
            ?? p.Images.First().Url,
            quantity,
            p.NewPrice ?? p.Price
        );
    }
}