using Identity.Domain.ValueObjects;

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

        builder.Property(p => p.UserId).HasConversion(id => id.Value, guid => UserId.From(guid)).HasColumnName("user_id");

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("order_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.Phone).HasConversion(phone => phone.Value, s => Phone.FromUnsafe(s)).HasColumnName("phone");

        builder.Property(o => o.Email).HasConversion(email => email.Value, s => Email.FromUnsafe(s)).HasColumnName("email");


        builder.OwnsMany(x => x.CouponIds, b =>
        {
            b.ToTable("coupon_ids");
            b.WithOwner().HasForeignKey("order_id");
            b.Property<Guid>("id");
            b.Property(c => c.Value).HasColumnName("coupon_id");
            b.HasKey("id");
        });

        builder.Property(o => o.ShipmentId)
            .HasConversion(id => Optional(id).Match<Guid?>(shipmentId => shipmentId.Value, () => null),
                guid => Optional(guid).Match<ShipmentId?>(ShipmentId.From, () => null)).HasColumnName("shipment_id");
        builder.Property(o => o.PaymentId)
            .HasConversion(id => Optional(id).Match<Guid?>(paymentId => paymentId.Value, () => null),
                guid => Optional(guid).Match<PaymentId?>(PaymentId.From, () => null)).HasColumnName("payment_id");


        builder.Property(o => o.Subtotal).HasConversion(m => m.Value, d => Money.FromDecimal(d)).HasColumnName("sub_total");
        builder.Property(o => o.Total).HasConversion(m => m.Value, d => Money.FromDecimal(d)).HasColumnName("total");
        builder.Property(o => o.Tax).HasConversion(m => m.Value, d => Money.FromDecimal(d)).HasColumnName("tax");
        builder.Property(o => o.Discount).HasConversion(m => m.Value, d => Money.FromDecimal(d)).HasColumnName("discount");

        builder.Property(o => o.TrackingCode)
            .HasConversion(
                trackingCode => trackingCode.Value,
                str => TrackingCode.FromUnsafe(str)
            ).HasColumnName("tracking_code");

        builder.Property(o => o.OrderStatus)
            .HasConversion(s => s.Name, s => OrderStatus.FromUnsafe(s)).HasColumnName("order_status");

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



        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.CreatedBy).HasColumnName("created_by");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");
        builder.Property(o => o.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(order => order.UserId);
        builder.HasIndex(order => order.Email);
        builder.HasIndex(order => order.TrackingCode).IsUnique();
        builder.ToTable("orders");
    }
}