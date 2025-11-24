using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id");

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, s => Email.FromUnsafe(s))
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasConversion(f => f.Value, s => Firstname.FromUnsafe(s))
            .HasColumnName("first_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasConversion(l => l.Value, s => Lastname.FromUnsafe(s))
            .HasColumnName("last_name")
            .HasMaxLength(255)
            .IsRequired();


        builder.Property(u => u.Phone)
            .HasConversion(i => Optional(i).Match<string?>(phone => phone.Value, () => null), s => Phone.FromNullable(s))
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(u => u.Age)
            .HasConversion(a => Optional(a).Match<byte?>(age => age.Value, () => null), i => Age.FromNullable(i))
            .HasColumnName("age");

        builder.Property(u => u.Avatar)
            .HasConversion(i => Optional(i).Match<string?>(image => image.Value, () => null), i => ImageUrl.FromNullable(i))
            .HasColumnName("avatar")
            .HasMaxLength(1000);

        builder.Property(u => u.Gender)
            .HasConversion(i => Optional(i)
                .Match<string?>(image => image.Value, () => null), i => Gender.FromNullable(i))
            .HasColumnName("gender");


        builder.Property(u => u.HashedPassword)
            .HasColumnName("password")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_verified");

        builder.Property(u => u.EmailConfirmationExpiresAt)
            .HasColumnName("email_confirmation_expires_at");

        builder.Property(u => u.EmailConfirmationToken)
            .HasColumnName("email_confirmation_token");

        builder
            .HasMany(u => u.LikedProducts)
            .WithOne()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(u => u.PendingOrderIds)
            .WithOne()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        //builder.OwnsMany(u => u.CouponIds, b =>
        //{
        //    b.Property(c => c.Value).HasColumnName("coupon_id");
        //    b.ToTable("user_coupon_ids");
        //});

        //builder.OwnsMany(u => u.OrderIds, b =>
        //{
        //    b.Property(o => o.Value).HasColumnName("order_id");
        //    b.ToTable("user_order_ids");
        //});

        //builder.OwnsMany(u => u.LikedProductIds, b =>
        //{
        //    b.Property(p => p.Value).HasColumnName("product_id");
        //    b.ToTable("user_liked_product_ids");
        //});

        var rolesComparee = new ValueComparer<IReadOnlyList<Role>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );
        var permissionsComparee = new ValueComparer<IReadOnlyList<Permission>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );

        builder.Property(u => u.Roles)
            .HasConversion(
                roles => string.Join(",", roles.Select(r => r.Name)),
                str => str.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Role.FromUnsafe)
                    .ToList()
            )
            .HasColumnName("roles")
            .HasMaxLength(1500)
            .Metadata.SetValueComparer(rolesComparee);



        builder.Property(u => u.Permissions)
            .HasConversion(
                permissions => string.Join(",", permissions.Select(p => p.Name)),
                str => str.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Permission.FromUnsafe)
                    .ToList()
            )
            .HasColumnName("permissions")
            .HasMaxLength(1500)
            .Metadata.SetValueComparer(permissionsComparee);

        builder.Property(u => u.CartId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : CartId.From(value.Value)
            )
            .HasColumnName("cart_id");

        builder.HasMany(u => u.RefreshTokens).WithOne().HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(u => u.Address, nb =>
        {
            nb.Property(a => a.City).HasColumnName("city").HasMaxLength(200);
            nb.Property(a => a.Street).HasColumnName("street").HasMaxLength(200);
            nb.Property(a => a.HouseNumber).HasColumnName("house_number");
            //nb.Property(a => a.ZipCode).HasColumnName("zip_code");
        });

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(user => user.CartId);
    }
}
