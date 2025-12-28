namespace Basket.Domain.ValueObjects;

public record Tax
{
	public decimal Rate { get; }
	private Tax(decimal rate)
	{
		Rate = rate;
	}

	public static Fin<Tax> From(decimal repr)
	{
		var rate = decimal.Round(repr, 2, MidpointRounding.AwayFromZero);
		return repr is > 0m and <= 100m
			? FinSucc(new Tax(rate / 100m))
			: FinFail<Tax>(BadRequestError.New("TaxValue rate must be between 0 and 100."));
	}


	public static Tax FromUnsafe(decimal repr)
	{
		return new Tax(repr);
	}

	public static Money operator *(Tax left, Money right)
	{
		return Money.FromDecimal(right.Value * left.Rate);
	}

	public static decimal AdditiveIdentity => 0m;
}