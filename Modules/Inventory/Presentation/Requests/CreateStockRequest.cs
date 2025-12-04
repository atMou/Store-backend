using Inventory.Application.Features.AddStock;

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
            Quantity = Quantity,
            StockLow = StockLow,
            StockMid = StockMid,
            StockHigh = StockHigh
        };
    }
}