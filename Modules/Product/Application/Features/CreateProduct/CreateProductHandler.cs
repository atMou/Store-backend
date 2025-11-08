using MassTransit;

using Product.Application.Events;
using Product.Domain.Contracts;
using Product.Persistence.Data;

using Shared.Application.Abstractions;

namespace Product.Application.Features.CreateProduct;
public abstract record CreateProductCommand(
    string Slug,
    string[] ImageUrls,
    int Stock,
    int LowStockThreshold,
    decimal Price,
    string Currency,
    string Brand,
    string Size,
    string Color,
    string Category,
    string Description,
    decimal? NewPrice
) : ICommand<Fin<CreateProductResult>>;

public record CreateProductResult(Guid Id);

internal class CreateProductHandlerCommandHandler(ProductDBContext dbContext, IBus bus)
    : ICommandHandler<CreateProductCommand, Fin<CreateProductResult>>
{
    public async Task<Fin<CreateProductResult>> Handle(CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var db = from p in CreateProduct(command)
                 from _ in Db<ProductDBContext>.lift((ctx) => ctx.Products.Add(p))
                 select new CreateProductResult(p.Id.Value);
        return await db.RunSave(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseBiEvent(
                (result) => bus.Publish(new ProductCreatedEvent(result.Id), cancellationToken),
                (error) => bus.Publish(new ProductCreatingFailEvent(error), cancellationToken)
            );
    }

    private Fin<Product.Domain.Models.Product> CreateProduct(CreateProductCommand command) =>
        Domain.Models.Product.Create(new CreateProductDto
        {
            Slug = command.Slug,
            ImageUrls = command.ImageUrls,
            Stock = command.Stock,
            LowStockThreshold = command.LowStockThreshold,
            Price = command.Price,
            Currency = command.Currency,
            Brand = command.Brand,
            Size = command.Size,
            Color = command.Color,
            Category = command.Category,
            Description = command.Description,
            NewPrice = command.NewPrice
        });

}



