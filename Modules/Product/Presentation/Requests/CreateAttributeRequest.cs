using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;

public record CreateAttributeRequest
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    public CreateAttributeCommand ToCommand() => new CreateAttributeCommand
    {
        Name = Name,
        Description = Description
    };
}