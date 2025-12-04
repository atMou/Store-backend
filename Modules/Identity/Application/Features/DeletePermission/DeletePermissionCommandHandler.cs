namespace Identity.Application.Features.DeletePermission;

public record DeletePermissionCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public IEnumerable<string> Permissions { get; init; }
}


public class DeletePermissionCommandHandler(IdentityDbContext dbContext)
    : ICommandHandler<DeletePermissionCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeletePermissionCommand command, CancellationToken cancellationToken)
    {
        var db =
        GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == command.UserId,
            NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
            opt =>
            {
                opt.AddInclude(user => user.Permissions);
                return opt;
            },
            user => user.DeletePermissions([.. command.Permissions])
        ).Map(_ => unit);
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

