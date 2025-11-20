using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence.Data;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Order> builder)
    {

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value)
            )
            .HasColumnName("id");
        builder.Property(p => p.UserId).HasConversion(id => id.Value, guid => UserId.From(guid));

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("order_id").OnDelete(DeleteBehavior.Cascade);
        //builder.Property(o => o.Subtotal)
        //    .HasConversion(money => money.Value, d => Money.FromDecimal(d))
        //    .HasColumnName("subtotal");
        //builder.Property(o => o.Total)
        //    .HasConversion(money => money.Value, d => Money.FromDecimal(d))
        //    .HasColumnName("total");
        ////builder.Property(o => o.TaxRate).HasColumnName("tax_rate");
        //builder.Property(o => o.Status)
        //    .HasConversion(
        //        status => status.Name,
        //        str => OrderStatus.FromUnsafe(str)
        //    )
        //    .HasColumnName("status");

        builder.Property("_paidAt")
            .HasColumnName("paid_at");

        builder.Property("_cancelledAt")
            .HasColumnName("cancelled_at");

        builder.Property("_refundedAt")
            .HasColumnName("refunded_at");

        builder.Property("_shippedAt")
            .HasColumnName("shipped_at");

        builder.Property("_deliveredAt")
            .HasColumnName("delivered_at");

        builder.ToTable("orders");
    }
}