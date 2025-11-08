using Basket.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Basket.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property<DateTime>("ExpiryDate")
            .HasColumnName("ExpiryDate")
            .IsRequired();

        // Owned type Discount
        builder.OwnsOne(c => c.Discount, discount =>
        {
            discount.Property(d => d.DiscountType)
                .HasColumnName("DiscountType")
                .IsRequired();

            discount.Property(d => d.DiscountValue)
                .HasColumnName("DiscountValue")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

    }
}