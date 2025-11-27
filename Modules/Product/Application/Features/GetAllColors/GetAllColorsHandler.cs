namespace Product.Application.Features.GetAllColors;
public record GetAllColorsCommand : ICommand<IEnumerable<string>>
{
}
internal class GetAllColorsCommandHandler : ICommandHandler<GetAllColorsCommand, IEnumerable<string>>
{
    public Task<IEnumerable<string>> Handle(GetAllColorsCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Color.All.Select(c => c.Name));
    }
}

