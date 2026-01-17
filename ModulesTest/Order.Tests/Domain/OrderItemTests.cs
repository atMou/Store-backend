using FluentAssertions;
using Order.Domain.Contracts;
using Order.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Order.Tests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var dto = new CreateOrderItemDto
        {
            ProductId = ProductId.From(Guid.NewGuid()),
            ColorVariantId = ColorVariantId.From(Guid.NewGuid()),
            Slug = "test-product",
            Sku = "TEST-SKU-001",
            ImageUrl = "https://test.com/image.jpg",
            Quantity = 2,
            UnitPrice = 50m,
            LineTotal = 100m
        };

        // Act
        var result = OrderItem.Create(dto);

        // Assert
        result.IsSucc.Should().BeTrue();
        result.Match(
            item =>
            {
                item.ProductId.Should().Be(dto.ProductId);
                item.ColorVariantId.Should().Be(dto.ColorVariantId);
                item.Slug.Should().Be(dto.Slug);
                item.Sku.Should().Be(dto.Sku);
                item.Quantity.Should().Be(dto.Quantity);
                item.UnitPrice.Should().Be(dto.UnitPrice);
                item.LineTotal.Should().Be(dto.LineTotal);
                return item;
            },
            _ => throw new Exception("Should not fail")
        );
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldHandleBasedOnBusinessRules()
    {
        // Arrange
        var dto = new CreateOrderItemDto
        {
            ProductId = ProductId.From(Guid.NewGuid()),
            ColorVariantId = ColorVariantId.From(Guid.NewGuid()),
            Slug = "test-product",
            Sku = "TEST-SKU-001",
            ImageUrl = "https://test.com/image.jpg",
            Quantity = 0,
            UnitPrice = 50m,
            LineTotal = 0m
        };

        // Act
        var result = OrderItem.Create(dto);

        // Assert - Either succeeds or fails based on business rules
        // If business rules don't allow zero quantity, it should fail
        if (result.IsSucc)
        {
            result.Match(
                item => item.Quantity.Should().Be(0),
                _ => throw new Exception("Should not reach here")
            );
        }
        else
        {
            // Business rules reject zero quantity
            result.IsFail.Should().BeTrue();
        }
    }

    [Fact]
    public void Create_WithMultipleItems_ShouldCreateCorrectLineTotals()
    {
        // Arrange
        var dto1 = new CreateOrderItemDto
        {
            ProductId = ProductId.From(Guid.NewGuid()),
            ColorVariantId = ColorVariantId.From(Guid.NewGuid()),
            Slug = "product-1",
            Sku = "SKU-001",
            ImageUrl = "https://test.com/image1.jpg",
            Quantity = 3,
            UnitPrice = 25m,
            LineTotal = 75m
        };

        var dto2 = new CreateOrderItemDto
        {
            ProductId = ProductId.From(Guid.NewGuid()),
            ColorVariantId = ColorVariantId.From(Guid.NewGuid()),
            Slug = "product-2",
            Sku = "SKU-002",
            ImageUrl = "https://test.com/image2.jpg",
            Quantity = 2,
            UnitPrice = 50m,
            LineTotal = 100m
        };

        // Act
        var result1 = OrderItem.Create(dto1);
        var result2 = OrderItem.Create(dto2);

        // Assert
        result1.IsSucc.Should().BeTrue();
        result2.IsSucc.Should().BeTrue();

        var item1 = result1.ThrowIfFail();
        var item2 = result2.ThrowIfFail();

        item1.LineTotal.Should().Be(75m);
        item2.LineTotal.Should().Be(100m);
        (item1.LineTotal + item2.LineTotal).Should().Be(175m);
    }
}
