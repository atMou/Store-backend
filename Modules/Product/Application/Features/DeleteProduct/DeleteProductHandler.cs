using Product.Persistence.Data;

using Shared.Application.Abstractions;
using Shared.Domain.Errors;

namespace Product.Application.Features.DeleteProduct;

public abstract record DeleteProductCommand(Guid Id) : ICommand<Fin<bool>>;

internal class DeleteProductHandlerCommandHandler(ProductDBContext dbContext)
    : ICommandHandler<DeleteProductCommand, Fin<bool>>
{
    public async Task<Fin<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var db =
            from p in Db<ProductDBContext>.liftIO(async (ctx, e) =>
                await ctx.Products.FindAsync([request.Id], e.Token))
            from _ in when(p is null,
                Db<ProductDBContext>.fail<Unit>(NotFoundError.New($"Product with id '{request.Id}' was not found.")))

                //from _1 in Db<ProductDBContext>.liftIO(async (_, env) =>
                //{
                //    await bus.Publish(new ProductDeletingEvent(p.Id), env.Token);
                //    return unit;
                //})
            from _2 in Db<ProductDBContext>.lift(ctx => ctx.Products.Remove(p))
            select true;

        return await db.RunSaveT(dbContext, EnvIO.New(null, cancellationToken));
    }
}