

using Shared.Application.Contracts.User.Queries;

namespace Basket.Application.Features.Cart.CreateCart;

public record CreateCartCommand : ICommand<Fin<Guid>>
{
    public UserId UserId { get; init; } = null!;
}

internal class CreateCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    ISender sender)
    : ICommandHandler<CreateCartCommand, Fin<Guid>>
{
    public async Task<Fin<Guid>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var db =
            from result in IO.liftAsync(async e =>
                await sender.Send(new GetUserByIdQuery(command.UserId), e.Token))

            from cart in result.Bind(u =>
            {
                if (u.CartId is not null)
                {
                    return FinFail<Domain.Models.Cart>(ConflictError.New(
                        $"Cannot Create Cart for User '{command.UserId.Value}': User Has Cart with Id: '{u.CartId}'"));
                }

                var deliveryAddressResult = u.Addresses.FirstOrDefault(a => a.IsMain) ?? u.Addresses.First();

                return Domain.Models.Cart.Create(command.UserId, GetTaxService(), new Address
                {
                    City = deliveryAddressResult.City,
                    Street = deliveryAddressResult.Street,
                    PostalCode = deliveryAddressResult.PostalCode,
                    HouseNumber = deliveryAddressResult.HouseNumber,
                    ExtraDetails = deliveryAddressResult?.ExtraDetails
                });
            })

            from _3 in AddEntity<BasketDbContext, Domain.Models.Cart>(cart)
            select cart.Id.Value;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private decimal GetTaxService()
    {

        return 0.15m;
    }


}


