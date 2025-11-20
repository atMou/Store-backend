using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

using Shared.Domain.Contracts.Product;
using Shared.Infrastructure.Images.Options;

namespace Shared.Infrastructure.Images;

public class ImageService(IOptions<CloudinarySettings> options) : IImageService
{
    private readonly CloudinarySettings _options = options.Value;

    public IO<IEnumerable<ImageDto>> UploadProductImages(IFormFile[] files, bool[] isMain, string slug, string category,
        string brand, string color)
    {
        var index = 0;
        return files.AsIterable().Traverse(file =>
        {
            var alt = $" {slug} {category} {brand} {color}";
            var _isMain = isMain.ElementAtOrDefault(index);

            index++;

            return UploadAndResizeAsync(file, slug)
                .Map(url => new ImageDto
                {
                    Url = url,
                    AltText = alt,
                    IsMain = _isMain
                });
        }).Map(it => it.AsEnumerable()).As();
    }


    public IO<ImageUrl> UploadImage(IFormFile file, string userName)
    {
        return UploadAndResizeAsync(file, userName, "users", 200, 200).Map(ImageUrl.FromUnsafe);
    }
    private IO<string> UploadAndResizeAsync(
       IFormFile file,
       string slug,
       string folderName = "products",
       int maxWidth = 1200,
       int maxHeight = 1200)
    {
        return from _cloudinary in GetCredentials()
               from _ in when(!file.ContentType.StartsWith("image/"),
                   IO.fail<Unit>(InvalidOperationError.New("File is not a valid image.")))
               from url in liftIO(async () =>
               {
                   await using var inputStream = file.OpenReadStream();

                   var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                   var publicId = $"{folderName}/{slug}_{fileName}_{Guid.NewGuid():N}";

                   // Upload original, untouched file to Cloudinary
                   var uploadParams = new ImageUploadParams
                   {
                       File = new FileDescription(file.FileName, inputStream),
                       Folder = folderName,
                       PublicId = publicId,
                       Overwrite = true,
                       UseFilename = false,
                       UniqueFilename = false,

                       // Cloudinary handles all optimization perfectly
                       Transformation = new Transformation()
                           .Width(maxWidth)
                           .Height(maxHeight)
                           .Crop("limit")             // never upscale
                           .Quality("auto:best")      // Cloudinary chooses perfect quality
                           .FetchFormat("auto")       // f_auto
                           .Flags("progressive")      // best for JPEG images
                   };

                   var result = await _cloudinary.UploadAsync(uploadParams);

                   if (result.Error != null)
                       return IO.fail<string>(InvalidOperationError.New(
                           $"Cloudinary upload failed: {result.Error.Message}"));

                   return IO.pure(result.SecureUrl?.ToString()
                       ?? throw new InvalidOperationException("Failed to generate URL."));
               }).Bind(x => x)
               select url;
    }

    //private IO<string> UploadAndResizeAsync(IFormFile file, string slug, string folderName = "products", int width = 600,
    //    int height = 600)
    //{
    //    return from _cloudinary in GetCredentials()
    //           from _ in when(!file.ContentType.StartsWith("image/"),
    //               IO.fail<Unit>(InvalidOperationError.New("File is not a valid image.")))
    //           from s in liftIO(async () =>
    //           {
    //               await using var inputStream = file.OpenReadStream();
    //               using var image = await Image.LoadAsync(inputStream);
    //               var fileName = Path.GetFileNameWithoutExtension(file.FileName);

    //               // Resize only if larger than target
    //               if (image.Width > width || image.Height > height)
    //               {
    //                   image.Mutate(x => x.Resize(new ResizeOptions
    //                   {
    //                       Size = new Size(width, height),
    //                       Mode = ResizeMode.Max
    //                   }));
    //               }

    //               //await using var outputStream = new MemoryStream();
    //               //await image.SaveAsJpegAsync(outputStream);
    //               //outputStream.Position = 0;

    //               var publicId = $"{slug}_{fileName}";
    //               var uploadParams = new ImageUploadParams
    //               {
    //                   File = new FileDescription(publicId, inputStream),
    //                   Folder = folderName,
    //                   Overwrite = true,
    //                   PublicId = publicId,
    //                   UseFilename = true,
    //                   UniqueFilename = false,

    //               };
    //               var result = await _cloudinary.UploadAsync(uploadParams);
    //               if (result.Error != null)
    //               {
    //                   return IO.fail<string>(
    //                       InvalidOperationError.New($"Cloudinary upload failed: {result.Error.Message}"));
    //               }

    //               if (result.SecureUrl is null)
    //               {
    //                   return IO.fail<string>(InvalidOperationError.New("Failed to generate the Url"));
    //               }


    //               return IO.pure(result.SecureUrl.ToString());
    //           }).Bind(x => x)
    //           select s;
    //}

    private IO<Cloudinary> GetCredentials()
    {
        return from _cloudinary in
                (Optional(_options.ApiKey),
                    Optional(_options.ApiSecret),
                    Optional(_options.CloudName))
                .Apply((key, secret, cname) =>
                    new Cloudinary(new Account(cname, key, secret))).As()
                .Match(IO.pure,
                    IO.fail<Cloudinary>(InvalidOperationError.New("Cloudinary settings are missing or invalid.")))
               select _cloudinary;
    }
}