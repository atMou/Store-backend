namespace Product.Application.Features.GetAllBrands;
public record GetAllBrandsCommand : ICommand<IEnumerable<string>>
{
}
internal class GetAllBrandsCommandHandler : ICommandHandler<GetAllBrandsCommand, IEnumerable<string>>
{
	public Task<IEnumerable<string>> Handle(GetAllBrandsCommand request, CancellationToken cancellationToken)
	{
		return Task.FromResult(Brand.All.Select(b => b.Name));
	}
}

