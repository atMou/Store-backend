namespace Basket.Application.Features.Coupon.SetCouponsAsRedeemed;

public record UpdateCouponCommand(IEnumerable<CouponId> CouponIds) : ICommand<Fin<Unit>>;

internal class UpdateCouponCommandHandler(BasketDbContext dbContext, IClock clock)
    : ICommandHandler<UpdateCouponCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(UpdateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            GetUpdateEntities<BasketDbContext, Domain.Models.Coupon>(
                coupon => command.CouponIds.Contains(coupon.Id),
                null,
                (coupon) => coupon.MarkAsRedeemed(clock.UtcNow)
                );


        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
