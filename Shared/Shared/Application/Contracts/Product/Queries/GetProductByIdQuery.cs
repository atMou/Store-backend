using Shared.Application.Abstractions;
using Shared.Domain.Contracts.Product;

namespace Shared.Application.Contracts.Product.Queries;

public record GetProductByIdQuery(ProductId ProductId, string[]? Include = null) : IQuery<Fin<ProductDto>>;