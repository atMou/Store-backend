namespace Product.Domain.ValueObjects;

public record Discount
{
    private Discount() { }
    public decimal Value { get; }
    private Discount(decimal value)
    {
        Value = value;
    }

    public static Discount FromUnsafe(decimal repr)
    {
        return new Discount(repr);
    }

    public static Fin<Discount> From(decimal OldPrice, decimal? NewPrice)
    {

        return Optional(NewPrice).Match(
            np => np >= OldPrice
            ? Fin<Discount>.Fail("New price must be less than old price for a discount.")
            : new Discount(OldPrice - np),
                        () => new Discount(0m)
        );
    }


}
