using Shared.Application.Abstractions;
using Shared.Domain.Contracts.Product;

namespace Shared.Application.Contracts.Product.Queries;

public record GetProductByIdQuery(ProductId ProductId, bool IncludeRelated) : IQuery<Fin<ProductDto>>;