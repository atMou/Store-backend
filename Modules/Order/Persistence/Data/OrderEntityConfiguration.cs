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

        builder.Property(o => o.CartId)
            .HasConversion(id => id.Value, value => CartId.From(value))
            .HasColumnName("cart_id");
        builder.Property(p => p.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id");

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("order_id")
            .OnDelete(DeleteBehavior.Cascade);


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

        builder.Property(o => o.Phone).HasColumnName("phone");
        builder.Property(o => o.Email).HasColumnName("email");
        builder.Property(o => o.Subtotal).HasColumnName("sub_total");
        builder.Property(o => o.Total).HasColumnName("total");
        builder.Property(o => o.Tax).HasColumnName("tax");
        builder.Property(o => o.Discount).HasColumnName("discount");
        builder.Property(o => o.TotalAfterDiscounted).HasColumnName("total_after_discounted");
        builder.Property(o => o.ShipmentCost).HasColumnName("shipment_cost");
        builder.OwnsOne(o => o.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).HasColumnName("address_street");
            addressBuilder.Property(a => a.City).HasColumnName("address_city");
            addressBuilder.Property(a => a.PostalCode).HasColumnName("address_postal_code");
            addressBuilder.Property(a => a.HouseNumber).HasColumnName("address_house_number");
            addressBuilder.Property(a => a.ExtraDetails).HasColumnName("address_extra_details");
        });

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