using Db.ObjectResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Shared.Presentation.ObjectResults;

namespace Shared.Presentation.Extensions;

public static class FinExtensions
{
    private static readonly Regex _duplicateIndexAndEmailRegex = new(
        @"'IX_[^_]+_(\w+)'.*? \(([^)]+)\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
    );

    private static string ExtractFields(string message)
    {
        var match = _duplicateIndexAndEmailRegex.Match(message);
        return match.Success
            ? $"{match.Groups[1].Value.ToUpperInvariant()} {match.Groups[2].Value} already exists"
            : message;
    }

    private static ActionResult<T> GetFailActionResult<T>(Error error, string route)
    {
        var list = new List<string>();
        HttpStatusCodeInfo codeInfo;

        switch (error)
        {
            case ManyErrors me:
                list.AddRange(me.Errors.Select(e => e.Message));
                codeInfo = HttpStatusCodeInfo.FromCode(StatusCodes.Status400BadRequest);
                break;

            case Expected ex:
                list.Add(ex.Message);
                codeInfo = HttpStatusCodeInfo.FromCode(ex.Code);
                break;

            case Exceptional { Inner.IsSome: true } ex when ex.Inner.ValueUnsafe()!.Code == -2146232060:
                list.Add(ex.Inner.Match(e => ExtractFields(e.Message), () => ex.Message));
                codeInfo = HttpStatusCodeInfo.FromCode(StatusCodes.Status409Conflict);
                break;

            case Exceptional ex:
                list.Add(ex.Message);
                codeInfo = HttpStatusCodeInfo.FromCode(ex.Code);
                break;

            default:
                list.Add("An internal server error happened, please try again later.");
                codeInfo = HttpStatusCodeInfo.FromCode(StatusCodes.Status500InternalServerError);
                break;
        }

        var details = list.ToProblemDetails(codeInfo, route);

        return codeInfo.Code switch
        {
            StatusCodes.Status401Unauthorized => new UnauthorizedObjectResult(details),
            StatusCodes.Status406NotAcceptable => new NotAcceptableObjectResult(details),

            StatusCodes.Status400BadRequest => new BadRequestObjectResult(details),
            StatusCodes.Status403Forbidden => new ForbiddenObjectResult(details),
            StatusCodes.Status404NotFound => new NotFoundObjectResult(details),
            StatusCodes.Status409Conflict => new ConflictObjectResult(details),
            StatusCodes.Status422UnprocessableEntity => new UnprocessableEntityObjectResult(details),
            StatusCodes.Status429TooManyRequests => new TooManyRequestsObjectResult(details),
            StatusCodes.Status502BadGateway => new BadGatewayObjectResult(details),
            _ => new InternalServerErrorObjectResult(details)
        };
    }

    private static ProblemDetails ToProblemDetails(this IEnumerable<string> errors, HttpStatusCodeInfo codeInfo, string route)
    {
        return new ProblemDetails
        {
            Title = codeInfo.Name,
            Status = codeInfo.Code,
            Type = codeInfo.TypeUri,
            Detail = codeInfo.Description,
            Instance = route,
            Extensions = new Dictionary<string, object?>
            {
                { "Errors", errors }
            },

        };
    }

    public static ActionResult<T> ToActionResult<T>(
        this Fin<T> ma,
        Func<T, ActionResult<T>> ok,
        string route
    ) =>
        ma.Match(ok, e => GetFailActionResult<T>(e, route));
}


