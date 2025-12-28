using Inventory.Domain.Events;

using Shared.Domain.Enums;
using Shared.Domain.Errors;

namespace Inventory.Domain.Models;

public record Inventory : Aggregate<InventoryId>
{
    private Inventory() : base(InventoryId.New()) { }

    private Inventory(
        ProductId productId,
        VariantId variantId,
        string sku,
        Stock stock,
        string brand,
        string slug)
        : base(InventoryId.New())
    {
        ProductId = productId;
        VariantId = variantId;
        Sku = sku;
        Brand = brand;
        Slug = slug;
        Stock = stock;
    }

    public ProductId ProductId { get; private init; }
    public VariantId VariantId { get; private init; }
    public string Sku { get; private init; }
    public string Brand { get; private init; }
    public string Slug { get; private init; }
    public int Reserved { get; private set; } = 0;
    public byte[] Version { get; private set; }
    public Stock Stock { get; private set; }

    public int AvailableStock => Stock.Value - Reserved;

    public StockLevel StockLevel =>
        AvailableStock switch
        {
            <= 0 => StockLevel.OutOfStock,
            var s when s <= Stock.Low => StockLevel.LowStock,
            var s when s <= Stock.Mid => StockLevel.MediumStock,
            _ => StockLevel.HighStock
        };

    public static Fin<Inventory> Create(
        ProductId productId,
        VariantId variantId,
        string sku,
        int stock,
        int low,
        int mid,
        int high,
        string brand,
        string slug)
    {
        return Stock.From((stock, low, mid, high))
            .Map(_stock => new Inventory(productId, variantId, sku, _stock, brand, slug));
    }

    public Inventory Increase(int qty)
    {
        var previousLevel = StockLevel;
        var newStock = Stock.Value + qty;

        Stock = Stock with { Value = newStock };

        if ((previousLevel == StockLevel.OutOfStock || previousLevel == StockLevel.LowStock) &&
            (StockLevel == StockLevel.MediumStock || StockLevel == StockLevel.HighStock))
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, VariantId, Sku, AvailableStock));
        }

        return this;
    }

    public Fin<Inventory> Decrease(int qty)

    {
        var previousLevel = StockLevel;
        if (AvailableStock < qty)
            return FinFail<Inventory>(InvalidOperationError.New(
                $"Insufficient stock for SKU {Sku}. Available: {AvailableStock}, Requested: {qty}"));


        Stock = Stock with { Value = Stock.Value - qty };
        if (previousLevel != StockLevel)
        {
            AddDomainEvent(new StockLevelChangedDomainEvent(
                ProductId.Value,
                VariantId.Value,
                StockLevel >= StockLevel.MediumStock,
                StockLevel));
        }
        if (StockLevel == StockLevel.LowStock)
            AddDomainEvent(new ProductLowStockDomainEvent(ProductId, VariantId, Sku, AvailableStock));

        if (StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockDomainEvent(
                ProductId, VariantId, Sku, StockLevel,
                $"{Slug} {Brand} (SKU: {Sku}) is out of stock"));

        return this;
    }

    public Fin<Inventory> Reserve(int qty)
    {
        var previousLevel = StockLevel;
        if (AvailableStock < qty)
            return FinFail<Inventory>(InvalidOperationError.New(
                $"Insufficient available stock for SKU {Sku}. Available: {AvailableStock}, Requested: {qty}"));

        Stock = Stock with { Value = Stock.Value - qty };
        Reserved += qty;

        if (previousLevel != StockLevel)
        {
            AddDomainEvent(new StockLevelChangedDomainEvent(
                ProductId.Value,
                VariantId.Value,
                StockLevel >= StockLevel.MediumStock,
                StockLevel));
        }
        if (StockLevel == StockLevel.LowStock)
            AddDomainEvent(new ProductLowStockDomainEvent(ProductId, VariantId, Sku, AvailableStock));

        if (StockLevel == StockLevel.OutOfStock)
            AddDomainEvent(new ProductOutOfStockDomainEvent(
                ProductId, VariantId, Sku, StockLevel,
                $"{Slug} {Brand} (SKU: {Sku}) is out of stock"));

        AddDomainEvent(new ProductReservedDomainEvent(ProductId, VariantId, Sku, qty, AvailableStock));

        return this;
    }
    public Inventory ReleaseReservation(int qty)
    {
        var previousLevel = StockLevel;
        var newStock = Stock.Value + qty;
        var newReserved = Math.Max(0, Reserved - qty);


        Stock = Stock with { Value = newStock };
        Reserved = newReserved;

        if (previousLevel != StockLevel)
        {
            AddDomainEvent(new StockLevelChangedDomainEvent(
                ProductId.Value,
                VariantId.Value,
                StockLevel >= StockLevel.MediumStock,
                StockLevel));
        }
        if ((previousLevel == StockLevel.OutOfStock || previousLevel == StockLevel.LowStock) &&
            (StockLevel == StockLevel.MediumStock || StockLevel == StockLevel.HighStock))
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(ProductId, VariantId, Sku, AvailableStock));
        }

        return this;
    }

    public Inventory ConfirmReservation(int qty)
    {
        return this with
        {
            Reserved = Math.Max(0, Reserved - qty)
        };
    }

    public bool IsAvailable(int requestedQuantity) => AvailableStock >= requestedQuantity;

    public Fin<Inventory> UpdateStock(int? low = null, int? mid = null, int? high = null)
    {
        return Stock.From((
            Stock.Value,
            low ?? Stock.Low,
            mid ?? Stock.Mid,
            high ?? Stock.High
        )).Map(newStock => { Stock = newStock; return this; });
    }
}

