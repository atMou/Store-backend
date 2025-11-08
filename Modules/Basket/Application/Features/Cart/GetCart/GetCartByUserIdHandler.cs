using Basket.Domain.Contracts;

using Shared.Domain.Contracts.Cart;

namespace Basket.Application.Features.Cart.GetCart;

public record GetCartByUserIdResult(
    CartDto CartDto
);

public record GetCartByUserIdQuery(UserId UserId) : IQuery<Fin<GetCartByUserIdResult>>;

internal class GetCartByUserIdQueryHandler(BasketDbContext dbContext, ICartRepository cartRepository)
    : IQueryHandler<GetCartByUserIdQuery, Fin<GetCartByUserIdResult>>
{
    public Task<Fin<GetCartByUserIdResult>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        var db = from x in Db<BasketDbContext>.liftIO(ctx => cartRepository.GetCartByUserId(request.UserId, ctx,
                opt =>
                {
                    opt.AsNoTracking = true;
                    opt.AsSplitQuery = true;
                    opt.AddInclude(c => c.CartItems);
                    opt.AddInclude(c => c.Coupons);
                }))
                 select new GetCartByUserIdResult(x.ToDto());

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}