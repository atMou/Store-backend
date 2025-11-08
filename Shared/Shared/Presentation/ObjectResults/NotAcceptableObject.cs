using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Presentation.ObjectResults;
internal class NotAcceptableObjectResult : ObjectResult
{
    public NotAcceptableObjectResult(ProblemDetails details) : base(details)
    {
        StatusCode = StatusCodes.Status406NotAcceptable;
    }
}
