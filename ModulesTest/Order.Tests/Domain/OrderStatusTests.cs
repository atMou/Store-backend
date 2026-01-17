using FluentAssertions;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;

namespace Order.Tests.Domain;

public class OrderStatusTests
{
    [Fact]
    public void Pending_CanTransitionToPaid_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Pending.CanTransitionTo(OrderStatus.Paid);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Pending_CanTransitionToProcessing_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Pending.CanTransitionTo(OrderStatus.Processing);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Pending_CanTransitionToCancelled_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Pending.CanTransitionTo(OrderStatus.Cancelled);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Pending_CannotTransitionToShipped_ShouldFail()
    {
        // Act
        var result = OrderStatus.Pending.CanTransitionTo(OrderStatus.Shipped);

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Paid_CanTransitionToShipped_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Paid.CanTransitionTo(OrderStatus.Shipped);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Paid_CanTransitionToDelivered_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Paid.CanTransitionTo(OrderStatus.Delivered);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Paid_CannotTransitionToCancelled_ShouldFail()
    {
        // Act
        var result = OrderStatus.Paid.CanTransitionTo(OrderStatus.Cancelled);

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Shipped_CanTransitionToDelivered_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Shipped.CanTransitionTo(OrderStatus.Delivered);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Shipped_CannotTransitionToCancelled_ShouldFail()
    {
        // Act
        var result = OrderStatus.Shipped.CanTransitionTo(OrderStatus.Cancelled);

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Delivered_CanTransitionToCompleted_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.Delivered.CanTransitionTo(OrderStatus.Completed);

        // Assert
        result.IsSucc.Should().BeTrue();
    }

    [Fact]
    public void Cancelled_CannotTransitionToAnyStatus_ShouldFail()
    {
        // Act
        var resultToPending = OrderStatus.Cancelled.CanTransitionTo(OrderStatus.Pending);
        var resultToPaid = OrderStatus.Cancelled.CanTransitionTo(OrderStatus.Paid);
        var resultToShipped = OrderStatus.Cancelled.CanTransitionTo(OrderStatus.Shipped);

        // Assert
        resultToPending.IsFail.Should().BeTrue();
        resultToPaid.IsFail.Should().BeTrue();
        resultToShipped.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FromCode_WithValidCode_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.FromCode("Pending");

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            status => status.Should().Be(OrderStatus.Pending),
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void FromCode_WithInvalidCode_ShouldFail()
    {
        // Act
        var result = OrderStatus.FromCode("InvalidStatus");

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FromName_WithValidName_ShouldSucceed()
    {
        // Act
        var result = OrderStatus.FromName("Shipped");

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            status => status.Should().Be(OrderStatus.Shipped),
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void FromName_WithInvalidName_ShouldFail()
    {
        // Act
        var result = OrderStatus.FromName("NonExistentStatus");

        // Assert
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void FromUnsafe_WithValidName_ShouldReturnStatus()
    {
        // Act
        var result = OrderStatus.FromUnsafe("Delivered");

        // Assert
        result.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void FromUnsafe_WithInvalidName_ShouldReturnUnknown()
    {
        // Act
        var result = OrderStatus.FromUnsafe("NonExistentStatus");

        // Assert
        result.Should().Be(OrderStatus.Unknown);
    }

    [Fact]
    public void AllStatuses_ShouldContainExpectedStatuses()
    {
        // Assert
        OrderStatus.All.Should().Contain(OrderStatus.Pending);
        OrderStatus.All.Should().Contain(OrderStatus.Paid);
        OrderStatus.All.Should().Contain(OrderStatus.Shipped);
        OrderStatus.All.Should().Contain(OrderStatus.Delivered);
        OrderStatus.All.Should().Contain(OrderStatus.Completed);
        OrderStatus.All.Should().Contain(OrderStatus.Cancelled);
        OrderStatus.All.Should().Contain(OrderStatus.Refunded);
        OrderStatus.All.Should().HaveCountGreaterThan(5);
    }
}
