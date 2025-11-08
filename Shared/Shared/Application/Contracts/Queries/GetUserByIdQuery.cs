

using Shared.Application.Abstractions;

namespace Shared.Application.Contracts.Queries;
public record GetUserByIdQuery(UserId UserId) : IQuery<GetUserByIdQueryResult>;
