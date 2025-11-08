using Identity.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id");
        //builder.Property(u => u.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(255);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.CreatedBy).HasColumnName("created_by");
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by");

        builder.Property(u => u.Roles)
            .HasConversion(
                roles => string.Join(",", roles.Select(r => r.Name)),
                str => str.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Role.FromUnsafe)
                    .ToList()
            )
            .HasColumnName("roles")
            .HasMaxLength(500);

    }
}
