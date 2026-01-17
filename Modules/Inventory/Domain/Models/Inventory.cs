using Inventory.Domain.Events;

using Shared.Application.Features.Product.Events;
using Shared.Domain.Enums;

namespace Inventory.Domain.Models;

public class Inventory : Aggregate<InventoryId>
{
    private Inventory() : base(InventoryId.New())
    {
    }

    private Inventory(
        ProductId productId,
        string brand,
        string slug,
        string imageUrl,
        IEnumerable<InventoryColorVariant> colorVariants)
        : base(InventoryId.New())
    {
        ProductId = productId;
        Brand = brand;
        Slug = slug;
        ImageUrl = imageUrl;
        ColorVariants = colorVariants.ToList();
    }

    public ProductId ProductId { get; }
    public string Brand { get; }
    public string ImageUrl { get; }
    public string Slug { get; }
    public byte[] Version { get; private set; }
    public ICollection<InventoryColorVariant> ColorVariants { get; private set; } = [];

    private IEnumerable<InventorySizeVariant> SizeVariants =>
        ColorVariants.SelectMany(cv => cv.SizeVariants);

    public int TotalAvailableStock => SizeVariants.Sum(sv => sv.AvailableStock);

    public int TotalStock => SizeVariants.Sum(sv => sv.Stock.Value);

    public int TotalReserved => SizeVariants.Sum(sv => sv.Reserved);

    public static Inventory Create(
        ProductId productId,
        string brand,
        string slug,
        string imageUrl,
        IEnumerable<CreateColorVariantDto> colorVariants
    )
    {
        var cvs = colorVariants.Select(dto => InventoryColorVariant.Create(ColorVariantId.From(dto.ColorVariantId), dto.Color));
        return new Inventory(
            productId,
            brand,
            slug,
            imageUrl,
            cvs
        );
    }

    public Fin<(InventoryColorVariant ColorVariant, InventorySizeVariant SizeVariant)> GetSizeVariant(ColorVariantId colorVariantId, string sizeName)
    {
        var colorVariant = ColorVariants.FirstOrDefault(cv => cv.ColorVariantId == colorVariantId);
        if (colorVariant is null)
            return FinFail<(InventoryColorVariant, InventorySizeVariant)>(
                NotFoundError.New($"Color variant with ID '{colorVariantId}' not found"));

        var sizeVariant = colorVariant.SizeVariants
            .FirstOrDefault(sv => sv.Size.Name.Equals(sizeName, StringComparison.OrdinalIgnoreCase));

        if (sizeVariant is null)
            return FinFail<(InventoryColorVariant, InventorySizeVariant)>(
                NotFoundError.New($"Size variant '{sizeName}' not found for color variant '{colorVariantId}'"));

        return ((colorVariant, sizeVariant));
    }

    public Fin<Inventory> IncreaseStock(ColorVariantId colorVariantId, string sizeName, int qty)
    {
        return GetSizeVariant(colorVariantId, sizeName)
            .Bind(result =>
            {
                var (colorVariant, sizeVariant) = result;
                return sizeVariant.IncreaseStock(qty).Map(updatedSv =>
                {
                    var previousLevel = sizeVariant.StockLevel;

                    colorVariant.UpdateSizeVariant(updatedSv);

                    if ((previousLevel == StockLevel.OutOfStock || previousLevel == StockLevel.LowStock) &&
                        (updatedSv.StockLevel == StockLevel.MediumStock || updatedSv.StockLevel == StockLevel.HighStock))
                        AddDomainEvent(new ProductBackInStockDomainEvent(
                            ProductId, colorVariantId, updatedSv.AvailableStock));

                    return this;
                });
            });
    }

    public Fin<Inventory> DecreaseStock(ColorVariantId colorVariantId, string sizeName, int qty)
    {
        return GetSizeVariant(colorVariantId, sizeName).Bind(result =>
        {
            var (colorVariant, sizeVariant) = result;
            var previousLevel = sizeVariant.StockLevel;

            return sizeVariant.DecreaseStock(qty).Map(updatedSv =>
            {
                colorVariant.UpdateSizeVariant(updatedSv);

                if (previousLevel != updatedSv.StockLevel)
                    AddDomainEvent(new StockLevelChangedDomainEvent(
                        ProductId.Value,
                        colorVariantId.Value,
                        sizeName,
                        updatedSv.StockLevel >= StockLevel.MediumStock,
                        updatedSv.StockLevel));

                if (updatedSv.StockLevel == StockLevel.LowStock)
                    AddDomainEvent(new ProductLowStockDomainEvent(
                        ProductId, colorVariantId, updatedSv.AvailableStock));

                if (updatedSv.StockLevel == StockLevel.OutOfStock)
                    AddDomainEvent(new ProductOutOfStockDomainEvent(
                        ProductId,
                        colorVariant.Color,
                        updatedSv.Size.Name,
                        $"{Slug} {Brand} {colorVariant.Color} ( Size: {sizeName}) is out of stock)",
                        updatedSv.StockLevel,
                        false,
                        updatedSv.AvailableStock,
                        Slug

                    ));

                return this;
            });
        });
    }

    public Fin<Inventory> ReserveStock(ColorVariantId colorVariantId, string sizeName, int qty)
    {
        return GetSizeVariant(colorVariantId, sizeName).Bind(result =>
        {
            var (colorVariant, sizeVariant) = result;
            var previousLevel = sizeVariant.StockLevel;

            return sizeVariant.Reserve(qty).Map(updatedSv =>
            {
                colorVariant.UpdateSizeVariant(updatedSv);

                if (previousLevel != updatedSv.StockLevel)
                    AddDomainEvent(new StockLevelChangedDomainEvent(
                        ProductId.Value,
                        colorVariantId.Value,
                        sizeName,
                        updatedSv.StockLevel >= StockLevel.MediumStock,
                        updatedSv.StockLevel));

                if (updatedSv.StockLevel == StockLevel.LowStock)
                    AddDomainEvent(new ProductLowStockDomainEvent(
                        ProductId, colorVariantId, updatedSv.AvailableStock));

                if (updatedSv.StockLevel == StockLevel.OutOfStock)
                    AddDomainEvent(new ProductOutOfStockDomainEvent(
                        ProductId,
                        colorVariant.Color,
                        updatedSv.Size.Name,
                        $"{Slug} {Brand} {colorVariant.Color} ( Size: {sizeName}) is out of stock)",
                        updatedSv.StockLevel,
                        false,
                        updatedSv.AvailableStock,
                        Slug
                    ));

                AddDomainEvent(new ProductReservedDomainEvent(
                    ProductId, colorVariantId, qty, updatedSv.AvailableStock));

                return this;
            });
        });
    }

    public Fin<Inventory> ReleaseReservation(ColorVariantId colorVariantId, string sizeName, int qty)
    {
        return GetSizeVariant(colorVariantId, sizeName).Map(result =>
        {
            var (colorVariant, sizeVariant) = result;
            var previousLevel = sizeVariant.StockLevel;
            var updatedSv = sizeVariant.ReleaseReservation(qty);

            colorVariant.UpdateSizeVariant(updatedSv);

            if (previousLevel != updatedSv.StockLevel)
                AddDomainEvent(new StockLevelChangedDomainEvent(
                    ProductId.Value,
                    colorVariantId.Value,
                    sizeName,
                    updatedSv.StockLevel >= StockLevel.MediumStock,
                    updatedSv.StockLevel));

            if ((previousLevel == StockLevel.OutOfStock || previousLevel == StockLevel.LowStock) &&
                (updatedSv.StockLevel == StockLevel.MediumStock || updatedSv.StockLevel == StockLevel.HighStock))
                AddDomainEvent(new ProductBackInStockDomainEvent(
                    ProductId, colorVariantId, updatedSv.AvailableStock));

            return this;
        });
    }

    public Fin<Inventory> ConfirmReservation(ColorVariantId colorVariantId, string sizeName, int qty)
    {
        return GetSizeVariant(colorVariantId, sizeName).Map(result =>
        {
            var (colorVariant, sizeVariant) = result;
            var updatedSv = sizeVariant.ConfirmReservation(qty);

            colorVariant.UpdateSizeVariant(updatedSv);

            return this;
        });
    }

    public bool IsAvailable(ColorVariantId colorVariantId, string sizeName, int requestedQuantity)
    {
        return GetSizeVariant(colorVariantId, sizeName)
            .Map(result => result.SizeVariant.IsAvailable(requestedQuantity))
            .IfFail(false);
    }

    public Fin<Inventory> UpdateSizeVariant(ColorVariantId colorVariantId, string sizeName, int stock, int low, int mid, int high, IEnumerable<string> warehouses)
    {
        return GetSizeVariant(colorVariantId, sizeName).BiBind(
            result =>
            {
                var (colorVariant, sizeVariant) = result;
                return sizeVariant.Update(warehouses, stock, low, mid, high).Map(updatedSv =>
                {
                    colorVariant.UpdateSizeVariant(updatedSv);
                    return this;
                });
            },
            _ =>
            {
                var colorVariant = ColorVariants.FirstOrDefault(cv => cv.ColorVariantId == colorVariantId);
                if (colorVariant is null)
                    return FinFail<Inventory>(NotFoundError.New($"Color variant with ID '{colorVariantId}' not found"));

                return InventorySizeVariant.Create(sizeName, stock, low, mid, high, warehouses).Map(sv =>
                {
                    colorVariant.AddSizeVariant(sv);
                    return this;
                });
            });
    }

    public Fin<Inventory> CreateSizeVariant(
        ColorVariantId colorVariantId,
        string sizeName,
        int stock,
        int low,
        int mid,
        int high,
        IEnumerable<string> warehouseCode)
    {
        var colorVariant = ColorVariants.FirstOrDefault(cv => cv.ColorVariantId == colorVariantId);
        if (colorVariant is null)
            return FinFail<Inventory>(NotFoundError.New($"Color variant with ID '{colorVariantId}' not found"));

        if (colorVariant.SizeVariants.Any(sv => sv.Size.Name.Equals(sizeName, StringComparison.OrdinalIgnoreCase)))
            return FinFail<Inventory>(ValidationError.New($"Size variant '{sizeName}' already exists for this color variant"));

        return InventorySizeVariant.Create(sizeName, stock, low, mid, high, warehouseCode).Map(newSizeVariant =>
        {
            colorVariant.AddSizeVariant(newSizeVariant);
            return this;
        });
    }

    public Fin<Inventory> RemoveSizeVariant(ColorVariantId colorVariantId, string sizeName)
    {
        return GetSizeVariant(colorVariantId, sizeName).Map(result =>
        {
            var (colorVariant, sizeVariant) = result;
            colorVariant.RemoveSizeVariant(sizeVariant.Id);
            return this;
        });
    }

    public Fin<Inventory> Update(UpdateInventoryDto dto)
    {
        return dto.ColorVariants.AsIterable().Traverse(cvDto =>
            Optional(ColorVariants.FirstOrDefault(cv => cv.ColorVariantId.Value == cvDto.ColorVariantId.Value))
                .ToFin(NotFoundError.New($"Color variant with ID '{cvDto.ColorVariantId.Value}' not found"))
                .Bind(colorVariant => colorVariant.UpdateSizeVariants(cvDto.SizeVariants))
        ).Map(_ => this).As();
    }
}