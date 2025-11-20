using Shared.Application.Abstractions;

namespace Shared.Application.Contracts.Product.Queries;

public record IsProductInAnyCartsQuery(ProductId ProductId)
    : IQuery<Fin<Unit>>;

