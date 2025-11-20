using Microsoft.AspNetCore.Http;

using Shared.Domain.Contracts.Product;

namespace Shared.Infrastructure.Images;
public interface IImageService
{
    IO<IEnumerable<ImageDto>> UploadProductImages(IFormFile[] files, bool[] isMain, string slug, string category, string brand, string color);
    IO<ImageUrl> UploadImage(IFormFile file, string name);
}
