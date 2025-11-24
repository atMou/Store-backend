
using Identity.Infrastructure.Authentication;

namespace Identity.Application.Features.EmailVerification;

public record EmailVerificationCommand(string Email, string Token) : ICommand<Fin<EmailVerificationCommandResult>>;

public record EmailVerificationCommandResult(RefreshToken RefreshToken, string AccessToken);
public record EmailVerificationResponse(string AccessToken);

internal class EmailVerificationCommandHandler(
    IJwtProvider jwtProvider,
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IClock clock,
    IOptions<JwtOptions> options
) : ICommandHandler<EmailVerificationCommand, Fin<EmailVerificationCommandResult>>
{
    public Task<Fin<EmailVerificationCommandResult>> Handle(EmailVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from t in (ValidateGuidToken(command.Token), Email.From(command.Email))
                .Apply((guid, email) => (guid, email)).As()


            from x in GetUpdateEntityA<IdentityDbContext, User, RefreshToken, string>(
                user => user.Email == t.email,
                NotFoundError.New($"Please Verify Email your email and try again"),
                (user => RefreshToken.Generate(user.Id, options.Value.Salt, clock.UtcNow),
                    user => jwtProvider.Generate(user, options.Value, TimeSpan.FromMinutes(30))
                ),

                (user, _, _) => user.EnsureNotVerified(),
                (user, _, _) => user.VerifyConfirmationToken(t.guid, clock.UtcNow),
                (user, refreshToken, _) => user.AddRefreshToken(refreshToken, clock.UtcNow))

            select (x.a, x.b, x.c);



        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async tuple =>
                {
                    await endpoint.Publish(new UserEmailVerifiedIntegrationEvent(tuple.a.Id.Value),
                        cancellationToken);
                    return new EmailVerificationCommandResult(tuple.b, tuple.c);
                }
            );
    }


    private Fin<Guid> ValidateGuidToken(string repr)
    {
        return Guid.TryParse(repr, out var token)
            ? FinSucc(token)
            : FinFail<Guid>(ValidationError.New("Invalid verification RefreshToken format."));
    }

}
