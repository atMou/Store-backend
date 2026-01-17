using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence;

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


        builder.OwnsOne(u => u.Avatar, avatarBuilder =>
        {
            avatarBuilder.Property(a => a.PublicId)
                .HasMaxLength(1500)
                .HasColumnName("avatar_public_id");
            avatarBuilder.Property(a => a.Value)
                .HasColumnName("avatar_url")
                .HasMaxLength(1000);

        });

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

        builder.Property(u => u.EmailConfirmationCode)
            .HasColumnName("email_confirmation_code");


        builder
 .OwnsMany(u => u.LikedProductIds, lp =>
            {
                lp.ToTable("user_liked_product_ids");

                lp.WithOwner()
                    .HasForeignKey("UserId");

                lp.Property<Guid>("UserId")
                    .HasColumnName("user_id");

                lp.Property(productId => productId.Value)
                    .HasColumnName("product_id");

                lp.HasKey("UserId", "Value");
            });

        builder.OwnsMany(u => u.ProductSubscriptions, ps =>
        {
            ps.ToTable("user_product_subscriptions");

            ps.WithOwner()
                .HasForeignKey("UserId");

            ps.Property<Guid>("UserId")
                .HasColumnName("user_id")
                .IsRequired();

            ps.Property<int>("Id")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            ps.Property(s => s.Key)
                .HasColumnName("subscription_key")
                .HasMaxLength(255)
                .IsRequired();

            ps.HasKey("UserId", "Id");

            ps.HasIndex("UserId", "Key")
                .IsUnique()
                .HasDatabaseName("ix_user_product_subscriptions_user_key");
        });

        builder.Property(u => u.HasPendingOrders)
            .HasColumnName("has_pending_orders")
            .IsRequired();



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

        builder.OwnsMany(u => u.Addresses, ab =>
            {
                ab.ToTable("user_addresses");
                ab.HasKey(a => new { a.UserId, a.Id });
                ab.Property(a => a.Id).HasConversion(id => id.Value, guid => AddressId.From(guid));
                ab.Property(a => a.UserId).HasConversion(id => id.Value, guid => UserId.From(guid));

                ab.Property(a => a.ReceiverName)
                    .HasColumnName("receiver_name")
                    .HasMaxLength(200)
                    .IsRequired();

                ab.Property(a => a.Street)
                    .HasColumnName("street")
                    .HasMaxLength(255)
                    .IsRequired();
                ab.Property(a => a.City)
                    .HasColumnName("city")
                    .HasMaxLength(100)
                    .IsRequired();
                ab.Property(a => a.PostalCode)
                    .HasColumnName("postal_code");
                ab.Property(a => a.IsMain)
                    .HasColumnName("is_main")
                    .IsRequired();
                ab.Property(a => a.ExtraDetails)
                    .HasColumnName("extra_details")
                    .HasMaxLength(500);
                ab.Property(a => a.HouseNumber)
                    .HasColumnName("house_number")
                    .IsRequired();
            });




        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(user => user.CartId);
    }
}
