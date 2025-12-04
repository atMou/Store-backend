namespace Product.Domain.Contracts;

public record UpdateAttributeDto
{
    public string Name { get; init; }
    public string Description { get; init; }
}
