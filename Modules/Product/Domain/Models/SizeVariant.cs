using Shared.Domain.Enums;

namespace Product.Domain.Models;

public record SizeVariant
{
    private SizeVariant()
    {
    }

    private SizeVariant(Guid id, Size size, int stock, StockLevel level)
    {
        Size = size;
        Stock = stock;
        StockLevel = level;
        Id = id;

    }

    public Guid Id { get; private init; }
    public Size Size { get; private init; }
    public Sku Sku { get; private set; }
    public StockLevel StockLevel { get; private set; } = StockLevel.None;
    public int Stock { get; private set; }
    public static SizeVariant Create(
        Guid id,
        string size,
        int stock,
        StockLevel level,
        string brand,
        string category,
        string color
        )
    {
        return new SizeVariant(id, Size.FromCodeUnsafe(size), stock, level)
        {
            Sku = Sku.From(
                category,
                color,
                size,
                brand
            )
        };
    }

    public SizeVariant BrandChangeHandle(string brand, string category, string color)
    {
        Sku = Sku.From(
            category,
            color,
            Size.Code.ToString(),
            brand
        );
        return this;
    }

    public SizeVariant CategoryChangeHandle(string category, string brand, string color)
    {
        Sku = Sku.From(
            category,
            color,
            Size.Code.ToString(),
            brand
        );
        return this;
    }
    public SizeVariant ColorChangeHandle(string color, string category, string brand)
    {
        Sku = Sku.From(
            category,
            color,
            Size.Code.ToString(),
            brand
        );
        return this;
    }
    public SizeVariant UpdateStock(int stock, StockLevel level)
    {
        StockLevel = level;
        Stock = stock;
        return this;
    }
}
