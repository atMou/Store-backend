namespace Product.Application.Features.GetAllCategories;
public record GetAllCategoriesCommand : ICommand<IEnumerable<string>>
{
}
internal class GetAllCategoriesCommandHandler : ICommandHandler<GetAllCategoriesCommand, IEnumerable<string>>
{
    public Task<IEnumerable<string>> Handle(GetAllCategoriesCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Brand.All.Select(c => c.Name));
    }
}

