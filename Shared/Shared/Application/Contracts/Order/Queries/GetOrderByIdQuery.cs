
namespace Shared.Application.Contracts.Order.Queries;

public class GetOrderByIdQuery : ICommand<Fin<OrderResult>>
{
    public OrderId OrderId { get; init; }
}