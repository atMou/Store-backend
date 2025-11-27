using Basket.Domain.Events;

namespace Basket.Application.Features.Cart.UpdateItemsPrice;

public record UpdateCartItemsPriceCommand(ProductId ProductId, decimal NewPrice)
    : ICommand<Fin<UpdateCartItemsPriceResult>>;

public record UpdateCartItemsPriceResult(int UpdatedCount);

internal class UpdateCartItemPriceCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,

    IPublishEndpoint endpoint)
    : ICommandHandler<UpdateCartItemsPriceCommand, Fin<UpdateCartItemsPriceResult>>
{
    public async Task<Fin<UpdateCartItemsPriceResult>> Handle(UpdateCartItemsPriceCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from res in Db<BasketDbContext>.liftIO(ctx =>
                UpdateCartItemPrice(command.ProductId, command.NewPrice, ctx))
            select new UpdateCartItemsPriceResult(res);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseBiEvent(
                res =>
                    endpoint.Publish(new CartItemPriceUpdatedDomainEvent(res.UpdatedCount, command.ProductId),
                        cancellationToken),
                err =>
                    endpoint.Publish(new CartItemUpdateFailDomainEvent(command.ProductId, err), cancellationToken));
    }

    public IO<int> UpdateCartItemPrice(ProductId productId, decimal newPrice, BasketDbContext ctx)
    {
        return from res in IO.liftAsync(async e =>
                await ctx.CartItems.Where(ci => ci.ProductId == productId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(ci => ci.UnitPrice.Value, _ => newPrice), e.Token))
               select res;
    }
}