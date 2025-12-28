using Identity.Application.Events;

namespace Identity.Application.Features.ResendVerification;

public record ResendVerificationCommand(string Email) : ICommand<Fin<Unit>>;

internal class ResendVerificationCommandHandler(
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IClock clock
) : ICommandHandler<ResendVerificationCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(ResendVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from email in Email.From(command.Email)
            from u in
                GetUpdateEntity<IdentityDbContext, User>(
                    user => user.Email == email,
                    NotFoundError.New("Email you trying to verify is not registered."),
                    null,
                    user => user.EnsureNotVerified(),
                    user => user.GenerateEmailVerificationToken(clock.UtcNow))
            select u;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async u =>
                {
                    await endpoint.Publish(
                        new UserCreatedIntegrationEvent($"{u.FirstName.Value} {u.LastName.Value}", u.Email.Value,
                            u.EmailConfirmationCode, u.EmailConfirmationToken, u.EmailConfirmationExpiresAt),
                        cancellationToken);
                    return unit;
                }
            );
    }
}