namespace Identity.Application.Features.DeleteRole;

public record DeleteUserRoleCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public string Role { get; init; }
}


public class DeleteUserRoleCommandHandler(IdentityDbContext dbContext)
    : ICommandHandler<DeleteUserRoleCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteUserRoleCommand command, CancellationToken cancellationToken)
    {
        var db =
            GetUpdateEntity<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                opt =>
                {
                    opt.AddInclude(user => user.Roles);
                    return opt;
                },
                user => user.DeleteRoles(command.Role)
            ).Map(_ => unit);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

