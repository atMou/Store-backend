using System.ComponentModel.DataAnnotations.Schema;

using Identity.Domain.Contracts;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Models;

public record User : Aggregate<UserId>
{
    private User(
        Email email,
        Password password,
        Firstname firstname,
        Lastname lastname,
        ImageUrl avatar,
        IEnumerable<Role> roles,
        Gender gender,
        Age age,
        Address address,
        bool isVerified = false
    ) : base(UserId.New)
    {
        Email = email;
        FirstName = firstname;
        Avatar = avatar;
        LastName = lastname;
        Gender = gender;
        Age = age;
        Address = address;
        Password = password;
        _roles = roles;
    }

    public Email Email { get; private init; }
    public Firstname FirstName { get; private init; }
    public Lastname LastName { get; private init; }
    public Age Age { get; private init; }
    public ImageUrl Avatar { get; private init; }
    public Gender Gender { get; private init; }
    [NotMapped]
    public Password Password { get; private init; }
    public string HashedPassword { get; private init; }
    public bool IsVerified { get; private init; }

    public List<OrderId> OrderIds { get; private set; } = new();
    private IEnumerable<Role> _roles { get; }
    public IReadOnlyList<Role> Roles => _roles.ToList().AsReadOnly();
    public List<CouponId> CouponIds { get; private set; } = [];
    private List<ProductId> _likedProductsIds => [];
    public IReadOnlyList<ProductId> LikedProductsIds => _likedProductsIds.AsReadOnly();

    public CartId? CartId { get; private set; }
    public Address Address { get; private init; }

    public static Fin<User> Create(
       CreateUserDto dto
    )
    {
        return (
                Email.From(dto.Email),
                Firstname.From(dto.FirstName),
                Lastname.From(dto.LastName),
                ImageUrl.From(dto.Avatar),
                Age.From(dto.Age),
                Password.From(dto.Password),
                ValidateRolesIfNotNull(dto.Roles),
                Gender.From(dto.Gender),
                ValidateAddresses(dto.City, dto.Street, dto.ZipCode, dto.HouseNumber, dto.ExtraDetails)
            )
            .Apply((e, f, l, av, a, p, rs, g, ad) =>
                new User(e, p, f, l, av, rs, g, a, ad)).As();
    }


    private static Fin<IEnumerable<Role>> ValidateRolesIfNotNull(IEnumerable<string>? roles)
    {
        return roles is null
            ? FinSucc<IEnumerable<Role>>([Role.Default])
            : roles.AsIterable()
                .Traverse(Role.FromValue)
                .Map(i => i.AsEnumerable()).As();
    }

    public User SetIsVerified()
    {
        return this with { IsVerified = true };
    }

    public User SetHashedPassword(string hashedPassword)
    {
        return this with { HashedPassword = hashedPassword };
    }

    private static Fin<Address> ValidateAddresses(string city, string street, uint zipCode, short houseNumber,
        string? extraDetails)
    {
        return new Address(city, street, zipCode, houseNumber, extraDetails);
    }
}