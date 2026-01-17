using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => OrderItemId.From(value)
            )
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(o => o.ProductId)
            .HasConversion(id => id.Value, guid => ProductId.From(guid))
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(o => o.OrderId)
            .HasConversion(id => id.Value, guid => OrderId.From(guid))
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(o => o.ColorVariantId)
            .HasConversion(id => id.Value, guid => ColorVariantId.From(guid))
            .HasColumnName("color_variant_id")
            .IsRequired();

        builder.Property(o => o.Slug)
            .HasColumnName("slug")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.Sku)
            .HasColumnName("sku")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.Size)
            .HasColumnName("size")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Color)
            .HasColumnName("color")
            .HasMaxLength(100)
            .IsRequired();


        builder.Property(o => o.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(o => o.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.LineTotal)
            .HasColumnName("line_total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        // Indexes
        builder.HasIndex(o => o.ProductId);
        builder.HasIndex(o => o.Sku);
        builder.HasIndex(o => o.OrderId);

        builder.ToTable("order_items");
    }
}