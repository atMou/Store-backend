namespace Db.ObjectResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class TooManyRequestsObjectResult : ObjectResult
{
    public TooManyRequestsObjectResult(object value)
        : base(value)
    {
        StatusCode = StatusCodes.Status429TooManyRequests;
    }
}
