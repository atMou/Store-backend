namespace Basket.Application.Features.Coupon.DeleteCoupon;


public record DeleteCouponCommand(CouponId CouponId) : ICommand<Fin<Unit>>;


internal class DeleteCouponCommandHandler(BasketDbContext dbContext)
    : ICommandHandler<DeleteCouponCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            GetUpdateEntity<BasketDbContext, Domain.Models.Coupon>(
                coupon => coupon.Id == command.CouponId,
                NotFoundError.New($"Coupon with id '{command.CouponId.Value} was not found'"),
                c => c.EnsureCanDelete().Map(co => co.MarkAsDeleted())
                );

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
