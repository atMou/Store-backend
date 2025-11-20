namespace Basket.Application.Features.Coupon.CreateCoupon;


public record CreateCouponCommand(CreateCouponDto Dto) : ICommand<Fin<Unit>>;

internal class CreateCouponCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<CreateCouponCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(CreateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var createCouponDto = command.Dto with { CurrentDate = clock.UtcNow };
        var db =
            from c in Domain.Models.Coupon.Create(
                command.Dto.Description,
                command.Dto.DiscountValue,
                command.Dto.ExpiryDate,
                command.Dto.DiscountType,
                command.Dto.CurrentDate,
                command.Dto.MinimumPurchaseAmount)
            from _ in Db<BasketDbContext>.lift(ctx => ctx.Coupons.Add(c))
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
