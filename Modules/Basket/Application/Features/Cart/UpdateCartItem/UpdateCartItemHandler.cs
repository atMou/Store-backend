using Basket.Application.Contracts;

using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Cart.UpdateCartItem;

public record UpdateLineItemCommand : ICommand<Fin<CartResult>>
{
    public Guid CartId { get; init; }
    public Guid ColorVariantId { get; init; }
    public Guid SizeVariantId { get; init; }
    public int Quantity { get; init; }
}

internal class UpdateLineItemCommandHandler(
    ISender sender,
    BasketDbContext dbContext
) : ICommandHandler<UpdateLineItemCommand, Fin<CartResult>>
{
    public async Task<Fin<CartResult>> Handle(UpdateLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var cartId = CartId.From(command.CartId);
        var colorVariantId = ColorVariantId.From(command.ColorVariantId);

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.CartId == cartId);

        var loadCart = from coupons in loadCoupons
                       from cart in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                           c => c.Id == cartId,
                           NotFoundError.New($"Cart with Id {cartId.Value} not found."),
                           opt =>
                           {
                               opt.AsSplitQuery = true;
                               opt = opt.AddInclude(c => c.LineItems);
                               return opt;
                           },
                           c => c.LoadDiscountsFromCoupons(coupons)
                               .UpdateLineItemQuantity(colorVariantId, command.SizeVariantId, command.Quantity)
                       )
                       select cart;

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToResult(coupons));
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}


