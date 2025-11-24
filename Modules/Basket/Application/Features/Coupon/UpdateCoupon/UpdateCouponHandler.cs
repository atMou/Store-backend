namespace Basket.Application.Features.Coupon.UpdateCoupon;

public record UpdateCouponCommand(UpdateCouponDto Dto) : ICommand<Fin<Unit>>;

internal class UpdateCouponCommandHandler(BasketDbContext dbContext, IClock clock)
    : ICommandHandler<UpdateCouponCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(UpdateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            GetUpdateEntity<BasketDbContext, Domain.Models.Coupon>(
                coupon => coupon.Id == command.Dto.CouponId,
                    NotFoundError.New($"Coupon with id '{command.Dto.CouponId.Value} was not found'"),
                o => o.Update(command.Dto, clock.UtcNow)
                );


        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
