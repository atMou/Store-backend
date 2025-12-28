using Shared.Application.Abstractions;

namespace Shared.Application.Contracts.Product.Queries;

public record EnsureProductNotInCartsQuery(ProductId ProductId)
	: IQuery<Fin<Unit>>;

