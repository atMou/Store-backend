namespace Basket.Presentation.Requests;

public record CreateCartRequest
{
    public decimal Tax { get; init; }
    public decimal ShipmentCost { get; init; }

    public Guid? CouponId { get; init; }

    public Guid UserId { get; init; }

    public CreateCartCommand ToCommand()
    {
        return new CreateCartCommand()
        {
            CouponId = Optional(CouponId).Match<CouponId?>(Shared.Domain.ValueObjects.CouponId.From, () => null),
            Tax = Tax,
            UserId = Shared.Domain.ValueObjects.UserId.From(UserId),
            ShipmentCost = ShipmentCost
        };
    }

}


