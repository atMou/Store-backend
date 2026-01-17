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

        var loadCart = GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
               cart => cart.Id == cartId,
               NotFoundError.New($"Cart with Id {cartId.Value} not found."),
               opt =>
               {
                   opt.AsSplitQuery = true;
                   opt = opt.AddInclude(cart => cart.LineItems);
                   return opt;
               },
              cart => cart.UpdateLineItemQuantity(colorVariantId, command.SizeVariantId, command.Quantity)

           );

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.CartId == cartId);

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToResult(coupons));
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}


