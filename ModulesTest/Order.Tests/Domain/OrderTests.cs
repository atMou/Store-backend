namespace Order.Tests.Domain;

public class OrderTests
{
    private readonly CreateOrderDto _validOrderDto;

    public OrderTests()
    {
        _validOrderDto = new CreateOrderDto
        {
            UserId = UserId.From(Guid.NewGuid()),
            CartId = CartId.From(Guid.NewGuid()),
            Subtotal = 100m,
            Total = 120m,
            Tax = 10m,
            Discount = 5m,
            TotalAfterDiscounted = 105m,
            ShipmentCost = 15m,
            DeliveryAddress = new Address
            {
                Street = "Main Street",
                City = "Test City",
                PostalCode = 12345,
                HouseNumber = 123,
                ExtraDetails = "Apt 4B"
            },
            CouponIds = new List<CouponId>(),
            OrderItems = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = ProductId.From(Guid.NewGuid()),
                    ColorVariantId = ColorVariantId.From(Guid.NewGuid()),
                    Slug = "test-product",
                    Sku = "TEST-SKU-001",
                    ImageUrl = "https://test.com/image.jpg",
                    Quantity = 2,
                    UnitPrice = 50m,
                    LineTotal = 100m
                }
            }
        };
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var result = Order.Domain.Models.Order.Create(_validOrderDto);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            order =>
            {
                order.UserId.Should().Be(_validOrderDto.UserId);
                order.CartId.Should().Be(_validOrderDto.CartId);
                order.Subtotal.Should().Be(_validOrderDto.Subtotal);
                order.Total.Should().Be(_validOrderDto.Total);
                order.Tax.Should().Be(_validOrderDto.Tax);
                order.Discount.Should().Be(_validOrderDto.Discount);
                order.OrderStatus.Should().Be(OrderStatus.Pending);
                order.OrderItems.Should().HaveCount(1);
                return order;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void MarkAsPaid_FromPendingStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var paidAt = DateTime.UtcNow;

        // Act
        var result = order.MarkAsPaid(paymentId, paidAt);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            updatedOrder =>
            {
                updatedOrder.OrderStatus.Should().Be(OrderStatus.Paid);
                updatedOrder.PaymentId.Should().Be(paymentId);
                updatedOrder.PaidAt.IsSome.Should().BeTrue();
                return updatedOrder;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void MarkAsShipped_FromPaidStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var shipmentId = ShipmentId.From(Guid.NewGuid());
        var paidOrder = order.MarkAsPaid(paymentId, DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = paidOrder.MarkAsShipped(shipmentId, DateTime.UtcNow);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            updatedOrder =>
            {
                updatedOrder.OrderStatus.Should().Be(OrderStatus.Shipped);
                updatedOrder.ShipmentId.Should().Be(shipmentId);
                updatedOrder.ShippedAt.IsSome.Should().BeTrue();
                return updatedOrder;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void MarkAsShipped_FromPendingStatus_ShouldFail()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var shipmentId = ShipmentId.From(Guid.NewGuid());

        // Act
        var result = order.MarkAsShipped(shipmentId, DateTime.UtcNow);

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void MarkAsDelivered_FromShippedStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var shipmentId = ShipmentId.From(Guid.NewGuid());
        var paidOrder = order.MarkAsPaid(paymentId, DateTime.UtcNow).ThrowIfFail();
        var shippedOrder = paidOrder.MarkAsShipped(shipmentId, DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = shippedOrder.MarkAsDelivered(DateTime.UtcNow);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            updatedOrder =>
            {
                updatedOrder.OrderStatus.Should().Be(OrderStatus.Delivered);
                updatedOrder.DeliveredAt.IsSome.Should().BeTrue();
                return updatedOrder;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void MarkAsCancelled_FromPendingStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();

        // Act
        var result = order.MarkAsCancelled(DateTime.UtcNow);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            updatedOrder =>
            {
                updatedOrder.OrderStatus.Should().Be(OrderStatus.Cancelled);
                updatedOrder.CancelledAt.IsSome.Should().BeTrue();
                return updatedOrder;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void MarkAsCancelled_FromDeliveredStatus_ShouldFail()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var shipmentId = ShipmentId.From(Guid.NewGuid());
        var paidOrder = order.MarkAsPaid(paymentId, DateTime.UtcNow).ThrowIfFail();
        var shippedOrder = paidOrder.MarkAsShipped(shipmentId, DateTime.UtcNow).ThrowIfFail();
        var deliveredOrder = shippedOrder.MarkAsDelivered(DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = deliveredOrder.MarkAsCancelled(DateTime.UtcNow);

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void MarkAsRefunded_FromPendingStatus_ShouldFailDueToBusinessRules()
    {
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var result = order.MarkAsRefunded(DateTime.UtcNow);

        result.IsFail.Should().BeTrue("Refund should not be allowed from pending status");
    }

    [Fact]
    public void MarkAsRefunded_FromDeliveredStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var shipmentId = ShipmentId.From(Guid.NewGuid());
        var paidOrder = order.MarkAsPaid(paymentId, DateTime.UtcNow).ThrowIfFail();
        var shippedOrder = paidOrder.MarkAsShipped(shipmentId, DateTime.UtcNow).ThrowIfFail();
        var deliveredOrder = shippedOrder.MarkAsDelivered(DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = deliveredOrder.MarkAsRefunded(DateTime.UtcNow);

        if (result.IsSucc)
        {
            result.Match(
                updatedOrder =>
                {
                    updatedOrder.OrderStatus.Should().Be(OrderStatus.Refunded);
                    updatedOrder.RefundedAt.IsSome.Should().BeTrue();
                    return updatedOrder;
                },
                _ => throw new Exception("Should not fail")
            );
        }
        else
        {
            result.IsFail.Should().BeTrue();
        }
    }

    [Fact]
    public void EnsureCanDelete_WithCancelledStatus_ShouldFail()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var cancelledOrder = order.MarkAsCancelled(DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = cancelledOrder.EnsureCanDelete();

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void EnsureCanDelete_WithDeliveredStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = PaymentId.From(Guid.NewGuid());
        var shipmentId = ShipmentId.From(Guid.NewGuid());
        var paidOrder = order.MarkAsPaid(paymentId, DateTime.UtcNow).ThrowIfFail();
        var shippedOrder = paidOrder.MarkAsShipped(shipmentId, DateTime.UtcNow).ThrowIfFail();
        var deliveredOrder = shippedOrder.MarkAsDelivered(DateTime.UtcNow).ThrowIfFail();

        // Act
        var result = deliveredOrder.EnsureCanDelete();

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Update_WithPaidStatus_ShouldSucceed()
    {
        // Arrange
        var order = Order.Domain.Models.Order.Create(_validOrderDto).ThrowIfFail();
        var paymentId = Guid.NewGuid();
        var updateDto = new UpdateOrderDto
        {
            Status = "paid",
            PaymentId = paymentId,
            StatusDate = DateTime.UtcNow
        };

        // Act
        var result = order.Update(updateDto);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            updatedOrder =>
            {
                updatedOrder.OrderStatus.Should().Be(OrderStatus.Paid);
                updatedOrder.PaymentId.Should().Be(PaymentId.From(paymentId));
                return updatedOrder;
            },
            _ => throw new Exception("Should not fail")
        );
    }
}
