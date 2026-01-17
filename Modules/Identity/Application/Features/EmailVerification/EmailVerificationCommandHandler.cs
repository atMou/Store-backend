
using Identity.Application.Contracts;
using Identity.Infrastructure.Authentication;

using Shared.Application.Features.User.Events;

namespace Identity.Application.Features.EmailVerification;

public record EmailVerificationCommand(string Email, string? Code, string? Token) : ICommand<Fin<EmailVerificationResult>>;

public record EmailVerificationResult(UserResult User, RefreshToken RefreshToken, string AccessToken);
public record EmailVerificationResponse(UserResult User, string AccessToken);

internal class EmailVerificationCommandHandler(
    IJwtProvider jwtProvider,
    IdentityDbContext dbContext,
    IPublishEndpoint endpoint,
    IClock clock,
    IOptions<JwtOptions> options
) : ICommandHandler<EmailVerificationCommand, Fin<EmailVerificationResult>>
{
    public async Task<Fin<EmailVerificationResult>> Handle(EmailVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from t in (ValidateToken(command.Token, command.Code), Email.From(command.Email))
                .Apply((code, email) => (code, email)).As()


            from x in GetUpdateEntity<IdentityDbContext, User, RefreshToken, string>(
                user => user.Email == t.email,
                NotFoundError.New($"Email you trying to verify is not registered."),
                (user => RefreshToken.Generate(user.Id, options.Value.Salt, clock.UtcNow),
                    user => jwtProvider.Generate(user, options.Value, TimeSpan.FromMinutes(30))
                ),

                (user, _, _) => user.EnsureNotVerified(),
                (user, _, _) => user.VerifyConfirmationToken(t.code, clock.UtcNow),
                (user, refreshToken, _) => user.AddRefreshToken(refreshToken, clock.UtcNow))

            select (x.a, x.b, x.c);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async tuple =>
                {
                    var address = tuple.a.Addresses.FirstOrDefault(a => a.IsMain) ?? tuple.a.Addresses.First();
                    await endpoint.Publish(new UserEmailVerifiedIntegrationEvent(tuple.a.Id.Value,
                            new Shared.Domain.ValueObjects.Address()
                            {

                                City = address.City,
                                HouseNumber = address.HouseNumber,
                                PostalCode = address.PostalCode,
                                Street = address.Street,
                                ExtraDetails = address.ExtraDetails,
                                ReceiverName = address.ReceiverName
                            }),
                        cancellationToken);
                    return new EmailVerificationResult(tuple.a.ToResult(), tuple.b, tuple.c);
                }
            );
    }



    private Fin<string> ValidateToken(string? token, string? code)
    {
        var result = Optional(token).ToValidation<Error>(ValidationError.New("Token is required."))
                     | Optional(code).ToValidation<Error>(ValidationError.New("Code is required."));
        return result.ToFin();

    }

}
