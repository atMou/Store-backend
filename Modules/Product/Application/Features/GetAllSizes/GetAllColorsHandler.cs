namespace Product.Application.Features.GetAllSizes;
public record GetAllSizesCommand : ICommand<IEnumerable<string>>
{
}
internal class GetAllSizesCommandHandler : ICommandHandler<GetAllSizesCommand, IEnumerable<string>>
{
	public Task<IEnumerable<string>> Handle(GetAllSizesCommand request, CancellationToken cancellationToken)
	{
		return Task.FromResult(Size.All.Select(c => c.Name));
	}
}

