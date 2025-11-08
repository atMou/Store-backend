using Shared.Application.Abstractions;

namespace Shared.Application.Contracts.Product.Queries;

public record GetProductByIdQuery(ProductId ProductId) : IQuery<Fin<GetProductByIdResult>>;