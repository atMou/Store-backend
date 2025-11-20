using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Product.Application.Features.CreateProduct;
using Product.Application.Features.GetProducts;
using Product.Presentation.Requests;

using Shared.Presentation.Extensions;

namespace Product.Presentation.Controllers;


[ApiController]
[Route("[controller]")]
public class ProductsController(ISender sender, IPublishEndpoint endpoint) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetProductsQueryResult>> Get([FromQuery] GetProductsRequest request)
    {
        var result = await sender.Send(request.ToQuery());

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateProductResult>> Create([FromForm] CreateProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(res => Ok(res), "");
    }
}
