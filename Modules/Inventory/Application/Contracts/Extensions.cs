using Inventory.Application.Features.UpdateInventory;

using Shared.Application.Contracts.Inventory.Results;
using Shared.Application.Contracts.Product.Results;

using SixLabors.ImageSharp;

using Color = Shared.Domain.ValueObjects.Color;

namespace Inventory.Application.Contracts;

public static class Extensions
{
    public static UpdateInventoryDto ToDto(this UpdateInventoryCommand command)
    {
        return new UpdateInventoryDto
        {
            Id = InventoryId.From(command.Id),

            ColorVariants = command.ColorVariants.Select(cv => new UpdateInventoryColorDto
            {
                ColorVariantId = ColorVariantId.From(cv.ColorVariantId),
                SizeVariants = cv.SizeVariants.Select(sv => new UpdateInventorySizeDto
                {
                    SizeVariantId = sv.SizeVariantId,
                    Size = sv.Size,
                    Stock = sv.Stock,
                    Low = sv.Low,
                    Mid = sv.Mid,
                    High = sv.High,
                    Warehouses = sv.Warehouses.ToList()
                }).ToList()
            }).ToList()
        };
    }

    public static InventoryResult ToResult(this Domain.Models.Inventory inventory)
    {
        return new InventoryResult
        {
            Id = inventory.Id.Value,
            ProductId = inventory.ProductId.Value,
            ImageUrl = inventory.ImageUrl,
            Brand = inventory.Brand,
            Slug = inventory.Slug,
            TotalStock = inventory.TotalStock,
            TotalReserved = inventory.TotalReserved,
            TotalAvailableStock = inventory.TotalAvailableStock,
            ColorVariants = inventory.ColorVariants.Select(cv => new ColorVariantResult
            {
                Id = cv.ColorVariantId.Value,
                Color = new ColorResult()
                {
                    Name = Color.FromCodeUnsafe(cv.Color).Name,
                    Hex = Color.FromCodeUnsafe(cv.Color).Hex,
                },
                SizeVariants = cv.SizeVariants.Select(sv => new Shared.Application.Contracts.Inventory.Results.SizeVariantResult
                {
                    Id = sv.Id,
                    Size = new SizeResult
                    {
                        Code = sv.Size.Code.ToString(),
                        Name = sv.Size.Name
                    },
                    Stock = new StockResult
                    {
                        Stock = sv.Stock.Value,
                        Low = sv.Stock.Low,
                        Mid = sv.Stock.Mid,
                        High = sv.Stock.High
                    },
                    Reserved = sv.Reserved,
                    AvailableStock = sv.AvailableStock,
                    Warehouses = sv.Warehouses.Select(w => new WarehouseResult
                    {
                        Code = w.Code.ToString(),
                        Name = w.Name,
                        Address = w.Address,
                        City = w.City,
                        State = w.State,
                        Country = w.Country,
                        PostalCode = w.PostalCode,
                        ContactPhone = w.ContactPhone,
                        ContactEmail = w.ContactEmail
                    })
                })
            }).ToList()
        };
    }

    //public static InventoryResult ToResult(this Domain.Models.Inventory inventory, ColorVariantId colorVariantId)
    //{
    //    var colorVariant = inventory.ColorVariants.FirstOrDefault(cv => cv.ColorVariantId == colorVariantId);

    //    if (colorVariant == null)
    //    {
    //        return new InventoryResult
    //        {
    //            Id = inventory.Id.Value,
    //            ProductId = inventory.ProductId.Value,
    //            Brand = inventory.Brand,
    //            Slug = inventory.Slug,
    //            TotalStock = 0,
    //            TotalReserved = 0,
    //            TotalAvailableStock = 0,
    //            ColorVariants = new List<Shared.Application.Contracts.Inventory.Results.ColorVariantResult>()
    //        };
    //    }

    //    return new InventoryResult
    //    {
    //        Id = inventory.Id.Value,
    //        ProductId = inventory.ProductId.Value,
    //        Brand = inventory.Brand,
    //        Slug = inventory.Slug,
    //        TotalStock = colorVariant.SizeVariants.Sum(sv => sv.Stock.Value),
    //        TotalReserved = colorVariant.SizeVariants.Sum(sv => sv.Reserved),
    //        TotalAvailableStock = colorVariant.SizeVariants.Sum(sv => sv.AvailableStock),
    //        ColorVariants = new List<ColorVariantResult>
    //        {
    //            new ColorVariantResult
    //            {
    //                Id = colorVariant.ColorVariantId.Value,
    //                Color = colorVariant.Color,
    //                SizeVariants = colorVariant.SizeVariants.Select(sv => new Shared.Application.Contracts.Inventory.Results.SizeVariantResult
    //                {
    //                    Id = sv.Id,
    //                    Size = new SizeResult
    //                    {
    //                        Code = sv.Size.Code.ToString(),
    //                        Name = sv.Size.Name
    //                    },
    //                    Stock = new StockResult
    //                    {
    //                        Stock = sv.Stock.Value,
    //                        Low = sv.Stock.Low,
    //                        Mid = sv.Stock.Mid,
    //                        High = sv.Stock.High
    //                    },
    //                    Reserved = sv.Reserved,
    //                    AvailableStock = sv.AvailableStock,
    //                    Warehouses = sv.Warehouses.Select(w => new WarehouseResult
    //                    {
    //                        Code = w.Code.ToString(),
    //                        Name = w.Name,
    //                        Address = w.Address,
    //                        City = w.City,
    //                        State = w.State,
    //                        Country = w.Country,
    //                        PostalCode = w.PostalCode,
    //                        ContactPhone = w.ContactPhone,
    //                        ContactEmail = w.ContactEmail
    //                    })
    //                })
    //            }
    //        }
    //    };
    //}
}
