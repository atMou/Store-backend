using Shared.Application.Contracts.Carts.Queries;

namespace Basket.Application.Features.Cart.GetCart;


internal class GetUsersWithOutOfStockCartItemsQueryHandler(
    BasketDbContext dbContext)
    : IQueryHandler<GetUsersWithOutOfStockCartItemsQuery, Fin<IEnumerable<UserId>>>
{
    public async Task<Fin<IEnumerable<UserId>>> Handle(GetUsersWithOutOfStockCartItemsQuery query, CancellationToken cancellationToken)
    {
        var db =
              GetEntities<BasketDbContext, Domain.Models.Cart>(cart =>
              cart.LineItems.Any(li => li.ColorVariantId == query.ColorVariantId),
                  opt => opt.AddInclude(cart => cart.LineItems)

              ).Map(list => list.Select(cart => cart.UserId));


        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}