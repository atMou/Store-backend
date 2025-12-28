namespace Product.Application.Features.GetAllCategories;
public record GetAllCategoriesCommand : ICommand<IEnumerable<CategoryResult>>
{
}
internal class GetAllCategoriesCommandHandler : ICommandHandler<GetAllCategoriesCommand, IEnumerable<CategoryResult>>
{
	public Task<IEnumerable<CategoryResult>> Handle(GetAllCategoriesCommand request, CancellationToken cancellationToken)
	{
		var all = Category.All.Distinct();
		var results = all.Select(c => c.ToResult()).ToList();
		return Task.FromResult(results.AsEnumerable());
	}

}