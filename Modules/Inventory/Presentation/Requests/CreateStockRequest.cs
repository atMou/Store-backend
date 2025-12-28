using Inventory.Application.Features.CreateStock;

namespace Inventory.Presentation.Requests;

public record CreateStockRequest
{
	public Guid ProductId { get; set; }
	public int Quantity { get; init; }
	public int StockLow { get; init; }
	public int StockMid { get; init; }
	public int StockHigh { get; init; }

	public CreateStockCommand ToCommand()
	{

		return new CreateStockCommand
		{
			ProductId = Shared.Domain.ValueObjects.ProductId.From(ProductId),
			Stock = Quantity,
			Low = StockLow,
			Mid = StockMid,
			High = StockHigh
		};
	}
}