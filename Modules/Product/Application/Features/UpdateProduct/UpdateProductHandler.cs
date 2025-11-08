using Product.Domain.Contracts;
using Product.Persistence.Data;

using Shared.Application.Abstractions;
using Shared.Domain.Errors;

namespace Product.Application.Features.UpdateProduct;

internal abstract record UpdateProductCommand(
    Guid Id,
    string Slug,
    bool IsFeatured,
    bool IsNew,
    bool IsBestSeller,
    bool IsTrending,
    string[] ImageUrls,
    int Stock,
    int LowStockThreshold,
    decimal Price,
    string Category,
    string Description
) : ICommand<Fin<bool>>;

internal class UpdateProductCommandHandler(ProductDBContext dbContext)
    : ICommandHandler<UpdateProductCommand, Fin<bool>>
{
    public async Task<Fin<bool>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var db = from p in Db<ProductDBContext>.liftIO(async (context, env) =>
                await context.Products.FindAsync([command.Id], env.Token))
                 from _ in when(p is null,
                     Db<ProductDBContext>.fail<Unit>(NotFoundError.New($"Product with id '{command.Id}' was not found.")))
                 from up in p.Update(
                     new UpdateProductDto
                     {
                         Id = command.Id,
                         Slug = command.Slug,
                         IsFeatured = command.IsFeatured,
                         IsNew = command.IsNew,
                         IsBestSeller = command.IsBestSeller,
                         IsTrending = command.IsTrending,
                         ImageUrls = command.ImageUrls,
                         Stock = command.Stock,
                         LowStockThreshold = command.LowStockThreshold,
                         Price = command.Price,
                         Category = command.Category,
                         Description = command.Description
                     }
                 )
                 select true;
        return await db.RunSave(dbContext, EnvIO.New(null, cancellationToken));
    }
}