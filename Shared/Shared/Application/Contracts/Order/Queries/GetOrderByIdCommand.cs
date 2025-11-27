
namespace Shared.Application.Contracts.Order.Queries;

public class GetOrderByIdCommand : ICommand<Fin<OrderResult>>
{
    public OrderId OrderId { get; init; }
}