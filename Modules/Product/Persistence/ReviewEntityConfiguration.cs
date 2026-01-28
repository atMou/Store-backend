using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Product.Domain.Models;

namespace Product.Persistence;

public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(r => r.Id);


        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => ReviewId.From(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(r => r.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value)
            )
            .HasColumnName("product_id")
            .IsRequired();


        builder.Property(r => r.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value)
            )
            .HasColumnName("user_id")
            .IsRequired();

        builder.OwnsOne(r => r.Comment, c =>
        {
            c.Property(co => co.Value).HasColumnName("comment").IsRequired();
        });

        builder.Property(r => r.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
        builder.Property(r => r.UserName).HasColumnName("user_name").HasMaxLength(100);
        builder.Property(r => r.Rating)
            .HasConversion(r => r.Value, b => Rating.FromUnsafe(b))
            .HasPrecision(3, 2)
            .HasColumnName("rating")
            .IsRequired();

        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");
        builder.Property(r => r.CreatedBy)
            .HasColumnName("created_by");
        builder.Property(r => r.UpdatedBy)
            .HasColumnName("updated_by");


        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.ProductId);
    }
}
