
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
    public List<PendingOrderId> PendingOrderIds { get; private set; } = [];
    public List<Role> Roles { get; private set; } = [];


    public List<Permission> Permissions { get; private set; } = [];
    public List<RefreshToken> RefreshTokens { get; private set; } = [];

    public CartId? CartId { get; private set; }
    public Address Address { get; private init; }
    public bool IsDeleted { get; private set; }

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

    public User AddPendingOrder(PendingOrderId orderId)
    {
        return this with { PendingOrderIds = [.. PendingOrderIds, orderId] };
    }
    public User RemovePendingOrder(PendingOrderId orderId)
    {
        return this with { PendingOrderIds = PendingOrderIds.Where(id => id != orderId).ToList() };
    }



    public User MarkAsDeleted()
    {
        return this with { IsDeleted = true };
    }
    public Fin<User> HasNoPendingOrders()
    {
        return PendingOrderIds.Count > 0
            ? FinFail<User>(
                InvalidOperationError.New($"User with id has '{PendingOrderIds.Count}' pending orders")) : this;
    }

    public Fin<User> EnsureNotVerified() =>
        IsEmailVerified ? FinFail<User>(InvalidOperationError.New("Email is already verified.")) : this;
    public User AddRefreshToken(RefreshToken token, DateTime dateTime)
    {
        var u = RevokeTokens(dateTime, "New refresh token added");
        return u with { RefreshTokens = [.. RefreshTokens, token] };
    }
    public User RevokeTokens(DateTime dateTime, string reason)
    {
        var activeTokens = RefreshTokens.Where(token => !token.IsRevoked);
        var revoked = activeTokens.AsIterable().Map(token => token.Revoke(reason, dateTime));
        return this;
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

    public Fin<User> DeletePermissions(params string[] permissions)
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
        var token = Helpers.GenerateCode(6);
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
            PhoneConfirmationToken = Helpers.GenerateCode(6),
            PhoneConfirmationExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }


    public User ToggleLikedProducts(params ProductId[] productIds)
    {
        var updatedLikedProducts = LikedProducts.ToList();
        foreach (var productId in productIds)
        {
            var existingProduct = updatedLikedProducts.FirstOrDefault(p => p.ProductId == productId);
            if (existingProduct.IsNotNull())
            {
                updatedLikedProducts = updatedLikedProducts.Where(lpid => lpid.ProductId != productId).ToList();
            }
            else
            {
                updatedLikedProducts.Add(LikedProductId.Create(Id, productId));
            }
        }
        return this with { LikedProducts = updatedLikedProducts };
    }

    public Fin<User> VerifyConfirmationToken(Guid token, DateTime utcNow)
    {

        return ValidateToken().Map(_ => this with
        {
            IsEmailVerified = true,
            EmailConfirmationToken = null,
            EmailConfirmationExpiresAt = null
        });

        Fin<Unit> ValidateToken() => token != EmailConfirmationToken
            ? FinFail<Unit>(UnAuthorizedError.New("Invalid email verification token."))
            : utcNow > EmailConfirmationExpiresAt
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

    public Fin<User> VerifyPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(this, HashedPassword, password);
        return result == PasswordVerificationResult.Failed
            ? FinFail<User>(BadRequestError.New("Invalid password"))
            : this;
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
        UpdateUserDto dto
    )
    {

        var validationSeq = Seq<Fin<User>>();

        if (dto.Email is not null)
        {
            validationSeq = validationSeq.Add(Email.From(dto.Email).Map(e => this with { Email = e }));
        }

        if (dto.FirstName is not null)
        {
            validationSeq = validationSeq.Add(Firstname.From(dto.FirstName).Map(f => this with { FirstName = f }));
        }

        if (dto.LastName is not null)
        {
            validationSeq = validationSeq.Add(Lastname.From(dto.LastName).Map(l => this with { LastName = l }));
        }

        if (dto.Age is not null)
        {
            validationSeq = validationSeq.Add(Age.From(dto.Age.Value).Map(a => this with { Age = a }));
        }

        if (dto.Password is not null)
        {

            validationSeq = validationSeq.Add(Password.From(dto.Password).Map(HashPassword));

        }
        if (dto.Gender is not null)
        {
            validationSeq = validationSeq.Add(Gender.From(dto.Gender).Map(g => this with { Gender = g }));
        }
        if (dto.Address is not null)
        {
            validationSeq = validationSeq.Add(SetAddress(dto.Address));
        }
        if (dto.Phone is not null)
        {
            validationSeq = validationSeq.Add(Phone.From(dto.Phone).Map(p => this with { Phone = p }));
        }
        if (dto.Image is not null)
        {
            validationSeq = validationSeq.Add(SetAvatar(dto.Image));
        }
        return validationSeq.Count == 0
            ? (this)
            : validationSeq.Traverse(identity).Map(seq => seq.Last<Seq, User>().Match(user => user, () => this)).As();

    }

}