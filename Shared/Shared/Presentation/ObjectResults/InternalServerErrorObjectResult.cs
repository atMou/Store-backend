namespace Db.ObjectResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object value)
        : base(value)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}
