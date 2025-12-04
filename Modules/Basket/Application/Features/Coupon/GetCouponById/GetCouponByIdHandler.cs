using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Coupon.GetCouponById;


public record GetCouponByIdCommand(CouponId CouponId) : ICommand<Fin<CouponResult>>;


internal class GetCouponByIdCommandHandler(BasketDbContext dbContext) : ICommandHandler<GetCouponByIdCommand, Fin<CouponResult>>
{
    public Task<Fin<CouponResult>> Handle(GetCouponByIdCommand command,
        CancellationToken cancellationToken)
    {
        var db = GetEntity<BasketDbContext, Domain.Models.Coupon>(
            c => c.Id == command.CouponId,
            NotFoundError.New($"Coupon with id '{command.CouponId.Value} was not found'"),
            options =>
            {
                options.AsNoTracking = true;
                return options;
            })
            .Map(coupon => coupon.ToResult());

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
