using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Product.Domain.Models;

namespace Product.Persistence.Data;

public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);


        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => ReviewId.From(value)).HasColumnName("id");

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
            c.Property(co => co.Value).HasColumnName("comment");
        });


        builder.Property(r => r.Rating)
            .HasConversion(r => r.Value, b => Rating.FromUnsafe(b)).HasPrecision(3, 2)
            .HasColumnName("rating").IsRequired();

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
        builder.ToTable("reviews");

    }
}
