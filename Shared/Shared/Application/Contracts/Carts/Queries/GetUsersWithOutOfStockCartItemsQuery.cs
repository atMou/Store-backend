namespace Shared.Application.Contracts.Carts.Queries;

public record GetUsersWithOutOfStockCartItemsQuery(ColorVariantId ColorVariantId) : IQuery<Fin<IEnumerable<UserId>>>
{
}