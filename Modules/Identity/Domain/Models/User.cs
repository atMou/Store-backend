using Identity.Application.Events;

using LanguageExt.UnsafeValueAccess;

using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Models;

public record User : Aggregate<UserId>
{
    private User() : base(UserId.New)
    {
    }

    private User(
        Email email,
        Password password,
        Firstname firstname,
        Lastname lastname,
        Gender? gender,
        Age? age,
        Address address
    ) : base(UserId.New)
    {
        Email = email;
        Password = password;
        FirstName = firstname;
        LastName = lastname;
        Gender = gender;
        Age = age;
        Address = address;
    }

    public Email Email { get; private init; }
    public Phone? Phone { get; private init; }
    public Firstname FirstName { get; private init; }
    public Lastname LastName { get; private init; }
    public Age? Age { get; private init; }
    public ImageUrl? Avatar { get; private set; }
    public Gender? Gender { get; private init; }
    public Guid? EmailConfirmationToken { get; private set; }
    public string? PhoneConfirmationToken { get; private set; }
    public DateTime? EmailConfirmationExpiresAt { get; private set; }
    public DateTime? PhoneConfirmationExpiresAt { get; private set; }

    [NotMapped] public Password Password { get; }

    public string HashedPassword { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; set; }

    public List<LikedProductId> LikedProducts { get; private init; } = [];
    public List<Role> Roles { get; private set; } = [];
    public List<Permission> Permissions { get; private set; } = [];

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

                Helpers.ValidateNullable<Age, byte>(dto.Age),
                Password.From(dto.Password),
                Helpers.ValidateNullable<Gender, string>(dto.Gender),
                ValidateAddresses(dto.City, dto.Street, dto.ZipCode, dto.HouseNumber, dto.ExtraDetails),
                Helpers.ValidateNullable<Phone, string>(dto.Phone)
            )
            .Apply((e, f, l, a, p, g, ad, ph) =>
                {
                    var user = new User(e, p, f, l, g.ValueUnsafe(), a.ValueUnsafe(), ad)
                        .GenerateEmailVerificationToken();

                    return user;
                }
            ).As();
    }

    private static Fin<IEnumerable<Role>> ValidateRoles(IEnumerable<string> roles)
    {
        return roles.AsIterable()
            .Traverse(Role.FromValue)
            .Map(i => i.AsEnumerable()).As();
    }

    public Fin<User> AssignUserToRoles(params string[] roles)
    {
        return ValidateRoles(roles).Map(rs => this with { Roles = rs.ToList() });
    }

    public Fin<User> AssignUserToPermissions(params string[] permissions)
    {
        return ValidatePermissions(permissions).Map(ps => this with { Permissions = ps.ToList() });
    }

    public Fin<User> DeletePermissions(string[] permissions)
    {
        return ValidatePermissions(permissions).Map(ps => this with { Permissions = Permissions.Except(ps).ToList() });
    }

    public Fin<User> DeleteRoles(params string[] roles)
    {
        return ValidateRoles(roles).Map(rs => this with { Roles = Roles.Except(rs).ToList() });
    }

    private Fin<IEnumerable<Permission>> ValidatePermissions(params string[] permissions)
    {
        return permissions.AsIterable().Traverse(Permission.FromValue)
            .Map(i => i.AsEnumerable()).As();
    }



    public User SetCartId(CartId cartId)
    {
        return this with { CartId = cartId };
    }

    public User SetAvatar(ImageUrl imageUrl)
    {
        return this with { Avatar = imageUrl };
    }

    public User DeleteCartId()
    {
        return this with { CartId = null };
    }

    public User SetPhone(Phone phone, DateTime utcNow)
    {
        var token = Helpers.Generate6DigitCode();
        AddDomainEvent(new PhoneNumberAddedDomainEvent(phone, token));

        return this with
        {
            Phone = phone,
            PhoneConfirmationToken = token,
            PhoneConfirmationExpiresAt = utcNow.AddMinutes(30)
        };
    }

    public User GenerateEmailVerificationToken()
    {
        return this with
        {
            EmailConfirmationToken = Guid.NewGuid(),
            EmailConfirmationExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    public User GeneratePhoneVerificationToken()
    {
        return this with
        {
            PhoneConfirmationToken = Helpers.Generate6DigitCode(),
            PhoneConfirmationExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }



    public User ToggleLikedProduct(ProductId productId)
    {
        var existingProduct = LikedProducts.FirstOrDefault(p => p.ProductId == productId);
        if (existingProduct.IsNotNull())
            return this with { LikedProducts = LikedProducts.Where(lpid => lpid.ProductId != productId).ToList() };
        return this with
        {
            LikedProducts = [.. LikedProducts, LikedProductId.Create(Id, productId)]
        };
    }


    public Fin<User> VerifyEmail(Guid token, DateTime expiresAt)
    {

        return ValidateToken().Map(_ => this with
        {
            IsEmailVerified = true,
            EmailConfirmationToken = null,
            EmailConfirmationExpiresAt = null
        });

        Fin<Unit> ValidateToken() => token != EmailConfirmationToken
            ? FinFail<Unit>(UnAuthorizedError.New("Invalid email verification token."))
            : expiresAt > EmailConfirmationExpiresAt
                ? FinFail<Unit>(UnAuthorizedError.New("Email verification token has already expired"))
                : FinSucc(unit);
    }

    public Fin<User> VerifyPhone(string token, DateTime expiresAt)
    {
        return ValidateToken().Map(_ => this with
        {
            IsPhoneVerified = true,
            PhoneConfirmationToken = null,
            PhoneConfirmationExpiresAt = null
        });

        Fin<Unit> ValidateToken() => token != PhoneConfirmationToken
            ? FinFail<Unit>(UnAuthorizedError.New("Invalid phone verification token."))
            : expiresAt > PhoneConfirmationExpiresAt
                ? FinFail<Unit>(UnAuthorizedError.New("Phone verification token has already expired"))
                : FinSucc(unit);
    }


    public User HashPassword(Password password)
    {
        var hasher = new PasswordHasher<User>();

        return this with
        {
            HashedPassword = hasher.HashPassword(this, password.Value)
        };
    }

    public Fin<Unit> VerifyPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(this, HashedPassword, password);
        return result == PasswordVerificationResult.Failed
            ? FinFail<Unit>(BadRequestError.New("Invalid password"))
            : unit;
    }

    private static Fin<Address> ValidateAddresses(string city, string street, uint zipCode, uint houseNumber,
        string? extraDetails)
    {
        return new Address(city, street, zipCode, houseNumber, extraDetails);
    }

    private User SetAddress(Address address)
    {
        return this with { Address = address };
    }

    public Fin<User> Update(
        string? email,
        string? firstName,
        string? lastName,
        byte? age,
        string? password,
        string? gender,
        Address? address,
        string? phone,
        ImageUrl? avatar
    )
    {
        var validationSeq = Seq<Fin<User>>();

        if (email is not null)
        {
            validationSeq = validationSeq.Add(Email.From(email).Map(e => this with { Email = e }));
        }

        if (firstName is not null)
        {
            validationSeq = validationSeq.Add(Firstname.From(firstName).Map(f => this with { FirstName = f }));
        }

        if (lastName is not null)
        {
            validationSeq = validationSeq.Add(Lastname.From(lastName).Map(l => this with { LastName = l }));
        }

        if (age is not null)
        {
            validationSeq = validationSeq.Add(Age.From(age.Value).Map(a => this with { Age = a }));
        }

        if (password is not null)
        {

            validationSeq = validationSeq.Add(Password.From(password).Map(HashPassword));

        }
        if (gender is not null)
        {
            validationSeq = validationSeq.Add(Gender.From(gender).Map(g => this with { Gender = g }));
        }
        if (address is not null)
        {
            validationSeq = validationSeq.Add(SetAddress(address));
        }
        if (phone is not null)
        {
            validationSeq = validationSeq.Add(Phone.From(phone).Map(p => this with { Phone = p }));
        }
        if (avatar is not null)
        {
            validationSeq = validationSeq.Add(SetAvatar(avatar));
        }
        return validationSeq.Count == 0
            ? (this)
            : validationSeq.Traverse(identity).Map(seq => seq.Last<Seq, User>().Match(user => user, () => this)).As();

    }

}