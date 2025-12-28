using Inventory.Presentation.Requests;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Presentation.Extensions;

namespace Inventory.Presentation.Controllers;
[ApiController]
[Route("[controller]")]
public class InventoriesController(ISender sender) : ControllerBase
{

	[HttpPost]
	public async Task<ActionResult<Unit>> Create([FromBody] CreateStockRequest request)
	{
		var result = await sender.Send(request.ToCommand());

		return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
	}

	[HttpGet]
	public ActionResult<string> Get()
	{
		return Ok("Got ir");
	}



}