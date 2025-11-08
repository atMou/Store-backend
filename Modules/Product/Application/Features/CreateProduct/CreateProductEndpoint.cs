namespace Catalog.Features.CreateProduct;
internal abstract record CreateProductRequest(
    string Slug,
    string SkuUniqId,
    bool IsFeatured,
    string ImageUrl,
    int Stock,
    int LowStockThreshold,
    decimal Price,
    string Currency,
    string Brand,
    string Size,
    string Color,
    string Category,
    string Description,
    bool IsNew);

internal record CreatedProductResponse(Guid Id);



//internal class CreateProductEndpoint : ICarterModule
//{
//    public void AddRoutes(IEndpointRouteBuilder app)
//    {
//        app.MapPost("/products",
//                async (CreateProductRequest request, ISender sender, HttpContext context,
//                    CancellationToken cancellationToken) =>
//                {
//                    var command = new CreateProductCommand(request.Slug, request.SkuUniqId, request.IsFeatured,
//                        request.ImageUrl, request.Stock, request.LowStockThreshold, request.Price, request.Currency,
//                        request.Brand, request.Size, request.Color, request.Category, request.Description,
//                        request.IsNew);
//                    var result = await sender.Send(command, cancellationToken);

//                    return result.Map(productResult => new CreatedProductResponse(productResult.Id))
//                        .ToActionResult(
//                            res => CreatedAtRoute(string.Empty, new { id = res.Id },
//                                new CreatedProductResponse(res.Id)), context.Request.Path);


//                }).WithName("Create Product")
//            .Produces<CreatedProductResponse>(StatusCodes.Status201Created)
//            .Produces(StatusCodes.Status400BadRequest)
//            .Produces(StatusCodes.Status500InternalServerError)
//            .WithTags("Products")
//            .WithMetadata(new HasPermissionAttribute(Errors.Domain.Bookings.Permission.CreateProduct));
//    }
//}
