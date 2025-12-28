using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Shipment.Presentation.Controllers;


[ApiController]
[Route("[controller]")]
public class ShipmentController(ISender sender) : ControllerBase
{
	//[HttpGet]
	//public async Task<ActionResult<GetProductsQueryResult>> Get([FromQuery] GetProductsRequest request)
	//{
	//    var result = await sender.Send(request.ToQuery());

	//    return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
	//}

	//[HttpPost]
	//[Consumes("multipart/form-data")]
	//public async Task<ActionResult<CreateProductResult>> Create([FromForm] CreateProductRequest request)
	//{
	//    var result = await sender.Send(request.ToCommand());
	//    return result.ToActionResult(res => Ok(res), "");
	//}
}
