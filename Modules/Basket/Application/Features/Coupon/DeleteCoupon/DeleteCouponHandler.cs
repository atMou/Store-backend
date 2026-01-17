namespace Basket.Application.Features.Coupon.DeleteCoupon;


public record DeleteCouponCommand(CouponId CouponId) : ICommand<Fin<Unit>>;


internal class DeleteCouponCommandHandler(BasketDbContext dbContext, IUserContext userContext)
    : ICommandHandler<DeleteCouponCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteCouponCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from user in userContext.GetCurrentUser<IO>().As()
            from _ in GetUpdateEntity<BasketDbContext, Domain.Models.Coupon>(
                coupon => coupon.Id == command.CouponId,
                NotFoundError.New($"Coupon with id '{command.CouponId.Value} was not found'"),
                null,
                c => c.EnsureCanDelete(UserId.From(user.Id)).Map(co => co.MarkAsDeleted())
            )
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
