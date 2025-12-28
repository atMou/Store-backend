using Microsoft.AspNetCore.Http;

using Image = Shared.Domain.ValueObjects.Image;

namespace Shared.Infrastructure.Images;
public interface IImageService
{
    IO<IEnumerable<Image>> UploadProductImages(IFormFile[] files, bool[] isMain, string slug, string category, string brand, string? color = null);
    IO<ImageUrl> UploadImage(IFormFile file, string name);
    IO<Unit> DeleteImagesAsync(IEnumerable<string> publicIds);
}
