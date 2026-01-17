namespace Basket.Application.Features.Coupon.CreateCoupon;


public record CreateCouponCommand : ICommand<Fin<Unit>>
{
    public string Description { get; init; } = null!;
    public string DiscountType { get; init; } = null!;
    public decimal DiscountValue { get; init; }
    public decimal? MinimumPurchaseAmount { get; init; }
    public DateTime ExpiryDate { get; init; }
}

internal class CreateCouponCommandHandler(BasketDbContext dbContext, IClock clock) : ICommandHandler<CreateCouponCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(CreateCouponCommand command,
        CancellationToken cancellationToken)
    {

        var db =
            from c in Domain.Models.Coupon.Create(
                command.Description,
                command.DiscountValue,
                command.ExpiryDate,
                command.DiscountType,
                command.MinimumPurchaseAmount ?? 0m,
                clock.UtcNow)
            from _ in Db<BasketDbContext>.lift(ctx => ctx.Coupons.Add(c))
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
