namespace Product.Application.Features.GetMaterials;
public record GetMaterialsCommand : ICommand<IEnumerable<string>>
{
}
internal class GetMaterialsCommandHandler : ICommandHandler<GetMaterialsCommand, IEnumerable<string>>
{
    public Task<IEnumerable<string>> Handle(GetMaterialsCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Material.All.Select(m => m.Name));
    }
}

