using Microsoft.AspNetCore.Mvc;

namespace Order.Presentation.Controllers;


[ApiController]
[Route("[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetOrderById(Guid id)
    //{

    //}
}