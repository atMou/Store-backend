namespace Shared.Application.Contracts.Carts.Queries;

public record GetUsersWithOutOfStockCartItemsQuery(VariantId VariantId) : IQuery<Fin<IEnumerable<UserId>>>
{
}