using Basket.Persistence;

namespace Basket.Application.Features.Coupon.DeleteCoupon;


public record DeleteCouponCommand(Guid CouponId) : ICommand<Fin<DeleteCouponResult>>;

public record DeleteCouponResult();

internal class DeleteCouponCommandHandler(BasketDbContext dbContext, IUserContext userContext) : ICommandHandler<DeleteCouponCommand, Fin<DeleteCouponResult>>
{
    public Task<Fin<DeleteCouponResult>> Handle(DeleteCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from c in Db<BasketDbContext>.liftIO(async (ctx, e) =>
                await ctx.Coupons.FindAsync([command.CouponId], e.Token))
            from _1 in when(c is null, IO.fail<Unit>(NotFoundError.New("Coupon not found.")))
            from _2 in when(c.IsDeleted, IO.fail<Unit>(InvalidOperationError.New("Coupon is already deleted.")))
            from _3 in when(c.Status == CouponStatus.AssignedToUser, IO.fail<Unit>(InvalidOperationError.New("Cannot delete an active coupon.")))
            from _4 in Db<BasketDbContext>.lift(ctx => ctx.Coupons.Remove(c))
            select new DeleteCouponResult();

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
