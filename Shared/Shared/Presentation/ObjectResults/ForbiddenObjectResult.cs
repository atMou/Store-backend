namespace Db.ObjectResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class ForbiddenObjectResult : ObjectResult
{
	public ForbiddenObjectResult(object value)
		: base(value)
	{
		StatusCode = StatusCodes.Status403Forbidden;
	}
}
