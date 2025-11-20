using Basket.Persistence;

namespace Basket.Application.Features.Cart.UpdateItemsPrice;

public record UpdateCartItemsPriceCommand(ProductId ProductId, decimal NewPrice)
    : ICommand<Fin<UpdateCartItemsPriceResult>>;

public record UpdateCartItemsPriceResult(int updatedCount);

internal class UpdateCartItemPriceCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    ICartRepository cartRepository,
    IPublishEndpoint endpoint)
    : ICommandHandler<UpdateCartItemsPriceCommand, Fin<UpdateCartItemsPriceResult>>
{
    public async Task<Fin<UpdateCartItemsPriceResult>> Handle(UpdateCartItemsPriceCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from x in userContext.HasPermission<IO>(Permission.EditProduct,
                UnAuthorizedError.New("You are not authorized to update cart item prices")).As()
            from res in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.UpdateCartItemPrice(command.ProductId, command.NewPrice, ctx))
            select new UpdateCartItemsPriceResult(res);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseBiEvent(
                res =>
                    endpoint.Publish(new CartItemPriceUpdatedEvent(res.updatedCount, command.ProductId),
                        cancellationToken),
                err =>
                    endpoint.Publish(new CartItemUpdateFailEvent(command.ProductId, err), cancellationToken));
    }
}