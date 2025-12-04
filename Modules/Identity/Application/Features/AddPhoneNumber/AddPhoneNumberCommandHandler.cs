namespace Identity.Application.Features.AddPhoneNumber;

public record AddPhoneNumberCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public string PhoneNumber { get; init; }
}


public class AddPhoneNumberCommandHandler(
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext)
    : ICommandHandler<AddPhoneNumberCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AddPhoneNumberCommand command, CancellationToken cancellationToken)
    {
        var db =
            from phone in Phone.From(command.PhoneNumber)
            from _ in GetUpdateEntity<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                null,
                user => user.SetPhone(phone, clock.UtcNow)
            )

            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

