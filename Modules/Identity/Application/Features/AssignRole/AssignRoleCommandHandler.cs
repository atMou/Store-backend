namespace Identity.Application.Features.AssignRole;

public record AssignRoleCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public string Role { get; init; }
}


public class AssignRoleCommandHandler(
    IdentityDbContext dbContext)
    : ICommandHandler<AssignRoleCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AssignRoleCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntityA<IdentityDbContext, User>(
              user => user.Id == command.UserId,
              NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
              opt =>
              {
                  opt.AddInclude(user => user.Roles);
                  return opt;
              },
              user => user.AssignUserToRoles(command.Role)
          ).Map(_ => unit);

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

