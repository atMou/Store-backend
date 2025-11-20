using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence.Data;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<Domain.Models.OrderItem>
{
    public void Configure(EntityTypeBuilder<Domain.Models.OrderItem> builder)
    {

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => OrderItemId.From(value)
            )
            .HasColumnName("id");


        builder.Property(p => p.ProductId).HasConversion(id => id.Value, guid => ProductId.From(guid));

        builder.Property(p => p.Slug).HasColumnName("slug");
        builder.Property(p => p.Sku).HasColumnName("sku");
        builder.Property(p => p.ImageUrl).HasColumnName("image_url");
        builder.Property(p => p.Quantity).HasColumnName("quantity");
        builder.Property(p => p.UnitPrice)
            .HasConversion(money => money.Value, d => Money.FromDecimal(d))
            .HasColumnName("unit_price");



        builder.ToTable("OrderItemsDtos");
    }
}