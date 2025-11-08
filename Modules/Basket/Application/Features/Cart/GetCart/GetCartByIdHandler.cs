using Basket.Domain.Contracts;

using Shared.Domain.Contracts.Cart;

namespace Basket.Application.Features.Cart.GetCart;
public record GetCartByCartIdResult(
    CartDto CartDto
);

public record GetCartByCartIdQuery(CartId CartId) : IQuery<Fin<GetCartByCartIdResult>>;

internal class GetCartByCartIdQueryHandler(BasketDbContext dbContext, ICartRepository cartRepository)
    : IQueryHandler<GetCartByCartIdQuery, Fin<GetCartByCartIdResult>>
{
    public Task<Fin<GetCartByCartIdResult>> Handle(GetCartByCartIdQuery request, CancellationToken cancellationToken)
    {
        var db = from x in Db<BasketDbContext>.liftIO(ctx => cartRepository.GetCartById(request.CartId, ctx,
                opt =>
                {
                    opt.AsNoTracking = true;
                    opt.AsSplitQuery = true;
                    opt.AddInclude(c => c.CartItems);
                    opt.AddInclude(c => c.Coupons);
                }))
                 select new GetCartByCartIdResult(x.ToDto());

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}