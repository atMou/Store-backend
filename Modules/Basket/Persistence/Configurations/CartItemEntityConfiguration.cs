using Basket.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.CartId)
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .IsRequired();

        builder.Property(ci => ci.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ci => ci.Sku)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ci => ci.ImageUrl)
            .HasMaxLength(500);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        // Owned type: UnitPrice (Money)
        builder.OwnsOne(ci => ci.UnitPrice, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

        });

        //// Optional Coupon relationship
        //builder.HasOne(ci => ci.Coupon)
        //    .WithMany() // Coupon doesn’t need backref to CartItem
        //    .HasForeignKey(ci => ci.CouponId)
        //    .OnDelete(DeleteBehavior.SetNull);

        //// Ignore computed properties
        //builder.Ignore(ci => ci.LineTotal);
        //builder.Ignore(ci => ci.LineTotalAfterDiscount);
        //builder.Ignore(ci => ci.UnitPriceAfterDiscount);
    }
}