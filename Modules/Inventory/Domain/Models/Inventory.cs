using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using Inventory.Domain.ValueObjects;

using Shared.Domain.Errors;

namespace Inventory.Domain.Models;
public record Inventory : Aggregate<ProductId>
{
    private Inventory() : base(ProductId.New()) { }

    private Inventory(ProductId productId, int reserved, Stock stock)
        : base(productId)
    {
        ProductId = productId;
        Reserved = reserved;
        Stock = stock;
    }

    public ProductId ProductId { get; private init; }
    public int Reserved { get; private set; }

    // Optimistic concurrency token
    public int Version { get; private set; }

    public Stock Stock { get; private set; }

    public StockLevel StockLevel =>
        Stock.Value switch
        {
            <= 0 => StockLevel.OutOfStock,
            var s when s <= Stock.Low => StockLevel.Low,
            var s when s <= Stock.Mid => StockLevel.Medium,
            _ => StockLevel.High
        };

    public static Fin<Inventory> Create(ProductId productId, int reserved, int quantity, int stockLow, int stockMid, int stockHigh)
       => Stock.From((quantity, stockLow, stockMid, stockHigh))
          .Map(stock => new Inventory(productId, reserved, stock));

    public Inventory Increase(int qty)
    {
        var inventory = this with
        {
            Stock = Stock with { Value = Stock.Value + qty },
            Version = Version + 1
        };
        if ((StockLevel == StockLevel.OutOfStock || StockLevel == StockLevel.Low) &&
            inventory.StockLevel == StockLevel.Medium || inventory.StockLevel == StockLevel.High)
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, inventory.Stock.Value));
        }

        return inventory;
    }

    public Fin<Inventory> Decrease(int qty)
    {
        if (Stock.Value < qty)
            return InvalidOperationError.New($"Insufficient stock, available stock is {Stock.Value}");
        var inventory = this with { Stock = Stock with { Value = Stock.Value - qty }, Version = Version + 1 };

        if (inventory.StockLevel == StockLevel.Low)
            AddDomainEvent(new ProductLowStockAlertEvent(ProductId, inventory.Stock.Value));

        if (inventory.StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockAlertEvent(ProductId));

        return inventory;
    }

    public Fin<Inventory> Reserve(int qty)
    {

        if (Stock.Value < qty)
            return InvalidOperationError.New($"Insufficient stock, available stock is {Stock.Value}");
        var inventory = this with { Stock = Stock with { Value = Stock.Value - qty }, Reserved = Reserved + qty, Version = Version + 1 };

        if (inventory.StockLevel == StockLevel.Low)
            AddDomainEvent(new ProductLowStockAlertEvent(ProductId, inventory.Stock.Value));

        if (inventory.StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockAlertEvent(ProductId));

        return inventory;
    }

    public Inventory ReleaseReservationDueToCancellation(int qty)
    {
        var newStock = Stock.Value + qty;
        var inventory = this with { Stock = Stock with { Value = newStock }, Reserved = Math.Min(0, Reserved - qty), Version = Version + 1 };
        if ((StockLevel == StockLevel.OutOfStock || StockLevel == StockLevel.Low) &&
            inventory.StockLevel == StockLevel.Medium || inventory.StockLevel == StockLevel.High)
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, inventory.Stock.Value));
        }

        return inventory;
    }

    public Inventory ReleaseReservationDueToOrderCompletion(int qty)
    {
        return this with { Reserved = Math.Min(0, Reserved - qty), Version = Version + 1 };

    }
}