using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Identity.Domain.Models;

namespace Identity.Persistence;

internal class UserLikedProductConfiguration : IEntityTypeConfiguration<UserLikedProduct>
{
    public void Configure(EntityTypeBuilder<UserLikedProduct> builder)
    {
        builder.ToTable("user_liked_products");

        builder.HasKey(ulp => new { ulp.UserId, ulp.ProductId });

        builder.Property(ulp => ulp.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ulp => ulp.ProductId)
            .HasConversion(id => id.Value, guid => ProductId.From(guid))
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ulp => ulp.LikedAt)
            .HasColumnName("liked_at")
            .IsRequired();

        builder.HasIndex(ulp => ulp.ProductId);
    }
}
