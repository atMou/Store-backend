using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence;

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
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(o => o.CartId)
            .HasConversion(id => id.Value, value => CartId.From(value))
            .HasColumnName("cart_id")
            .IsRequired();


        builder.Property(p => p.UserId)
            .HasConversion(id => id.Value, guid => UserId.From(guid))
            .HasColumnName("user_id")
            .IsRequired();


        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
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
                guid => Optional(guid).Match<ShipmentId?>(ShipmentId.From, () => null))
            .HasColumnName("shipment_id")
            .IsRequired(false);

        builder.Property(o => o.PaymentId)
            .HasConversion(id => Optional(id).Match<Guid?>(paymentId => paymentId.Value, () => null),
                guid => Optional(guid).Match<PaymentId?>(PaymentId.From, () => null))
            .HasColumnName("payment_id")
            .IsRequired(false);

        builder.Property(o => o.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(o => o.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(o => o.Subtotal)
            .HasColumnName("sub_total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Total)
            .HasColumnName("total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Tax)
            .HasColumnName("tax")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Discount)
            .HasColumnName("discount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.TotalAfterDiscounted)
            .HasColumnName("total_after_discounted")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.ShipmentCost)
            .HasColumnName("shipment_cost")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(o => o.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(o => o.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(o => o.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(255)
                .IsRequired();
            addressBuilder.Property(a => a.ReceiverName)
                .HasColumnName("receiver_name")
                .HasMaxLength(255)
                .IsRequired();

            addressBuilder.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code")
                .IsRequired();

            addressBuilder.Property(a => a.HouseNumber)
                .HasColumnName("address_house_number")
                .IsRequired();

            addressBuilder.Property(a => a.ExtraDetails)
                .HasColumnName("address_extra_details")
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.Property(o => o.TrackingCode)
            .HasConversion(
                trackingCode => trackingCode.Value,
                str => TrackingCode.FromUnsafe(str)
            )
            .HasColumnName("tracking_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.OrderStatus)
            .HasConversion(s => s.Name, s => OrderStatus.FromUnsafe(s))
            .HasColumnName("order_status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property("_paidAt")
            .HasColumnName("paid_at")
            .IsRequired(false);

        builder.Property("_cancelledAt")
            .HasColumnName("cancelled_at")
            .IsRequired(false);

        builder.Property("_refundedAt")
            .HasColumnName("refunded_at")
            .IsRequired(false);

        builder.Property("_shippedAt")
            .HasColumnName("shipped_at")
            .IsRequired(false);

        builder.Property("_deliveredAt")
            .HasColumnName("delivered_at")
            .IsRequired(false);

        // Ignore [NotMapped] Option<DateTime> properties
        builder.Ignore(o => o.PaidAt);
        builder.Ignore(o => o.DeliveredAt);
        builder.Ignore(o => o.ShippedAt);
        builder.Ignore(o => o.CancelledAt);
        builder.Ignore(o => o.RefundedAt);

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(o => o.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasIndex(order => order.UserId);
        builder.HasIndex(order => order.CartId);
        builder.HasIndex(order => order.PaymentId);
        builder.HasIndex(order => order.Email);
        builder.HasIndex(order => order.OrderStatus);
        builder.HasIndex(order => order.TrackingCode).IsUnique();
        builder.HasIndex(order => order.CreatedAt);
        builder.HasIndex(order => order.IsDeleted);

        builder.ToTable("orders");
    }
}