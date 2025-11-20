

using Shared.Application.Abstractions;
using Shared.Domain.Contracts.User;

namespace Shared.Application.Contracts.Queries;
public record GetUserByIdQuery(UserId UserId) : IQuery<Fin<UserDto>>;
