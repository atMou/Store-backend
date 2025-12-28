namespace Basket.Domain.Contracts;

public record CreateCartDto
{
	public Guid UserId { get; init; }
	public decimal TaxRate { get; init; }
	public Coupon? Coupon { get; init; }

}
