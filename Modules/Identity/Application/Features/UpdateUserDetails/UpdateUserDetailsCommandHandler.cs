namespace Identity.Application.Features.UpdateUserDetails;

public record UpdateUserDetailsCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; } = null!;
    public IFormFile? Image { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public Address? Address { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? Gender { get; init; }
    public byte? Age { get; init; }

    public UpdateUserDto ToDto()
    {
        return new UpdateUserDto()
        {
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Address = Address,
            Age = Age,
            Gender = Gender,
            Password = Password,
            Phone = Phone,
            UserId = UserId
        };
    }
}

public class UpdateUserDetailsCommandHandler(
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext,
    IImageService imageService,
    IUserContext userContext)
    : ICommandHandler<UpdateUserDetailsCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(UpdateUserDetailsCommand command, CancellationToken cancellationToken)
    {
        var db =
            from u in GetUpdateEntityA<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                null,
                user => user.Update(command.ToDto()))
            from img in UploadImage(command.Image, u.FirstName.Value, u.LastName.Value)
            from _ in UpdateEntity<IdentityDbContext, User>(u, user => user.Update(new UpdateUserDto() { Image = img }))
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
    private IO<ImageUrl> UploadImage(IFormFile? Imag, string firstname, string lastname)
    {
        return Imag.IsNotNull()
            ? imageService.UploadImage(Imag!, $"{firstname}_{lastname}")
            : IO.pure(ImageUrl.FromUnsafe(""));

    }
}