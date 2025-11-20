using Shared.Application.Contracts.Queries;

namespace Basket.Application.Features.Coupon.AssignCouponToUser;

public record AssignCouponToUserCommand(Guid UserId, Guid CouponId) : ICommand<Fin<Unit>>;


internal class AssignCouponToUserCommandHandler(BasketDbContext dbContext, IUserContext userContext, ISender sender, IClock clock)
    : ICommandHandler<AssignCouponToUserCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AssignCouponToUserCommand command,
        CancellationToken cancellationToken)
    {
        var loadUser =
            Db<BasketDbContext>.liftIO(async (_, e) =>
                await sender.Send(new GetUserByIdQuery(UserId.From(command.UserId)), e.Token));

        var loadCoupon = Db<BasketDbContext>.liftIO(async (ctx, e) =>
            await ctx.Coupons.FirstOrDefaultAsync(c => c.Id == CouponId.From(command.CouponId), e.Token));


        var db = from x in (
                loadUser,
                loadCoupon
            ).Apply((fin, coupon) => { return fin.Bind(_ => ValidateCoupon(coupon)); })

                 from c in x.Bind(coupon => coupon.SetUser(UserId.From(command.UserId)))
                 select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private static Fin<Domain.Models.Coupon> ValidateCoupon(
        Domain.Models.Coupon? coupon)
    {
        if (coupon is null)
            return FinFail<Domain.Models.Coupon>(NotFoundError.New("Coupon not found."));

        if (coupon.IsDeleted)
            return FinFail<Domain.Models.Coupon>(InvalidOperationError.New("Coupon is deleted."));

        if (coupon.UserId is not null || coupon.CartId is not null)
            return FinFail<Domain.Models.Coupon>(InvalidOperationError.New("Coupon is already assigned to a cart."));

        if (coupon.Status != CouponStatus.Unknown)
        {
            return FinFail<Domain.Models.Coupon>(InvalidOperationError.New("Coupon is unavailable."));
        }

        return coupon;
    }
}