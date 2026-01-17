using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {

        builder.HasKey(c => c.Id);

        builder.Property(coupon => coupon.Id)
            .HasConversion(id => id.Value, guid => CouponId.From(guid))
            .ValueGeneratedNever()
            .HasColumnName("coupon_id");



        builder.Property(c => c.UserId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : UserId.From(value.Value))
            .HasColumnName("user_id");

        builder.Property(c => c.Code).HasColumnName("code")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasConversion(description => description.Value, s => Description.FromUnsafe(s))
            .HasColumnName("description")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(coupon => coupon.ExpiryDate)
            .HasConversion(date => date.Value, dt => ExpiryDate.FromUnsafe(dt))
            .HasColumnName("expiry_date")
            .IsRequired();

        builder.OwnsOne(c => c.Discount, discount =>
        {
            discount.Property(d => d.DiscountType)
                .HasColumnName("discount_type")
                .IsRequired();

            discount.Property(d => d.DiscountValue)
                .HasColumnName("discount_value")
                .HasColumnType("decimal(6,2)")
                .IsRequired();
        });

        builder.Property(c => c.MinimumPurchaseAmount)
            .HasColumnName("minimum_purchase_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(c => c.CouponStatus)
            .HasConversion(status => status.Name, s => CouponStatus.FromUnsafe(s))
            .HasColumnName("status");

        builder.Property(c => c.CartId).HasConversion(
            id => Optional(id).Match<Guid?>(cartId => cartId.Value, () => null),
            guid => Optional(guid).Match<CartId?>(CartId.From, () => null));


        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted");

        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");

        //builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CartId).IsUnique();
        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.CouponStatus);

        builder.ToTable("coupons");
    }
}