using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Presentation.ObjectResults;

public class TooManyRequestsObjectResult : ObjectResult
{
	public TooManyRequestsObjectResult(object value)
		: base(value)
	{
		StatusCode = StatusCodes.Status429TooManyRequests;
	}
}
