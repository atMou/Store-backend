using Shared.Domain.Enums;

namespace Inventory.Domain.Models;


public record InventorySizeVariant
{
    private InventorySizeVariant()
    {
    }
    private InventorySizeVariant(
        Guid id,
        Size size,
        IEnumerable<Warehouse> warehouses,
        Stock stock)
    {
        Id = id;
        Size = size;
        Warehouses = warehouses.ToList();
        Stock = stock;
    }

    public Guid Id { get; private init; }
    public Size Size { get; private init; }
    public Stock Stock { get; private set; }
    public int Reserved { get; private set; }
    public ICollection<Warehouse> Warehouses { get; private set; } = [];


    public int AvailableStock => Stock.Value - Reserved;

    public StockLevel StockLevel =>
        AvailableStock switch
        {
            <= 0 => StockLevel.OutOfStock,
            var s when s <= Stock.Low => StockLevel.LowStock,
            var s when s <= Stock.Mid => StockLevel.MediumStock,
            _ => StockLevel.HighStock
        };


    public static Fin<InventorySizeVariant> Create(
        string size,
        int stock,
        int low,
        int mid,
        int high,
       IEnumerable<string> warehouses)
    {
        var ls = warehouses.AsIterable().Traverse(Warehouse.From);

        return (
            Stock.From((stock, low, mid, high)), Size.From(size), ls
        ).Apply((st, si, _locations) => new InventorySizeVariant(
                Guid.NewGuid(),
                si,
                _locations,
                st
            )
        ).As();


    }


    public Fin<InventorySizeVariant> Update(IEnumerable<string> locations, int? stock, int? low = null, int? mid = null, int? high = null)
    {
        var ls = locations.AsIterable().Traverse(Warehouse.From);
        var st = Stock.From((stock ?? Stock.Value, low ?? Stock.Low, mid ?? Stock.Mid, high ?? Stock.High));

        return (st, ls).Apply((_stock, _locations) =>
        {
            //if (_stock.Value > Stock.Value)
            //    notify((Size.Name, _stock.Value, true,
            //        $"Stock for size {Size.Name} is available."));
            //if (_stock.Value < Stock.Value)
            //    notify((Size.Name, _stock.Value, true,
            //        $"Stock for size {Size.Name} is Low Stock."));


            Stock = _stock;
            Warehouses = _locations.ToList();
            return this;

        }).As();
    }



    public Fin<InventorySizeVariant> IncreaseStock(int qty)
    {
        if (qty <= 0)
        {
            return FinFail<InventorySizeVariant>(InvalidOperationError.New($"Increasing stock with '{qty}' is not allowed"));
        }
        return this with { Stock = Stock with { Value = Stock.Value + qty } };
    }


    public Fin<InventorySizeVariant> DecreaseStock(int qty)
    {
        if (AvailableStock < qty)
            return FinFail<InventorySizeVariant>(InvalidOperationError.New(
                $"Insufficient stock for size {Size.Name}. Available: {AvailableStock}, Requested: {qty}"));

        return this with { Stock = Stock with { Value = Stock.Value - qty } };
    }

    public Fin<InventorySizeVariant> Reserve(int qty)
    {
        if (AvailableStock < qty)
            return FinFail<InventorySizeVariant>(InvalidOperationError.New(
                $"Insufficient available stock for size {Size.Name}. Available: {AvailableStock}, Requested: {qty}"));

        return this with
        {
            Stock = Stock with { Value = Stock.Value - qty },

            Reserved = Reserved + qty
        };
    }


    public InventorySizeVariant ReleaseReservation(int qty)
    {
        var newReserved = Math.Max(0, Reserved - qty);
        return this with
        {
            Stock = Stock with { Value = Stock.Value + qty },
            Reserved = newReserved
        };
    }

    public InventorySizeVariant ConfirmReservation(int qty)
    {
        return this with { Reserved = Math.Max(0, Reserved - qty) };
    }

    public bool IsAvailable(int requestedQuantity) => AvailableStock >= requestedQuantity;
}
