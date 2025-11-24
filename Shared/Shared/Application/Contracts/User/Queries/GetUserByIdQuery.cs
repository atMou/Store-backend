using Shared.Application.Abstractions;
using Shared.Application.Contracts.User.Results;

namespace Shared.Application.Contracts.User.Queries;
public record GetUserByIdQuery(UserId UserId) : IQuery<Fin<UserResult>>;
