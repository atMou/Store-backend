using Basket.Domain.Models;

using MediatR;

using Shared.Application.Contracts.Queries;
using Shared.Domain.Contracts.User;
using Shared.Domain.Errors;

using Unit = LanguageExt.Unit;

namespace Basket.Application.Features.Cart.CreateCart;

public record CreateCartCommand(
    double taxRate,
    CouponId? couponId = null) : ICommand<Fin<CreateCartCommandResult>>;

public record CreateCartCommandResult(
    Guid CartId
);

internal class CreateCartCommandHandler(
    IUserContext userContext,
    BasketDbContext ctx,
    ISender sender,
    IBus bus,
    ICartRepository cartRepository)
    : ICommandHandler<CreateCartCommand, Fin<CreateCartCommandResult>>
{
    public Task<Fin<CreateCartCommandResult>> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var db =
            from u in userContext.GetCurrentUser<IO>()
            from res in IO.liftAsync(async e => await sender.Send(new GetUserByIdQuery(UserId.From(u.Id)), e.Token))
            from _ in when(res.userDto.CartId is not null,
                IO.fail<Unit>(
                    $"Cannot Create Cart for User '{u.Id}': User Has Cart with Id: '{res.userDto.CartId}'"))
            from co in GetCoupon(command.couponId, res.userDto)
            let cart = Domain.Models.Cart.Create(
                UserId.From(u.Id),
                command.taxRate,
                co)
            from a in Db<BasketDbContext>.lift(BCxt =>
            {
                BCxt.Carts.Add(cart);
                return unit;
            })
            select new CreateCartCommandResult(cart.Id.Value);

        return db.RunSave(ctx, EnvIO.New(null, cancellationToken));
    }


    private IO<Coupon?> GetCoupon(CouponId? couponId, UserDto userDto)
    {
        return Optional(couponId).Match(
            id =>
            {
                var coupon = userDto.CouponIds.FirstOrDefault(c => c == id.Value);
                if (coupon == Guid.Empty)
                    return from c in IO.liftAsync(async e => await ctx.Coupons.FindAsync([id.Value], e.Token))
                           select c;
                return IO.fail<Coupon?>(NotFoundError.New($"Coupon with Id '{id.Value}' not found"));
            },
            () => IO.pure<Coupon?>(null));
    }
}