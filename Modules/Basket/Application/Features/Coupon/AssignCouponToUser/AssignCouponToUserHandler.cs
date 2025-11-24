using Shared.Application.Contracts.User.Queries;

namespace Basket.Application.Features.Coupon.AssignCouponToUser;

public record AssignCouponToUserCommand(UserId UserId, CouponId CouponId) : ICommand<Fin<Unit>>;


internal class AssignCouponToUserCommandHandler(BasketDbContext dbContext, ISender sender, IClock clock)
    : ICommandHandler<AssignCouponToUserCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AssignCouponToUserCommand command,
        CancellationToken cancellationToken)
    {
        var loadUser = Db<BasketDbContext>.liftIO(async (_, e) =>
            await sender.Send(new GetUserByIdQuery(command.UserId), e.Token));

        var loadCoupon =
            GetEntity<BasketDbContext, Domain.Models.Coupon>(
            coupon => coupon.Id == command.CouponId,
            NotFoundError.New($"Coupon with Id {command.CouponId.Value} not found."));

        var db = from x in (
                loadUser,
                loadCoupon
            ).Apply((u, c) =>
                u.Bind(ud => c.AssignToUser(UserId.From(ud.Id), DateTime.UtcNow)))
                 select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}