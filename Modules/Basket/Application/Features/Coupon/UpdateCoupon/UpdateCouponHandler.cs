namespace Basket.Application.Features.Coupon.UpdateCoupon;


public record UpdateCouponCommand(UpdateCouponDto Dto) : ICommand<Fin<UpdateCouponResult>>;

public record UpdateCouponResult(CouponDto Coupon);

internal class UpdateCouponCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<UpdateCouponCommand, Fin<UpdateCouponResult>>
{
    public Task<Fin<UpdateCouponResult>> Handle(UpdateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =

            from co in Db<BasketDbContext>.liftIO(async (ctx, e) =>
                    await ctx.Coupons
                    .FirstOrDefaultAsync(c => c.Id == CouponId.From(command.Dto.CouponId), e.Token))
            from c in co.Update(command.Dto, clock.UtcNow)
            select new UpdateCouponResult(c.ToDto());

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
