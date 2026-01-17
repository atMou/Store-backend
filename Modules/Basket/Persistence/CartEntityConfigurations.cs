using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence;
internal class CartEntityConfigurations : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {


        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, guid => CartId.From(guid))
            .HasColumnName("cart_id")
            .ValueGeneratedNever();

        builder.Property(c => c.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(cart => cart.Tax)
            .HasConversion(tax => tax.Rate, r => Tax.FromUnsafe(r))
            .HasColumnName("tax_rate").HasPrecision(5, 2);


        builder.Property(cart => cart.IsCheckedOut)
            .HasColumnName("is_checked_out");


        builder.OwnsMany(x => x.CouponIds, b =>
        {
            b.ToTable("coupon_ids");
            b.WithOwner().HasForeignKey("cart_id");
            b.Property<Guid>("id");
            b.Property(c => c.Value).HasColumnName("coupon_id");
            b.HasKey("id");
        });

        builder.Property(cart => cart.TotalSub)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("total_sub");

        builder.Property(c => c.TotalDiscount)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("total_discount");

        builder.OwnsOne(cart => cart.DeliveryAddress, address =>
        {
            address.Property(a => a.ReceiverName)
                .HasColumnName("delivery_address_receiver_name")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("delivery_address_city")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Street)
                .HasColumnName("delivery_address_street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("delivery_address_postal_code")
                .IsRequired();

            address.Property(a => a.HouseNumber)
                .HasColumnName("delivery_address_house_number")
                .IsRequired();

            address.Property(a => a.ExtraDetails)
                .HasColumnName("delivery_address_extra_details")
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.Property(cart => cart.ShipmentCost)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("shipment_cost");

        builder.Property(cart => cart.IsActive)
            .HasColumnName("is_active");


        // Configure LineItems as OwnsMany
        builder.OwnsMany(c => c.LineItems, li =>
        {
            li.ToTable("line_items");
            li.WithOwner().HasForeignKey("cart_id");


            li.HasKey("CartId", "ProductId", "ColorVariantId", "SizeVariantId");

            li.Property(x => x.ProductId)
                .HasConversion(id => id.Value, value => ProductId.From(value))
                .HasColumnName("product_id")
                .IsRequired();

            li.Property(x => x.CartId)
                .HasConversion(id => id.Value, value => CartId.From(value))
                .HasColumnName("cart_id")
                .IsRequired();

            li.Property(x => x.ColorVariantId)
                .HasConversion(id => id.Value, value => ColorVariantId.From(value))
                .HasColumnName("color_variant_id")
                .IsRequired();


            li.Property(x => x.SizeVariantId)
                .HasColumnName("size_variant_id")
                .IsRequired();

            li.Property(x => x.Color)
                .HasMaxLength(100)
                .HasColumnName("color")
                .IsRequired();

            li.Property(x => x.Size)
                .HasMaxLength(50)
                .HasColumnName("size")
                .IsRequired();

            li.Property(x => x.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku")
                .IsRequired();

            li.Property(x => x.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug")
                .IsRequired();

            li.Property(x => x.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");

            li.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            li.Property(x => x.UnitPrice)
                .HasConversion(money => money.Value, d => Money.FromDecimal(d))
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            li.Ignore(x => x.LineTotal);

            li.HasIndex(x => x.CartId);
            li.HasIndex(x => x.ProductId);
            li.HasIndex(x => new { x.ColorVariantId, x.SizeVariantId });
        });

        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");

        builder.Ignore(c => c.TaxValue);
        builder.Ignore(c => c.TotalAfterDiscounted);
        builder.Ignore(c => c.Total);

        builder.HasIndex(c => c.UserId);
        builder.ToTable("carts");
    }
}