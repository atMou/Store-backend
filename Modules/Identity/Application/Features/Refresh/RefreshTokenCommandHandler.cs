using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenCommand(string? RefreshToken, string Email) : ICommand<Fin<RefreshCommandResult>>;

public record RefreshCommandResult(RefreshToken RefreshToken, string AccessToken);

public record RefreshTokenResponse(string AccessToken)
{
}

public class RefreshTokenCommandHandler(
    IClock clock,
    IOptions<JwtOptions> options,
    IJwtProvider jwtProvider,
    IdentityDbContext dbContext)
    : ICommandHandler<RefreshTokenCommand, Fin<RefreshCommandResult>>
{
    private readonly JwtOptions _options = options.Value;

    public Task<Fin<RefreshCommandResult>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var db =

            from email in Email.From(command.Email)
            from res in GetUpdateEntity<IdentityDbContext, User, RefreshToken, string>(
                user => user.Email == email,
                NotFoundError.New($"User with´email '{email.Value}' does not exist."),
                (
                    user => RefreshToken.Generate(user.Id, _options.Salt, clock.UtcNow),
                    user => jwtProvider.Generate(user, _options)
                ),
                (u, rt, _) => u.AddRefreshToken(rt, clock.UtcNow))
            select new RefreshCommandResult(res.b, res.c);
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}