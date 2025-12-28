using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

using Shared.Infrastructure.Images.Options;

using Image = Shared.Domain.ValueObjects.Image;

namespace Shared.Infrastructure.Images;

public class ImageService(IOptions<CloudinarySettings> options) : IImageService
{
    private readonly CloudinarySettings _options = options.Value;

    public IO<IEnumerable<Image>> UploadProductImages(IFormFile[] files, bool[] isMain, string slug, string category, string brand, string? color = null)
    {
        var index = 0;
        if (!files.Any())
        {
            return IO<IEnumerable<Image>>.Pure([]);
        }
        return files.AsIterable().Traverse(file =>
        {
            var alt = $"{slug} {category} {brand} {color}".Trim();
            var _isMain = isMain.ElementAtOrDefault(index);

            index++;

            return UploadAndResizeAsync(file, "products", slug, category, brand, color)
                .Map(t => Image.FromUnsafe(t.Url, t.PublicId, alt, _isMain));
        }).Map(it => it.AsEnumerable()).As();
    }

    public IO<ImageUrl> UploadImage(IFormFile file, string userName)
    {
        return UploadAndResizeAsync(file, "users", userName, null, null, null, 200, 200).Map(tuple => ImageUrl.FromUnsafe(tuple.Url, tuple.PublicId));
    }



    private IO<(string Url, string PublicId)> UploadAndResizeAsync(
       IFormFile file,
       string folderName,
       string slug,
       string? category = null, string? brand = null, string? color = null,
       int maxWidth = 1200,
       int maxHeight = 1200)
    {

        return from _cloudinary in GetCredentials()
               from _ in when(!file.ContentType.StartsWith("image/"),
                   IO.fail<Unit>(InvalidOperationError.New("File is not a valid image.")))
               from tuple in liftIO(async () =>
               {
                   await using var inputStream = file.OpenReadStream();

                   var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                   var publicId = category is not null && brand is not null && color is not null
                       ? $"{slug}-{brand}-{category}-{color}-{fileName}-{Helpers.GenerateCode(8)}"
                       : $"{slug}-{fileName}-{Helpers.GenerateCode(8)}";

                   var uploadParams = new ImageUploadParams
                   {
                       File = new FileDescription(file.FileName, inputStream),
                       Folder = folderName,
                       PublicId = publicId,
                       Overwrite = true,
                       UseFilename = false,
                       UniqueFilename = false,

                       Transformation = new Transformation()
                           .Width(maxWidth)
                           .Height(maxHeight)
                           .Crop("limit")
                           .Quality("auto:best")
                           .FetchFormat("auto")
                           .Flags("progressive")
                   };

                   var result = await _cloudinary.UploadAsync(uploadParams);

                   if (result.Error != null)
                       return IO.fail<(string, string)>(InvalidOperationError.New(
                           $"Cloudinary upload failed: {result.Error.Message}"));


                   var secureUrl = result.SecureUrl?.ToString();
                   return secureUrl.IsNotNull() ? IO.pure((secureUrl!, result.PublicId)) :
                     IO.fail<(string Url, string PublicId)>(InvalidOperationError.New("Failed to generate URL."));

               }).Bind(x => x)
               select tuple;
    }
    public IO<Unit> DeleteImagesAsync(IEnumerable<string> publicIds)
    {
        return from _cloudinary in GetCredentials()

               from results in publicIds.AsIterable().Traverse(id =>
                   IO.liftAsync(async e => await _cloudinary.DestroyAsync(new DeletionParams(id))))
               let res = results.Partition(result => result.Error.IsNotNull())


               let errs = res.First.Fold("", (s, result) => $"{s} :: {result.Error} ")
               from x in when(res.First.Any(),
                   IO.fail<Unit>(ConflictError.New($"Failed to delete some or all images: {errs}")))
               select unit;

    }
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