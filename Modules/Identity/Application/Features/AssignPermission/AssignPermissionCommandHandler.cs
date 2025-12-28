namespace Identity.Application.Features.AssignPermission;

public record AssignPermissionCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public IEnumerable<string> Permissions { get; init; }
}


public class AssignPermissionCommandHandler(
    IdentityDbContext dbContext)
    : ICommandHandler<AssignPermissionCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AssignPermissionCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == command.UserId,
            NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
            opt =>
            {
                opt.AddInclude(user => user.Permissions);
                return opt;
            },
            user => user.AssignUserToPermissions([.. command.Permissions])
        ).Map(_ => unit);
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

