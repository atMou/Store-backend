namespace Identity.ValueObjects;

using System.Numerics;

using LanguageExt.Common;

public record Age :
    IComparisonOperators<Age, Age, bool>,
    IComparisonOperators<Age, int, bool>
{
    public readonly int Value;

    private Age(int repr)
    {
        Value = repr;

    }

    public static Fin<Age> From(string repr, DateTime dateTime)
    {
        return DateOnly.TryParse(repr, out var result)
            ? From(result, DateOnly.FromDateTime(dateTime))
            : Error.New($"Could not parse date of birth '{repr}'");

    }

    public static Fin<Age> From(byte repr)
    {
        return repr > 120 ? FinFail<Age>((Error)$"Please enter an proper age.") : new Age(repr);
    }


    public static Fin<Age> From(DateOnly dob, DateOnly now)
    {

        int age = now.Year - dob.Year;
        if (now < dob.AddYears(age))
        {
            age--;
        }

        return age < 18 ?
            FinFail<Age>(Error.New($"Age value '{dob.ToShortDateString()}' is invalid, please enter a proper age over 18 Yo."))
            : new Age(age);
    }


    private static Fin<Unit> IsLessThanZero(int repr)
    {
        return repr < 0
              ? FinFail<Unit>(Error.New($"Age value '{repr}' is invalid, please enter a proper age value."))
              : unit;
    }

    private static Fin<Unit> IsMoreThan100(int repr)
    {
        return repr > 100
            ? FinFail<Unit>(Error.New($"Age value '{repr}' is invalid, please enter a proper age value."))
            : unit;
    }

    private static Fin<Unit> IsLessThan18(int repr)
    {
        return repr < 0
            ? FinFail<Unit>(Error.New($"Age value '{repr}' is invalid, please enter a age over 18 YO value."))
            : unit;
    }
    public static Age FromUnsafe(int repr)
    {
        return new Age(repr);
    }



    public int To()
    {
        return Value;
    }


    public static bool operator >(Age left, Age right)
    {
        return left.To() > right.To();
    }

    public static bool operator >=(Age left, Age right)
    {
        return left.To() >= right.To();
    }

    public static bool operator <(Age left, Age right)
    {
        return left.To() < right.To();
    }

    public static bool operator <=(Age left, Age right)
    {
        return left.To() <= right.To();
    }

    public static bool operator ==(Age left, int right)
    {
        return left.To() == right;
    }

    public static bool operator !=(Age left, int right)
    {
        return !(left == right);
    }

    public static bool operator >(Age left, int right)
    {
        return left.To() > right;
    }

    public static bool operator >=(Age left, int right)
    {
        return left.To() >= right;
    }

    public static bool operator <(Age left, int right)
    {
        return left.To() < right;
    }

    public static bool operator <=(Age left, int right)
    {
        return left.To() <= right;
    }
}
