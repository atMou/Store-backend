using Basket.Application.Contracts;

using Shared.Application.Contracts.Carts.Results;
using Shared.Application.Contracts.Product.Results;

namespace Basket.Application.Features.Cart.AddMultipleCartItems;

public record AddMultipleLineItemsCommand : ICommand<Fin<CartResult>>
{
    public CartId CartId { get; init; }
    public IEnumerable<LineItemRequest> Items { get; init; } = [];
}

public record LineItemRequest
{
    public ProductId ProductId { get; init; }
    public ColorVariantId ColorVariantId { get; init; }
    public Guid SizeVariantId { get; init; }
    public int Quantity { get; init; }
}

internal class AddMultipleLineItemsCommandHandler(
    ISender sender,
    BasketDbContext dbContext
) : ICommandHandler<AddMultipleLineItemsCommand, Fin<CartResult>>
{
    public async Task<Fin<CartResult>> Handle(AddMultipleLineItemsCommand command, CancellationToken cancellationToken)
    {
        var productIds = command.Items.Select(request => request.ProductId);

        var loadCart = from paginatedResults in Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetProductsByIdsQuery { ProductIds = productIds, Include = $"{Variants}" },
                    e.Token))
                       from cart in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                           cart => cart.Id == command.CartId,
                           NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                           opt =>
                           {
                               opt.AsSplitQuery = true;
                               opt = opt.AddInclude(cart => cart.LineItems);
                               return opt;
                           },
                           cart => paginatedResults.Bind(p => p.Items.AsIterable().Traverse(result =>
                                   command.Items.AsIterable().Traverse(request => CreateLineItem(result, cart,
                                       request.ColorVariantId, request.Quantity, request.SizeVariantId))))
                               .Map(iterable => cart.AddLineItems([.. iterable.Flatten()])))
                       select cart;

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.CartId == command.CartId);

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToResult(coupons));

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<LineItem> CreateLineItem(ProductResult p, Domain.Models.Cart cart, ColorVariantId colorVariantId,
        int quantity, Guid svId)
    {
        var variant = p.ColorVariants.FirstOrDefault(v => v.Id == colorVariantId.Value);
        if (variant == null)
            return FinFail<LineItem>(NotFoundError.New($"Variant with Id {colorVariantId.Value} not found."));

        var size = Optional(variant.SizeVariants.FirstOrDefault(sv => sv.Id == svId))
            .ToFin(NotFoundError.New($"Size Variant with Id {svId} not found."));

        return size.Map(s => LineItem.Create(
            ProductId.From(p.Id),
            cart.Id,
            colorVariantId,
            s.Id,
            p.Slug,
            s.Sku,
            variant.Color.Name,
            s.Size.Name,
            variant.Images.FirstOrDefault(dto => dto.IsMain)?.Url
            ?? p.Images.First().Url,
            quantity,
            p.NewPrice ?? p.Price
        ));
    }
}