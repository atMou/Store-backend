namespace Basket.Application.Features.Coupon.GetCouponByUserId;


public record GetCouponByUserIdCommand(Guid UserId) : ICommand<Fin<GetCouponByUserIdResult>>;

public record GetCouponByUserIdResult(IEnumerable<CouponDto> Coupons, int count);

internal class GetCouponByUserIdCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<GetCouponByUserIdCommand, Fin<GetCouponByUserIdResult>>
{
    public Task<Fin<GetCouponByUserIdResult>> Handle(GetCouponByUserIdCommand command,
        CancellationToken cancellationToken)
    {
        var db =

            from cos in Db<BasketDbContext>.liftIO(async (ctx, e) =>
                await ctx.Coupons
                    .Where(c => c.UserId == UserId.From(command.UserId)).ToListAsync(e.Token))
                //from _1 in when(cos is null, IO.fail<Unit>(NotFoundError.New($"User does not have coupon with id '{command.UserId}' not found")))
            select new GetCouponByUserIdResult(cos.Select(co => co.ToDto()), cos.Count());

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
