using Inventory.Domain.Events;
using Inventory.Domain.ValueObjects;

using Shared.Domain.Enums;
using Shared.Domain.Errors;

namespace Inventory.Domain.Models;
public record Inventory : Aggregate<ProductId>
{
    private Inventory() : base(ProductId.New()) { }

    private Inventory(ProductId productId, Stock stock)
        : base(productId)
    {
        ProductId = productId;
        Stock = stock;
    }

    public ProductId ProductId { get; private init; }
    public VariantId VariantId { get; private init; }
    public int Reserved { get; private init; } = 0;

    public byte[] Version { get; private init; }

    public Stock Stock { get; private init; }

    public StockLevel StockLevel =>
        Stock.Value switch
        {
            <= 0 => StockLevel.OutOfStock,
            var s when s <= Stock.Low => StockLevel.LowStock,
            var s when s <= Stock.Mid => StockLevel.MediumStock,
            _ => StockLevel.HighStock
        };

    public static Fin<Inventory> Create(ProductId productId, int quantity, int stockLow, int stockMid, int stockHigh)
       => Stock.From((quantity, stockLow, stockMid, stockHigh))
          .Map(stock => new Inventory(productId, stock));

    public Inventory Increase(int qty)
    {
        var inventory = this with
        {
            Stock = Stock with { Value = Stock.Value + qty },

        };
        if ((StockLevel == StockLevel.OutOfStock || StockLevel == StockLevel.LowStock) &&
            inventory.StockLevel == StockLevel.MediumStock || inventory.StockLevel == StockLevel.HighStock)
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, inventory.Stock.Value));
        }

        return inventory;
    }

    public Fin<Inventory> Decrease(int qty)
    {
        if (Stock.Value < qty)
            return InvalidOperationError.New($"Insufficient stock for product  with id {ProductId.Value}, available stock is {Stock.Value}");
        var inventory = this with { Stock = Stock with { Value = Stock.Value - qty } };

        if (inventory.StockLevel == StockLevel.LowStock)
            AddDomainEvent(new ProductLowStockAlertEvent(ProductId, inventory.Stock.Value));

        if (inventory.StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockAlertEvent(ProductId));

        return inventory;
    }

    public Fin<Inventory> Reserve(int qty)
    {

        if (Stock.Value < qty)
            return InvalidOperationError.New($"Insufficient stock for product with id {ProductId.Value}, available stock is {Stock.Value}");
        var inventory = this with { Stock = Stock with { Value = Stock.Value - qty }, Reserved = Reserved + qty };

        if (inventory.StockLevel == StockLevel.LowStock)
            AddDomainEvent(new ProductLowStockAlertEvent(ProductId, inventory.Stock.Value));

        if (inventory.StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockAlertEvent(ProductId));

        AddDomainEvent(new ProductReservedDomainEvent(ProductId, inventory.Stock.Value, inventory.StockLevel));
        return inventory;
    }

    public Inventory ReleaseReservationDueToCancellation(int qty)
    {
        var newStock = Stock.Value + qty;
        var inventory = this with { Stock = Stock with { Value = newStock }, Reserved = Math.Max(0, Reserved - qty) };
        if ((StockLevel == StockLevel.OutOfStock || StockLevel == StockLevel.LowStock) &&
            (inventory.StockLevel == StockLevel.MediumStock || inventory.StockLevel == StockLevel.HighStock))
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, inventory.Stock.Value));
        }

        return inventory;
    }

    public Inventory ReleaseReservationDueToOrderCompletion(int qty)
    {
        return this with { Reserved = Math.Max(0, Reserved - qty), };

    }
}