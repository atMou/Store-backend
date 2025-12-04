namespace Basket.Application.Features.Coupon.ExpireCoupon;


public record ExpireCouponCommand(CouponId CouponId) : ICommand<Fin<Unit>>;



internal class ExpireCouponCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<ExpireCouponCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(ExpireCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<BasketDbContext, Domain.Models.Coupon>(
         coupon => coupon.Id == command.CouponId,
         NotFoundError.New($"Coupon with id '{command.CouponId.Value} was not found'"),
         null,
                o => o.MarkAsExpired(clock.UtcNow)
         ).Map(_ => unit);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
