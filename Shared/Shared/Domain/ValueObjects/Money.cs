namespace Shared.Domain.ValueObjects;

public record Money
{
    public readonly decimal Value;

    private Money()
    {
    }

    public static Money Zero => new Money(0m);

    private Money(decimal amount)
    {
        Value = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public int Dollars => (int)Math.Truncate(Value);
    public byte Cents => (byte)((Value - Math.Truncate(Value)) * 100);

    public static Money FromDecimal(decimal amount)
    {
        return new Money(amount);
    }

    public static Money FromParts(int dollars, byte cents)
    {
        return new Money(dollars + cents / 100m);
    }

    public decimal ToDecimal()
    {
        return Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static implicit operator Money(decimal amount)
    {
        return FromDecimal(amount);
    }
    public static implicit operator decimal(Money money)
    {
        return money.ToDecimal();
    }

    public static implicit operator Money(int amount)
    {
        return FromDecimal(amount);
    }


    public static implicit operator double(Money money)
    {
        return (double)money.ToDecimal();
    }

    public static implicit operator Money(double amount)
    {
        return FromDecimal((decimal)amount);
    }


    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Value + right.Value);
    }

    public static Money operator -(Money left, Money right)
    {
        return new Money(left.Value - right.Value);
    }

    public static Money operator -(Money left, decimal right)
    {
        return new Money(left.Value - right);
    }

    public static Money operator *(Money left, int multiplier)
    {
        return new Money(left.Value * multiplier);
    }

    public static Money operator *(Money left, decimal multiplier)
    {
        return new Money(left.Value * multiplier);
    }

    public static Money operator /(Money left, int divisor)
    {
        return new Money(left.Value / divisor);
    }


    public static bool operator >(Money left, Money right)
    {
        return left.Value > right.Value;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator <(Money left, Money right)
    {
        return left.Value < right.Value;
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.Value <= right.Value;
    }

    public override string ToString()
    {
        return $"{Value:C}";
    }
}