
namespace Product.Presentation.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductResult>>> Get([FromQuery] GetProductsRequest request)
    {
        var result = await sender.Send(request.ToQuery());

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResult>> Get([FromRoute] Guid id, [FromQuery] string[] include)
    {
        var result = await sender.Send(new GetProductByIdQuery(ProductId.From(id), include));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }


    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateProductResult>> Create([FromForm] CreateProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(res => Ok(res), "");
    }
    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Unit>> Update([FromForm] UpdateProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), "");
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Delete([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteProductCommand(ProductId.From(id)));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }
    [HttpDelete]
    public async Task<ActionResult<Unit>> DeleteImages([FromBody] IEnumerable<Guid> ids)
    {
        var result = await sender.Send(new DeleteImagesCommand(ids.Select(guid => ProductImageId.From(guid))));

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var result = await sender.Send(new GetAllCategoriesCommand());

        return Ok(result);
    }


    [HttpGet("colors")]
    public async Task<ActionResult<IEnumerable<string>>> GetColors()
    {
        var result = await sender.Send(new GetAllColorsCommand());

        return Ok(result);
    }

    [HttpGet("sizes")]
    public async Task<ActionResult<IEnumerable<string>>> GetSizes()
    {
        var result = await sender.Send(new GetAllSizesCommand());

        return Ok(result);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<string>>> GetBrands()
    {
        var result = await sender.Send(new GetAllBrandsCommand());

        return Ok(result);
    }


}