namespace Product.Domain.ValueObjects;

public record Stock : DomainType<Stock, (int Value, int Low, int Mid, int High)>
{
    public int Value { get; init; }
    public int Low { get; init; }
    public int High { get; init; }
    public int Mid { get; init; }

    private Stock(int value, int low, int mid, int high)
    {
        Value = value;
        Low = low;
        High = high;
        Mid = mid;
    }

    public static Fin<Stock> From((int Value, int Low, int Mid, int High) repr)
    {

        return repr switch
        {
            (var v and < 0, _, _, _) => FinFail<Stock>(ValidationError.New($"Invalid stock value, cannot be less than '0' : got '{v}'")),
            (var _, var l and < 0, _, _) => FinFail<Stock>(ValidationError.New($"Invalid low stock value, cannot be less than '0' : got '{l}'")),
            (var _, var _, var m and < 0, _) => FinFail<Stock>(ValidationError.New($"Invalid mid stock value, cannot be less than '0' : got '{m}'")),
            (var _, var _, var _, var h and < 0) => FinFail<Stock>(ValidationError.New($"Invalid high stock value, cannot be less than '0' : got '{h}'")),
            var (_, l, m, h) when l > m || l > h || m > h => FinFail<Stock>(ValidationError.New($"Invalid stock thresholds: Low={l}, Mid={m}, High={h}")),
            var (v, l, m, h) => FinSucc(new Stock(v, l, m, h)),

        };
    }

    public (int Value, int Low, int Mid, int High) To()
    {
        return (Value, Low, Mid, High);
    }

    public static Stock FromUnsafe(int value, int low, int mid, int high)
    {
        return new Stock(value, low, mid, high);
    }

}
