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
			opt.AddInclude(p => p.Variants);
			var includes = query.Include.Split(',');

			foreach (string se in includes.Distinct())
			{
				if (string.Equals(se, Variants, StringComparison.OrdinalIgnoreCase))
				{
					opt = opt.AddInclude(p => p.Variants);
				}

				if (string.Equals(se, Reviews, StringComparison.OrdinalIgnoreCase))
				{
					opt = opt.AddInclude(p => p.Reviews);
				}

				if (string.Equals(se, Images, StringComparison.OrdinalIgnoreCase))
				{
					opt.AddInclude(p => p.Images);
				}
			}


		}
		return opt;
	}
}