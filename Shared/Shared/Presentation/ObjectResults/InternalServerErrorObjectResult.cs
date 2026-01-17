using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Presentation.ObjectResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object value)
        : base(value)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
