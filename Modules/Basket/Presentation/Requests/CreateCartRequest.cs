namespace Basket.Presentation.Requests;

public record CreateCartRequest
{
    public decimal taxRate { get; init; }

    public Guid? CouponId { get; init; }

    public Guid UserId { get; init; }

    public CreateCartCommand ToCommand()
    {
        return new CreateCartCommand()
        {
            CouponId = Optional(CouponId).Match<CouponId?>(Shared.Domain.ValueObjects.CouponId.From, () => null),
            TaxRate = taxRate,
            UserId = Shared.Domain.ValueObjects.UserId.From(UserId)
        };
    }

}


