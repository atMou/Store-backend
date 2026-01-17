using Shared.Domain.Validations;

namespace Shared.Domain.ValueObjects;

public record TrackingCode
{
    public string Value { get; private set; }


    private TrackingCode()
    {
        Value = Helpers.GenerateCode(10);
    }

    public static TrackingCode Create()
    {
        return new TrackingCode();
    }

    public static TrackingCode FromUnsafe(string code)
    {
        return new TrackingCode { Value = code };
    }

    public Fin<Unit> Validate()
    {

        if (Value.Length != 10)
            return FinFail<Unit>(ValidationError.New("Tracking code must be of characters long."));
        return unit;
    }
}