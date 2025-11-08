using Basket.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence.Configurations;
internal class CartEntityConfigurations : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        //// Owned type: Tax
        //builder.OwnsOne(c => c.Tax, tax =>
        //{
        //    tax.Property(t => t.Rate)
        //        .HasColumnName("TaxRate")
        //        .IsRequired();
        //});

        // Owned type: Shipping
        //builder.OwnsOne(c => c.Shipping, shipping =>
        //{
        //    shipping.OwnsOne(s => s.Cost, cost =>
        //    {
        //        cost.Property(m => m.Value)
        //            .HasColumnName("ShippingCost")
        //            .IsRequired();
        //    });
        //    shipping.OwnsOne(s => s.Address, address =>
        //    {
        //        address.Property(a => a.Street).HasMaxLength(200);
        //        address.Property(a => a.City).HasMaxLength(100);
        //        address.Property(a => a.State).HasMaxLength(100);
        //        address.Property(a => a.Country).HasMaxLength(100);
        //        address.Property(a => a.ZipCode).HasMaxLength(20);
        //    });
        //});

        //// Optional Coupon navigation
        //builder.HasOne(c => c.Coupon)
        //    .WithMany()
        //    .HasForeignKey(c => c.CouponId);

        //// CartItem relationship
        //builder.HasMany<CartItem>("Items")
        //    .WithOne()
        //    .HasForeignKey(ci => ci.CartId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //// Ignore computed properties
        //builder.Ignore(c => c.Subtotal);
        builder.Ignore(c => c.TotalTax);
        //builder.Ignore(c => c.TotalShipping);
        builder.Ignore(c => c.Total);
        builder.Ignore(c => c.TotalDiscount);
    }
}
