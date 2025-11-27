using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence.Data;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<Domain.Models.OrderItem>
{
    public void Configure(EntityTypeBuilder<Domain.Models.OrderItem> builder)
    {

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => OrderItemId.From(value)
            )
            .HasColumnName("id");


        builder.Property(o => o.ProductId).HasConversion(id => id.Value, guid => ProductId.From(guid));

        builder.Property(o => o.Slug).HasColumnName("slug");
        builder.Property(o => o.Sku).HasColumnName("sku");
        builder.Property(o => o.ImageUrl).HasColumnName("image_url");
        builder.Property(o => o.Quantity).HasColumnName("quantity");
        builder.Property(o => o.UnitPrice).HasColumnName("unit_price");


        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.CreatedBy).HasColumnName("created_by");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");
        builder.Property(o => o.UpdatedBy).HasColumnName("updated_by");
        builder.ToTable("OrderItems");
    }
}