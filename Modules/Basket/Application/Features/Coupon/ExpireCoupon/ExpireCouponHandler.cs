using Basket.Persistence;

namespace Basket.Application.Features.Coupon.ExpireCoupon;


public record ExpireCouponCommand(Guid CouponId) : ICommand<Fin<ExpireCouponResult>>;

public record ExpireCouponResult(CouponDto Coupon);

internal class ExpireCouponCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<ExpireCouponCommand, Fin<ExpireCouponResult>>
{
    public Task<Fin<ExpireCouponResult>> Handle(ExpireCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =

            from co in Db<BasketDbContext>.liftIO(async (ctx, e) =>
                await ctx.Coupons.FirstOrDefaultAsync(c => c.Id == CouponId.From(command.CouponId), e.Token))

            from _1 in when(co is null, IO.fail<Unit>(NotFoundError.New($"Coupon with ID {command.CouponId} not found")))
            from _2 in when(co!.Status == CouponStatus.Expired, IO.fail<Unit>(InvalidOperationError.New($"Coupon with ID '{command.CouponId}' is already expired.")))
            let _3 = co.SetExpired(clock.UtcNow)
            select new ExpireCouponResult(co.ToDto());

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
