namespace Identity.Domain.ValueObjects;

public record ProductSubscription
{
    private ProductSubscription() { }

    private ProductSubscription(string key)
    {
        Key = key;
    }

    public string Key { get; private init; }
    public static ProductSubscription Create(string productId, string colorCode, string sizeCode)
    {
        var key = $"{productId}_{colorCode}_{sizeCode}";
        return new ProductSubscription(key);
    }

    public bool Matches(string productId, string colorCode, string sizeCode)
    {
        var key = $"{productId}_{colorCode}_{sizeCode}";
        return Key == key;
    }


}
