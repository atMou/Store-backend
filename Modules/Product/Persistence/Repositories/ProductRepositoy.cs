
using Product.Application.Features.GetProducts;

namespace Product.Persistence.Repositories;


public class ProductRepository : IProductRepository
{


    public IO<(List<Domain.Models.Product> Products, int TotalCount)> GetProducts(ProductDBContext ctx,
        GetProductsQuery query)
    {

        return IO.liftAsync(async e =>
        {
            //var selector = QueryableExtensions.BuildSelector<Domain.Models.Product>(query.Fields, query.Include);

            return await ctx.Products.WithQueryOptions(options => { ProductOptionsEvaluator(options, query); })
                //.WithProjection<Domain.Models.Product, ProductDto>(query.Fields, query.Include)
                .GroupBy(_ => 1)
                .Select(g => new { TotalCount = g.Count(), Items = g.ToList() })
                .FirstOrDefaultAsync(e.Token)
                .Map(res => (res!.Items, res.TotalCount));
        });
    }

    public IO<Domain.Models.Product> GetProductById(ProductId id, ProductDBContext ctx,
        Action<QueryOptions<Domain.Models.Product>>? options = null)
    {
        return from c in IO<Domain.Models.Product?>.LiftAsync(async e => await
                ctx.Products.WithQueryOptions(options)
                    .FirstOrDefaultAsync(product => product.Id == id, e.Token))
               from a in when(c is null, IO.fail<Unit>(NotFoundError.New($"Product with id '{id}' was not found.")))
               select c;
    }




    public IO<List<Domain.Models.Product>> GetProductsByIds(IEnumerable<ProductId> productIds, ProductDBContext ctx,
        Action<QueryOptions<Domain.Models.Product>>? options = null)
    {
        var ids = productIds.Select(id => id.Value).ToList();
        return IO.liftAsync(async e =>
            await ctx.Products.WithQueryOptions(options)
                .Where(p => ids.Contains(p.Id.Value))
                .ToListAsync(e.Token));
    }
    //public IO<ProductId> UpdateProductPrice(ProductId productId, decimal newPrice, ProductDBContext ctx)
    //{
    //    return from p in GetProductById(productId, ctx)
    //           let product = p.UpdatePrice(newPrice)
    //           select p.Id;

    //}
    public IO<bool> DeleteProduct(ProductId productId, ProductDBContext ctx)
    {
        return from product in GetProductById(productId, ctx)
               from _ in IO.lift(() => ctx.Products.Remove(product))
               select true;
    }



    private QueryOptions<Domain.Models.Product> ProductOptionsEvaluator(QueryOptions<Domain.Models.Product> options, GetProductsQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize

        };

        if (!string.IsNullOrWhiteSpace(query.Brand))
            options = options.AddFilters(p => p.Brand.Name.Contains(query.Brand));


        if (!string.IsNullOrWhiteSpace(query.Category))
            options = options.AddFilters(p => p.Category.Name.Contains(query.Category));

        if (!string.IsNullOrWhiteSpace(query.Color))
            options = options.AddFilters(p => p.Color.Name.Contains(query.Color));

        if (!string.IsNullOrWhiteSpace(query.Size))
            options = options.AddFilters(p => p.Size.Name.Contains(query.Size));

        if (!string.IsNullOrWhiteSpace(query.Search))
            options = options.AddFilters(p => p.Slug.Value.Contains(query.Search));

        if (query.MinPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value <= query.MaxPrice.Value);

        if (query.Include is { Length: > 0 })
        {
            foreach (string se in query.Include)
            {
                if (string.Equals(se, "variants", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Variants);
                }

                if (string.Equals(se, "reviews", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Reviews);
                }

            }
        }



        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var sortBy = query.OrderBy.ToLowerInvariant();

            options = sortBy switch
            {
                "price" => options.AddOrderBy(p => p.Price.Value),
                "brand" => options.AddOrderBy(p => p.Brand.Name),
                "totalsales" => options.AddOrderBy(p => p.TotalSales),
                "totalreviews" => options.AddOrderBy(p => p.TotalReviews),
                "averagerating" => options.AddOrderBy(p => p.AvgRating.Value),
                _ => options
            };
        }

        if (!string.IsNullOrWhiteSpace(query.SortDir))
        {
            var sortDir = query.SortDir.ToLowerInvariant();
            options = sortDir switch
            {
                "asc" => options.AddSortDirAsc(),
                "desc" => options.AddSortDirDesc(),
                _ => options
            };
        }

        return options;
    }


}