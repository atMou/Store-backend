namespace Identity.Application.Features.AddPhoneNumber;

public record AddPhoneNumberCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public string PhoneNumber { get; init; }
}


public class AddAddressCommandHandler(
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext)
    : ICommandHandler<AddPhoneNumberCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddPhoneNumberCommand command, CancellationToken cancellationToken)
    {
        var db =
            from phone in Phone.From(command.PhoneNumber)
            from _ in GetUpdateEntity<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                null,
                user => user.AddPhone(phone, clock.UtcNow)
            )

            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

