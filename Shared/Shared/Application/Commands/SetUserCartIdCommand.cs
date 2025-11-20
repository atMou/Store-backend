using Shared.Application.Abstractions;

namespace Shared.Application.Commands;

public record SetUserCartIdCommand(Guid UserId, Guid CartId) : ICommand<Fin<Unit>>;