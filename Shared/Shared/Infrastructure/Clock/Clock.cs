namespace Shared.Infrastructure.Clock;

public class Clock : IClock
{
    public DateTime UtcNow { get; } = DateTime.UtcNow;
}
