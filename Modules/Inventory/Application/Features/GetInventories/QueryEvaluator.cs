using Shared.Persistence.Extensions;

namespace Inventory.Application.Features.GetInventories;

public static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Inventory> Evaluate(
        QueryOptions<Domain.Models.Inventory> options,
        GetInventoriesQuery query)
    {
        options = options with
        {
            AsNoTracking = true,
            WithPagination = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        // Include ColorVariants for all queries since we need them to access SizeVariants
        options = options.AddInclude(i => i.ColorVariants);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchTerm = query.Search.ToLower();
            options = options.AddFilters(i =>
                i.Brand.ToLower().Contains(searchTerm) ||
                i.Slug.ToLower().Contains(searchTerm) ||
                i.ColorVariants.Any(cv => cv.SizeVariants.Any(sv => sv.Size.Name.ToLower().Contains(searchTerm))));
        }

        // Apply brand filter
        if (!string.IsNullOrWhiteSpace(query.Brand))
        {
            options = options.AddFilters(i => i.Brand.ToLower() == query.Brand.ToLower());
        }

        // Apply size filter
        if (!string.IsNullOrWhiteSpace(query.Size))
        {
            options = options.AddFilters(i =>
                i.ColorVariants.Any(cv => cv.SizeVariants.Any(sv => sv.Size.Name.ToLower() == query.Size.ToLower())));
        }

        // Apply warehouse filter - checks if any warehouse in any size variant matches
        if (!string.IsNullOrWhiteSpace(query.WarehouseCode))
        {
            var warehouseLocation = Warehouse.FromUnsafe(query.WarehouseCode);
            options = options.AddFilters(i =>
                i.ColorVariants.Any(cv =>
                    cv.SizeVariants.Any(sv =>
                        sv.Warehouses.Any(wh => wh.Code == warehouseLocation.Code))));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var isDescending = query.SortDir?.ToLower() == "desc";

            options = query.OrderBy.ToLower() switch
            {
                "brand" => options.AddOrderBy(i => i.Brand),
                "size" => options.AddOrderBy(i => i.ColorVariants.Sum(cv => cv.SizeVariants.Count)),
                "stock" => options.AddOrderBy(i => i.TotalStock),
                "available" => options.AddOrderBy(i => i.TotalAvailableStock),
                "warehouse" => options.AddOrderBy(i =>
                    i.ColorVariants
                        .SelectMany(cv => cv.SizeVariants)
                        .SelectMany(sv => sv.Warehouses)
                        .Select(wh => wh.Code)
                        .FirstOrDefault()),
                _ => options
            };

            if (isDescending)
            {
                options = options.AddSortDirDesc();
            }
            else
            {
                options = options.AddSortDirAsc();
            }
        }

        return options;
    }
}
