using Microsoft.AspNetCore.Http;

using Shared.Application.Contracts.Product.Results;

namespace Shared.Infrastructure.Images;
public interface IImageService
{
    IO<IEnumerable<ImageResult>> UploadProductImages(IFormFile[] files, bool[] isMain, string slug, string category, string brand, string? color = null, string? size = null);
    IO<ImageUrl> UploadImage(IFormFile file, string name);
    IO<Unit> DeleteImagesAsync(IEnumerable<string> publicIds);
}
