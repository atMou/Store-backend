using Shared.Application.Abstractions;
using Shared.Application.Contracts.Product.Results;

namespace Shared.Application.Contracts.Product.Queries;

public record GetProductByIdQuery(ProductId ProductId, string[]? Include = null) : IQuery<Fin<ProductResult>>;