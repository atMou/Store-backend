using Product.Persistence;

using Shared.Infrastructure.Images;

namespace Product.Application.Features.DeleteImages;

public record DeleteImagesCommand(IEnumerable<ProductImageId> ProductImageIds)
    : ICommand<Fin<Unit>>
{

}

internal class DeleteImagesCommandHandler(
    ProductDBContext dbContext,
    IImageService imageService)
    : ICommandHandler<DeleteImagesCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteImagesCommand command,
        CancellationToken cancellationToken)
    {

        var db =
          from ps in GetEntities<ProductDBContext, Domain.Models.Product>(
                product => product.Images.Any(pi => command.ProductImageIds.Contains(pi.Id)),
                (opt) =>
                {
                    opt.AsSplitQuery = true;
                    opt.AddInclude(p => p.Images);
                    return opt;
                })

          from _1 in when(ps.Count == 0,
              IO.fail<Unit>(NotFoundError.New($"The Specified images ids are not assigned to products")))

          let urls = ps.SelectMany(p => p.Images
              .Where(pi => command.ProductImageIds.Contains(pi.Id)).Select(pi => pi.ImageUrl.Value))


          from _2 in imageService.DeleteImagesAsync(urls)

          let x = ps.SelectMany(p => p.Images
              .Where(pi => command.ProductImageIds.Contains(pi.Id))).Select(pi => pi.Id)

          let s = ps.Select(p => p.DeleteImages(x))

          select unit;



        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}