using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Persistence.Configurations;

public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {


        builder.ToTable("line_items");

        builder.HasKey(li => new { li.CartId, li.ProductId }).HasName("line_item_id");

        builder.Property(li => li.CartId)
            .HasConversion(id => id.Value, value => CartId.From(value))
            .HasColumnName("cart_id")
            .IsRequired();

        builder.Property(li => li.ProductId)
            .HasConversion(id => id.Value, value => ProductId.From(value))
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(li => li.Slug)
            .HasMaxLength(200)
            .HasColumnName("slug")
            .IsRequired();

        builder.Property(li => li.ImageUrl)
            .HasMaxLength(500)
            .HasColumnName("image_url");

        builder.Property(li => li.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(li => li.UnitPrice)
            .HasConversion(m => m.Value, d => Money.FromDecimal(d))
            .HasColumnName("unit_price");

        builder.Property(li => li.LineTotal)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("line_total");

        //builder.Property(l => l.CreatedAt).HasColumnName("created_at");
        //builder.Property(l => l.CreatedBy).HasColumnName("created_by");
        //builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");
        //builder.Property(l => l.UpdatedBy).HasColumnName("updated_by");



        //builder.Ignore(li => li.LineTotal);
        builder.Ignore(li => li.LineTotal);
        builder.HasIndex(li => li.CartId);
        builder.HasIndex(li => li.ProductId);

    }

}
