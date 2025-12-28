namespace Identity.Application.Features.Logout;


public record LogoutCommand(string Email) : ICommand<Fin<Unit>>;
public class LogoutCommandHandler(IClock clock, IdentityDbContext dbContext) : ICommandHandler<LogoutCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var db =
            from e in Email.From(command.Email)
            from x in GetUpdateEntity<IdentityDbContext, User>(
                user => user.Email == e,
                NotFoundError.New($"User with Email: '{command.Email}' is not available"),
                opt =>
                {
                    opt = opt.AddInclude(user => user.RefreshTokens);
                    opt.AsSplitQuery = true;
                    return opt;
                },
                user => user.RevokeTokens(clock.UtcNow, "Logout Action")
            )
            select unit;
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}
