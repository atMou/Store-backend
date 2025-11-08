
namespace Shared.Application.Abstractions;

using Unit = LanguageExt.Unit;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
