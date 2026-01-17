namespace Shared.Application.Abstractions;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}
