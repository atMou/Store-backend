using System.Linq.Expressions;

using Expression = System.Linq.Expressions.Expression;

namespace Product.Persistence.Extensions;
public static class QueryableExtensions
{
    public static IQueryable<TResult> WithProjection<TAggregate, TResult>(
        this IQueryable<TAggregate> queryable,
        IEnumerable<string> fields,
        IEnumerable<string>? includes = null)
        where TAggregate : class, IAggregate
        where TResult : class, new()
    {
        var selectedFields = (fields as string[] ?? fields.ToArray())
            .Select(f => f.Trim())
            .Where(f => !string.IsNullOrEmpty(f))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (includes != null)
        {
            foreach (var include in includes)
                queryable = queryable.Include(include);
        }

        if (!selectedFields.Any())
            return queryable.Select(e => new TResult());

        var entityParam = Expression.Parameter(typeof(TAggregate), "e");
        var bindings = new List<MemberBinding>();

        var dtoProperties = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var dtoProp in dtoProperties)
        {
            if (!dtoProp.CanWrite) continue;
            if (!selectedFields.Contains(dtoProp.Name)) continue;

            var expression = BuildPropertyExpression(entityParam, dtoProp.Name, dtoProp.PropertyType);
            if (expression == null) continue;

            // Handle type conversion (e.g. int → int?, Guid → string, etc.)
            if (expression.Type != dtoProp.PropertyType)
            {
                if (CanConvert(expression.Type, dtoProp.PropertyType))
                {
                    expression = Expression.Convert(expression, dtoProp.PropertyType);
                }
                else
                {
                    expression = Expression.Default(dtoProp.PropertyType);
                }
            }

            bindings.Add(Expression.Bind(dtoProp, expression));
        }

        var newExpr = Expression.New(typeof(TResult));
        var initExpr = Expression.MemberInit(newExpr, bindings);
        var lambda = Expression.Lambda<Func<TAggregate, TResult>>(initExpr, entityParam);

        return queryable.Select(lambda);
    }
    private static Expression? BuildPropertyExpression(Expression e, string fieldName, Type targetType)
    {
        return fieldName switch
        {
            // Simple Value Objects
            "Id" => Expr(e, p => p.Id.Value),
            "Slug" => Expr(e, p => p.Slug.Value),
            "Sku" => Expr(e, p => p.Sku.Value),
            "Brand" => Expr(e, p => p.Brand.Name),
            "Size" => Expr(e, p => p.Size.Name),
            "Color" => Expr(e, p => p.Color.Name),
            "ColorHex" => Expr(e, p => p.Color.Hex),
            "Price" => Expr(e, p => p.Price.Value),
            "Category" => Expr(e, p => p.Category.Name),
            "Description" => Expr(e, p => p.Description.Value),
            //"Stock" => Expr(e, p => p.Stock.Value),
            //"AverageRating" => Expr(e, p => p.AvgRating.Value),

            "NewPrice" => Expression.Condition(
                Expression.ReferenceEqual(Expression.Property(e, "NewPrice"), Expression.Constant(null)),
                Expression.Constant(null, typeof(decimal?)),
                Expression.Property(Expression.Property(e, "NewPrice"), "Value")),

            "Discount" => Expression.Condition(
                Expression.ReferenceEqual(Expression.Property(e, "Discount"), Expression.Constant(null)),
                Expression.Constant(null, typeof(decimal?)),
                Expression.Property(Expression.Property(e, "Discount"), "Value")),

            "StockLevel" => Expression.Call(Expression.Property(e, "StockLevel"), "ToString", Type.EmptyTypes),

            // Status flags
            "IsNew" => Expr(e, p => p.Status.IsNew),
            "IsFeatured" => Expr(e, p => p.Status.IsFeatured),
            "IsBestSeller" => Expr(e, p => p.Status.IsBestSeller),
            "IsTrending" => Expr(e, p => p.Status.IsTrending),

            // Primitives
            "TotalReviews" => Expression.Property(e, "TotalReviews"),
            "TotalSales" => Expression.Property(e, "TotalSales"),

            _ => null
        };
    }

    // Helper that works because it's inside the generic method
    private static Expression Expr(Expression e, Expression<Func<Domain.Models.Product, object>> selector)
    {
        var replaced = new ParameterReplacer((ParameterExpression)e).Visit(selector.Body);
        return replaced!;
    }

    class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _target;
        public ParameterReplacer(ParameterExpression target) => _target = target;
        protected override Expression VisitParameter(ParameterExpression node)
            => node.Type == _target.Type ? _target : node;
    }
    //private static Expression? BuildPropertyExpression(Expression baseExpr, string fieldName, Type targetType)
    //{
    //    // Map DTO property → actual expression on entity (with Value Object unwrapping)
    //    return fieldName switch
    //    {
    //        nameof(ProductResult.Id) => Expr(e => ((Domain.Models.Product)e).Id.Value),
    //        nameof(ProductResult.Slug) => Expr(e => ((Domain.Models.Product)e).Slug.Value),
    //        nameof(ProductResult.Sku) => Expr(e => ((Domain.Models.Product)e).Sku.Value),
    //        nameof(ProductResult.Brand) => Expr(e => ((Domain.Models.Product)e).Brand.Name),
    //        nameof(ProductResult.Size) => Expr(e => ((Domain.Models.Product)e).Size.Name),
    //        nameof(ProductResult.Color) => Expr(e => ((Domain.Models.Product)e).Color.Name),
    //        nameof(ProductResult.ColorHex) => Expr(e => ((Domain.Models.Product)e).Color.Hex),
    //        nameof(ProductResult.Price) => Expr(e => ((Domain.Models.Product)e).Price.Value),
    //        nameof(ProductResult.NewPrice) => Expr(e => ((Domain.Models.Product)e).NewPrice == null ? (decimal?)null : ((Domain.Models.Product)e).NewPrice.Value),
    //        nameof(ProductResult.Discount) => Expr(e => ((Domain.Models.Product)e).Discount == null ? (decimal?)null : ((Domain.Models.Product)e).Discount.Value),
    //        nameof(ProductResult.Category) => Expr(e => ((Domain.Models.Product)e).Category.Name),
    //        nameof(ProductResult.Description) => Expr(e => ((Domain.Models.Product)e).Description.Value),
    //        nameof(ProductResult.Stock) => Expr(e => ((Domain.Models.Product)e).Stock.Value),
    //        nameof(ProductResult.AverageRating) => Expr(e => ((Domain.Models.Product)e).AvgRating.Value),
    //        nameof(ProductResult.StockLevel) => Expr(e => ((Domain.Models.Product)e).StockLevel.ToString()),
    //        nameof(ProductResult.IsNew) => Expr(e => ((Domain.Models.Product)e).Status.IsNew),
    //        nameof(ProductResult.IsFeatured) => Expr(e => ((Domain.Models.Product)e).Status.IsFeatured),
    //        nameof(ProductResult.IsBestSeller) => Expr(e => ((Domain.Models.Product)e).Status.IsBestSeller),
    //        nameof(ProductResult.IsTrending) => Expr(e => ((Domain.Models.Product)e).Status.IsTrending),
    //        nameof(ProductResult.TotalReviews) => Expr(e => ((Domain.Models.Product)e).TotalReviews),
    //        nameof(ProductResult.TotalSales) => Expr(e => ((Domain.Models.Product)e).TotalSales),

    //        nameof(ProductResult.Images) => Expr(e => (IEnumerable<ImageDto>)((Domain.Models.Product)e)
    //            .ProductImages.Select(pi => new ImageDto
    //            {
    //                Id = pi.Id.Value,
    //                Url = pi.ImageUrl.Value,
    //                AltText = pi.AltText,
    //                IsMain = pi.IsMain
    //            })),

    //        nameof(ProductResult.Variants) => Expr(e => (IEnumerable<ProductVariantResult>)((Domain.Models.Product)e)
    //            .Variants.Select(v => new ProductVariantResult
    //            {
    //                Id = v.Id.Value,
    //                Sku = v.Sku.Value,
    //                Size = v.Size.Name,
    //                Color = v.Color.Name,
    //                ColorHex = v.Color.Hex,
    //                Price = v.Price.Value,
    //                Stock = v.Stock.Value
    //            })),

    //        nameof(ProductResult.Reviews) => Expr(e => (IEnumerable<ReviewResult>)((Domain.Models.Product)e)
    //            .Reviews.Select(r => new ReviewResult
    //            {
    //                Id = r.Id.Value,
    //                UserId = r.UserId.Value,
    //                ProductId = r.ProductId.Value,
    //                Comment = r.Comment.Value,
    //                Rating = r.Rating.Value,
    //                RatingDescription = r.Rating.Description
    //            })),

    //        _ => null
    //    };
    //}

    //private static Expression Expr<T>(Expression<Func<TAggregate, T>> expr) => expr.Body;

    private static bool CanConvert(Type from, Type to)
    {
        if (to.IsAssignableFrom(from)) return true;
        if (to == typeof(string)) return true;
        if (Nullable.GetUnderlyingType(to) is Type underlying && underlying == from) return true;
        return false;
    }
}