namespace Basket.Application.Features.Cart.ChangeDeliveryAddress;

public record ChangeDeliveryAddressCommand : ICommand<Fin<Unit>>
{
    public CartId CartId { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string ExtraDetails { get; init; }

}


internal class ChangeDeliveryAddressCommandHandler(
    ISender sender,
    BasketDbContext dbContext
) : ICommandHandler<ChangeDeliveryAddressCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(ChangeDeliveryAddressCommand command,
        CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
            cart => cart.Id == command.CartId,
            NotFoundError.New($"Cart with Id '{command.CartId.Value}' was not found."),
            null,
            cart => cart.ChangeDeliveryAddress(new Address
            {
                Street = command.Street,
                City = command.City,
                PostalCode = command.PostalCode,
                HouseNumber = command.HouseNumber,
                ExtraDetails = command.ExtraDetails
            })).Map(_ => unit);
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


