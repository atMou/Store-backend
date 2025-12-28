namespace Identity.Application.Features.UpdateUserDetails;

public record UpdateUserDetailsCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; } = null!;
    public IFormFile? Image { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public UpdateAddressDto? Address { get; init; }
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
            AddressDto = Address,
            Age = Age,
            Gender = Gender,
            Password = Password,
            Phone = Phone,
            UserId = UserId,
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
            from u in GetUpdateEntity<IdentityDbContext, User>(
                user => user.Id == command.UserId,
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                null,
                user => user.Update(command.ToDto(), clock))
            from img in UploadImage(command.Image, u).Map(u.AddAvatar)
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
    private IO<ImageUrl> UploadImage(IFormFile? Imag, User user)
    {
        return from x in user.Avatar?.PublicId is not null && Imag.IsNotNull() ?
                imageService.DeleteImagesAsync([user.Avatar.PublicId]) :
                IO.pure(Unit.Default)
               from y in Imag.IsNotNull() ? imageService.UploadImage(Imag!, $"{user.FirstName.Value}_{user.LastName.Value}") :
                  IO.pure(ImageUrl.FromUnsafe("", ""))
               select y;

    }
}