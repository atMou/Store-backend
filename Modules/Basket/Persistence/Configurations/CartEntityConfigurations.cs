using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence.Configurations;
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

        builder.HasMany(c => c.LineItems)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cart => cart.TotalSub)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("total_sub");

        builder.Property(cart => cart.Discount)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("discount");


        //builder.OwnsMany(c => c.LineItems, li =>
        //{
        //    li.WithOwner().HasForeignKey("cart_id");
        //    li.HasKey(item => new { item.ProductId, item.CartId });

        //    li.Property(x => x.ProductId)
        //        .HasConversion(
        //            id => id.Value,
        //            value => ProductId.From(value)).HasColumnName("product_id");

        //    li.Property(x => x.CartId)
        //        .HasConversion(
        //            id => id.Value,
        //            value => CartId.From(value)).HasColumnName("cart_id");

        //    li.Property(x => x.LineTotal)
        //        .HasConversion(money => money.Value, d => Money.FromDecimal(d))
        //        .HasColumnName("line_total");

        //    li.Property(x => x.UnitPrice)
        //        .HasConversion(money => money.Value, d => Money.FromDecimal(d))
        //        .HasColumnName("unit_price");

        //    li.Property(x => x.Quantity);

        //    li.Property(x => x.Slug).HasColumnName("slug");
        //    li.Property(x => x.ImageUrl).HasColumnName("image_url");

        //    li.ToTable("line_items");
        //});


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



//builder.HasMany(c => c.LineItems)
//    .WithOne()
//    .HasForeignKey(ci => ci.CartId)
//    .OnDelete(DeleteBehavior.Cascade);