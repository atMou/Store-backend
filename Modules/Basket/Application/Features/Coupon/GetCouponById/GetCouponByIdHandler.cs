namespace Basket.Application.Features.Coupon.GetCouponById;


public record GetCouponByIdCommand(Guid CouponId) : ICommand<Fin<GetCouponByIdResult>>;

public record GetCouponByIdResult(CouponDto Coupon);

internal class GetCouponByIdCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<GetCouponByIdCommand, Fin<GetCouponByIdResult>>
{
    public Task<Fin<GetCouponByIdResult>> Handle(GetCouponByIdCommand command,
        CancellationToken cancellationToken)
    {
        var db =

            from co in Db<BasketDbContext>.liftIO(async (ctx, e) =>
                await ctx.Coupons
                    .FirstOrDefaultAsync(c => c.Id == CouponId.From(command.CouponId), e.Token))
            from _1 in when(co is null, IO.fail<Unit>(NotFoundError.New($"Coupon with id '{command.CouponId}' not found")))
            select new GetCouponByIdResult(co.ToDto());

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
