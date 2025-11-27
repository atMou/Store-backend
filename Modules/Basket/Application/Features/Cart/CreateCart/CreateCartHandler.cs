

using LanguageExt.Pipes;

using Shared.Application.Contracts.User.Queries;

namespace Basket.Application.Features.Cart.CreateCart;

public record CreateCartCommand : ICommand<Fin<Guid>>
{
    public UserId UserId { get; init; } = null!;
    public decimal Tax { get; init; }
    public decimal ShipmentCost { get; init; }
    public CouponId? CouponId { get; init; } = null;
}

internal class CreateCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    ISender sender)
    : ICommandHandler<CreateCartCommand, Fin<Guid>>
{
    public Task<Fin<Guid>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var db =

            from currentUser in userContext.GetCurrentUser<IO>().As().MapFail(e =>
                UnAuthorizedError.New($"Cannot Create Cart: Unable to get current user."))

            from result in IO.liftAsync(async e =>
                await sender.Send(new GetUserByIdQuery(command.UserId), e.Token))

            from cart in result.Bind(result =>
            {
                if (result.CartId is not null)
                {
                    return FinFail<Domain.Models.Cart>(ConflictError.New(
                        $"Cannot Create Cart for User '{command.UserId.Value}': User Has Cart with Id: '{result.CartId}'"));
                }

                if (!result.Addresses.Any())
                {
                    return FinFail<Domain.Models.Cart>(InvalidOperationError.New(
                        $"Cannot Create Cart for User '{command.UserId.Value}': User Has No Addresses."));
                }
                var deliveryAddressResult = result.Addresses.FirstOrDefault(a => a.IsMain) ?? result.Addresses.First();

                return Domain.Models.Cart.Create(command.UserId, command.Tax, command.ShipmentCost, new Address
                {
                    City = deliveryAddressResult.City,
                    Street = deliveryAddressResult.Street,
                    PostalCode = deliveryAddressResult.PostalCode,
                    HouseNumber = deliveryAddressResult.HouseNumber,
                    ExtraDetails = deliveryAddressResult?.ExtraDetails
                });
            })


            from _3 in Db<BasketDbContext>.lift(cxt => cxt.Carts.Add(cart))
            select cart.Id.Value;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


