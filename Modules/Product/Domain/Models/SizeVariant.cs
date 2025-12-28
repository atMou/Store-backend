using Shared.Domain.Enums;

namespace Product.Domain.Models;

public record SizeVariant
{
    private SizeVariant() { }
    public SizeVariant(Size size)
    {
        Size = size;
    }

    public Size Size { get; private set; }
    public bool IsInStock { get; private set; } = true;
    public StockLevel StockLevel { get; private set; } = StockLevel.HighStock;


    public static SizeVariant Create(Size size)
    {
        return new SizeVariant(size);
    }
    public SizeVariant UpdateStock(bool inStock, StockLevel level)
    {
        IsInStock = inStock;
        StockLevel = level;
        return this;
    }
}
