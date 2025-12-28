namespace Db.ObjectResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class BadGatewayObjectResult : ObjectResult
{
	public BadGatewayObjectResult(object value)
		: base(value)
	{
		StatusCode = StatusCodes.Status502BadGateway;
	}
}
