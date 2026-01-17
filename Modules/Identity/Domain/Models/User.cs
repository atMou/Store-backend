using Identity.Domain.Events;

namespace Identity.Domain.Models;

public class User : Aggregate<UserId>
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
        Phone? phone,
        Address address
    ) : base(UserId.New)
    {
        Email = email;
        Password = password;
        FirstName = firstname;
        LastName = lastname;
        Gender = gender;
        Age = age;
        Phone = phone;
        Addresses = [address];
    }


    public Email Email { get; private set; }
    public Phone? Phone { get; private set; }
    public Firstname FirstName { get; private set; }
    public Lastname LastName { get; private set; }
    public Age? Age { get; private set; }
    public ImageUrl? Avatar { get; private set; }
    public Gender? Gender { get; private set; }
    public string? EmailConfirmationToken { get; private set; } = String.Empty;
    public string? EmailConfirmationCode { get; private set; } = String.Empty;
    public string? PhoneConfirmationToken { get; private set; } = String.Empty;
    public DateTime? EmailConfirmationExpiresAt { get; private set; }
    public DateTime? PhoneConfirmationExpiresAt { get; private set; }


    [NotMapped] public Password Password { get; }
    public string HashedPassword { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }

    public ICollection<ProductId> LikedProductIds { get; private set; } = [];

    public ICollection<ProductSubscription> ProductSubscriptions { get; private set; } = [];

    public bool HasPendingOrders { get; private set; }
    public List<Role> Roles { get; private set; } = [];


    public List<Permission> Permissions { get; private set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public ICollection<Address> Addresses { get; private set; }

    public CartId? CartId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Fin<User> Create(CreateUserDto dto, DateTime utcNow)
    {
        return (
                Email.From(dto.Email),
                Firstname.From(dto.FirstName),
                Lastname.From(dto.LastName),
                Helpers.ValidateNullable<Age, byte>(dto.Age),
                Password.From(dto.Password),
                Helpers.ValidateNullable<Gender, string>(dto.Gender),
                Helpers.ValidateNullable<Phone, string>(dto.Phone)
            )
            .Apply((e, f, l, a, p, g, ph) =>
                {
                    var address = Address.Create(
                        $"{f.Value} {l.Value}",
                        dto.Street,
                        dto.City,
                        dto.PostalCode,
                        true,
                        dto.ExtraDetails,
                        dto.HouseNumber);
                    var user = new User(e, p, f, l, g.ValueUnsafe(), a.ValueUnsafe(), ph.ValueUnsafe(), address)
                        .GenerateEmailVerificationToken(utcNow);

                    return user;
                }
            ).As();
    }

    public User SetHasPendingOrders(bool value)
    {
        HasPendingOrders = value;
        return this;
    }

    public User MarkAsDeleted()
    {
        IsDeleted = true;
        return this;
    }

    public Fin<User> EnsureNoPendingOrders()
    {
        return HasPendingOrders
            ? FinFail<User>(
                InvalidOperationError.New($"User with id '{Id.Value}' has pending orders"))
            : this;
    }

    public Fin<User> EnsureNotVerified()
    {
        return IsEmailVerified ? FinFail<User>(InvalidOperationError.New("Email is already verified.")) : this;
    }

    public User AddRefreshToken(RefreshToken token, DateTime dateTime)
    {
        var user = RevokeTokens(dateTime, "New refresh token added");
        RefreshTokens = [.. user.RefreshTokens, token];
        return user;
    }

    public User RevokeTokens(DateTime dateTime, string reason)
    {
        var (revoked, active) = RefreshTokens.AsIterable().Partition(token => token.IsRevoked);
        RefreshTokens = [.. active.Map(token => token.Revoke(reason, dateTime)), .. revoked];
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
        return ValidateRoles(roles).Map(rs =>
        {
            Roles = rs.ToList();
            return this;
        });
    }

    public User AssignUserToRoles(params Role[] roles)
    {
        Roles = roles.ToList();
        return this;

    }

    public Fin<User> AssignUserToPermissions(params string[] permissions)
    {
        return ValidatePermissions(permissions).Map(ps =>
        {
            Permissions = ps.ToList();
            return this;
        });
    }

    public Fin<User> DeletePermissions(params string[] permissions)
    {
        return ValidatePermissions(permissions).Map(ps =>
        {
            Permissions = Permissions.Except(ps).ToList();
            return this;
        });
    }

    public Fin<User> DeleteRoles(params string[] roles)
    {
        return ValidateRoles(roles).Map(rs =>
        {
            Roles = Roles.Except(rs).ToList();
            return this;
        });
    }

    private Fin<IEnumerable<Permission>> ValidatePermissions(params string[] permissions)
    {
        return permissions.AsIterable().Traverse(Permission.FromValue)
            .Map(i => i.AsEnumerable()).As();
    }


    public User AddCartId(CartId cartId)
    {
        CartId = cartId;
        return this;
    }

    public User AddAddress(Address address)
    {
        Addresses = [.. Addresses, address];
        return this;
    }
    public User AddAvatar(ImageUrl imageUrl)
    {
        Avatar = imageUrl;
        return this;
    }

    public User DeleteCartId()
    {
        CartId = null;
        return this;
    }

    public User AddPhone(Phone phone, DateTime utcNow)
    {
        var token = Helpers.GenerateCode(6);
        AddDomainEvent(new PhoneNumberAddedDomainEvent(phone, token));


        Phone = phone;
        PhoneConfirmationToken = token;
        PhoneConfirmationExpiresAt = utcNow.AddMinutes(30);
        return this;
    }

    public User GenerateEmailVerificationToken(DateTime utcNow)
    {

        EmailConfirmationCode = Helpers.GenerateCodeNumber(6);
        EmailConfirmationToken = Guid.NewGuid().ToString();
        EmailConfirmationExpiresAt = utcNow.AddMinutes(30);
        return this;

    }

    public User GeneratePhoneVerificationToken()
    {
        PhoneConfirmationToken = Helpers.GenerateCode(6);
        PhoneConfirmationExpiresAt = DateTime.UtcNow.AddMinutes(30);
        return this;
    }


    public User ToggleLikedProducts(params ProductId[] productIds)
    {
        var (existing, nonExisting) = productIds.AsIterable().Partition(pId => LikedProductIds.Contains(pId));

        if (existing.Any())
        {
            LikedProductIds = LikedProductIds
                .Where(lpid => !existing.Contains<Iterable, ProductId>(lpid))
                .ToList();
        }

        if (nonExisting.Any())
        {
            LikedProductIds = [.. LikedProductIds, .. nonExisting];
        }

        return this;
    }

    public User SubscribeToProduct(string productId, string colorCode, string sizeCode)
    {
        var option = Optional(ProductSubscriptions.FirstOrDefault(s =>
            s.Matches(productId, colorCode, sizeCode)));
        return option.Match(subscription =>
           {
               ProductSubscriptions.Remove(subscription);
               return this;
           }, () =>
           {

               ProductSubscriptions.Add(ProductSubscription.Create(productId, colorCode, sizeCode));
               return this;
           });

    }

    public Fin<User> VerifyConfirmationToken(string? code, DateTime utcNow)
    {
        return ValidateToken().Map(_ =>
        {
            IsEmailVerified = true;
            EmailConfirmationToken = null;
            EmailConfirmationCode = null;
            EmailConfirmationExpiresAt = null;

            return this;
        });

        Fin<Unit> ValidateToken()
        {
            return utcNow > EmailConfirmationExpiresAt
                    ? FinFail<Unit>(UnAuthorizedError.New("Email confirmation token has already expired"))
                    : code == EmailConfirmationToken || code == EmailConfirmationCode
                        ? FinSucc(unit)
                    : FinFail<Unit>(UnAuthorizedError.New("Invalid email confirmation code."));
        }
    }

    public Fin<User> VerifyPhone(string token, DateTime expiresAt)
    {
        return ValidateToken().Map(_ =>
        {
            IsPhoneVerified = true;
            PhoneConfirmationToken = null;
            PhoneConfirmationExpiresAt = null;
            return this;
        });

        Fin<Unit> ValidateToken()
        {
            return token != PhoneConfirmationToken
                ? FinFail<Unit>(UnAuthorizedError.New("Invalid phone verification token."))
                : expiresAt > PhoneConfirmationExpiresAt
                    ? FinFail<Unit>(UnAuthorizedError.New("Phone verification token has already expired"))
                    : FinSucc(unit);
        }
    }


    public User HashPassword(Password password)
    {
        var hasher = new PasswordHasher<User>();
        HashedPassword = hasher.HashPassword(this, password.Value);
        return this;
    }

    public Fin<User> VerifyPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(this, HashedPassword, password);
        return result == PasswordVerificationResult.Failed
            ? FinFail<User>(BadRequestError.New("Invalid password"))
            : this;
    }

    private Fin<User> UpdateAddress(UpdateAddressDto address)
    {
        return Optional(Addresses.FirstOrDefault(a => a.Id.Value == address.AddressId))
              .ToFin(NotFoundError.New($"Address with id: '{address.AddressId}' not found"))
              .Map(existingAddress => existingAddress.Update(
                  address.ReceiverName,
                  address.Street,
                  address.City,
                  address.PostalCode,
                  address.IsMain,
                  address.ExtraDetails,
                  address.HouseNumber)).Map(a =>
              {
                  Addresses =
                      [.. Addresses.Where(ad => ad.Id.Value != a.Id.Value).Append(a)];
                  return this;
              });

    }
    public Fin<User> RemoveAddress(AddressId addressId)
    {
        return Optional(Addresses.FirstOrDefault(address => address.Id.Value == addressId.Value))
            .ToFin(NotFoundError.New($"Address with id: '{addressId.Value}' not found"))
            .Bind(addressToDelete =>
            {
                Addresses = Addresses.Where(a => a.Id != addressId).ToList();
                return FinSucc(this);
            });
    }

    private Fin<User> UpdateEmail(string repr)
    {
        return Email.From(repr).Map(e =>
          {
              Email = e;
              return this;
          });

    }
    private Fin<User> UpdateFirstName(string repr)
    {
        return Firstname.From(repr).Map(e =>
        {
            FirstName = e;
            return this;
        });
    }

    private Fin<User> UpdateLastName(string repr)
    {
        return Lastname.From(repr).Map(e =>
        {
            LastName = e;
            return this;
        });
    }

    private Fin<User> UpdateGender(string repr)
    {
        return Gender.From(repr).Map(e =>
        {
            Gender = e;
            return this;
        });
    }

    private Fin<User> UpdatePassword(string repr)
    {
        return Password.From(repr).Map(e =>
        {
            HashPassword(e);
            return this;
        });
    }
    private Fin<User> UpdateAge(byte repr)
    {
        return Age.From(repr).Map(e =>
        {
            Age = e;
            return this;
        });
    }


    public Fin<User> Update(
        UpdateUserDto dto, IClock clock
    )
    {
        var fin = FinSucc(this);

        if (dto.Email is not null)
        {
            fin = fin.Bind(u => u.UpdateEmail(dto.Email));
        }

        if (dto.FirstName is not null)
        {
            fin = fin.Bind(u => u.UpdateFirstName(dto.FirstName));
        }

        if (dto.LastName is not null)
        {
            fin = fin.Bind(u => u.UpdateLastName(dto.LastName));
        }

        if (dto.Age is not null)
        {
            fin = fin.Bind(u => u.UpdateAge(dto.Age.Value));
        }

        if (dto.Password is not null)
        {
            fin = fin.Bind(u => u.UpdatePassword(dto.Password));
        }

        if (dto.Gender is not null)
        {
            fin = fin.Bind(u => u.UpdateGender(dto.Gender));
        }

        if (dto.AddressDto is not null)
        {

            fin = fin.Bind(u => u.UpdateAddress(dto.AddressDto));
        }

        if (dto.Phone is not null)
        {
            fin = fin.Bind(user => Phone.From(dto.Phone)
                .Map(phone => user.AddPhone(phone, clock.UtcNow)));
        }
        return fin;

    }
}