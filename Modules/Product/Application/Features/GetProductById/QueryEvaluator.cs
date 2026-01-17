namespace Product.Application.Features.GetProductById;

public static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Product> Evaluate(QueryOptions<Domain.Models.Product> opt,
        GetProductByIdQuery query)
    {

        if (!string.IsNullOrEmpty(query.Include))
        {
            opt.AsSplitQuery = true;
            opt.AsNoTracking = true;
            opt.AddInclude(p => p.ColorVariants);
            var includes = query.Include.Split(',');

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, Variants, StringComparison.OrdinalIgnoreCase))
                {
                    opt = opt.AddInclude(p => p.ColorVariants);
                }

                if (string.Equals(se, Reviews, StringComparison.OrdinalIgnoreCase))
                {
                    opt = opt.AddInclude(p => p.Reviews);
                }

            }


        }
        return opt;
    }
}