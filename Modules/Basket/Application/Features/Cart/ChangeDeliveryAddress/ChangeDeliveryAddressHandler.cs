using Shared.Application.Contracts.User.Queries;
using Shared.Application.Features.Cart.Events;

namespace Basket.Application.Features.Cart.ChangeDeliveryAddress;

public record ChangeDeliveryAddressCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public CartId CartId { get; init; }
    public Guid AddressId { get; init; }

}


internal class ChangeDeliveryAddressCommandHandler(
    ISender sender,
    BasketDbContext dbContext,
    IPublishEndpoint endpoint
) : ICommandHandler<ChangeDeliveryAddressCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(ChangeDeliveryAddressCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from userResult in IO.liftAsync(async e =>
                await sender.Send(new GetUserByIdQuery(command.UserId), e.Token))

            from address in userResult.Bind(u =>
            {
                var addressResult = u.Addresses.FirstOrDefault(a => a.Id == command.AddressId);
                if (addressResult is null)
                {
                    return FinFail<Address>(NotFoundError.New($"Address with Id '{command.AddressId}' was not found for User with Id '{command.UserId.Value}'"));
                }

                return FinSucc(new Address
                {
                    ReceiverName = addressResult.ReceiverName,
                    City = addressResult.City,
                    Street = addressResult.Street,
                    PostalCode = addressResult.PostalCode,
                    HouseNumber = addressResult.HouseNumber,
                    ExtraDetails = addressResult.ExtraDetails
                });
            })

            from _ in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id '{command.CartId.Value}' was not found."),
                null,
                cart => cart.ChangeDeliveryAddress(address))
            select (command.CartId.Value, address);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async t =>
            {
                await endpoint.Publish(new DeliveryAddressChangedIntegrationEvent
                {
                    CartId = t.Item1,
                    Address = t.Item2
                }, cancellationToken);
                return unit;
            });
    }
}


