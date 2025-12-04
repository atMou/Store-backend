namespace Product.Domain.Contracts;

public record CreateAttributeDto
{
    public string Name { get; init; }
    public string Description { get; init; }
}
