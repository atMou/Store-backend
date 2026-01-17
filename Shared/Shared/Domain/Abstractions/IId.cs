namespace Shared.Domain.Abstractions;

//public abstract record Id<T>(Guid Value) : IId, IEquatable<T>
//    where T : Id<T>
//{
//    public Guid Value { get; } = Value;


//    public bool Equals(T? other) =>
//        other is not null && Value.Equals(other.Value);

//    public override int GetHashCode() => Value.GetHashCode();


//    public override string ToString() => Value.ToString();
//}

public interface IId
{
    Guid Value { get; }


}