namespace Shared.Infrastructure.Clock;
public interface IClock
{
    DateTime UtcNow { get; }
}
