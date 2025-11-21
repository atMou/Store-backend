

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
    ISender sender)
    : ICommandHandler<CreateCartCommand, Fin<Guid>>
{
    public Task<Fin<Guid>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        //var cart = Domain.Models.Cart.Create(command.UserId, command.Tax);
        var db =

            //from currentUser in userContext.GetCurrentUser<IO>().As().MapFail(e =>
            //    UnAuthorizedError.New($"Cannot Create Cart: Unable to get current user."))

            from result in IO.liftAsync(async e =>
                await sender.Send(new GetUserByIdQuery(command.UserId), e.Token))

            let userDto = result.Match<UserDto?>(userDto => userDto, _ => null)

            from _1 in when(userDto.CartId is not null, IO.fail<Unit>(ConflictError.New(
                $"Cannot Create Cart for User '{command.UserId.Value}': User Has Cart with Id: '{userDto.CartId}'")))

            from _cart in Domain.Models.Cart.Create(command.UserId, command.Tax, command.ShipmentCost)

                //from _2 in IO.liftAsync(async e =>
                //    await sender.Send(new SetUserCartIdCommand(currentUser.Id, _cart.Id.Value), e.Token))


            from _3 in Db<BasketDbContext>.lift(cxt => cxt.Carts.Add(_cart))
            select _cart.Id.Value;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}


