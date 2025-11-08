using Microsoft.AspNetCore.Mvc;

namespace Shared.Presentation.Extensions;

public static class ActionResultsExtensions
{
    public static ActionResult<T> Ok<T>(T t) => new OkObjectResult(t);
    public static ActionResult<T> CreatedAtRoute<T>(string routeName, object routeValues, T t) => new CreatedAtRouteResult(routeName, routeValues, t);

}
