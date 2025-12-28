namespace Shared.Presentation.Extensions;

public sealed class HttpStatusCodeInfo
{
	private static readonly Dictionary<int, HttpStatusCodeInfo> _byCode = new();


	public static readonly HttpStatusCodeInfo None = new(
		0, "None",
		"None Status Code",
		"");

	public static readonly HttpStatusCodeInfo BadRequest = new(
		400, "BadRequest",
		"The request could not be understood or was missing required parameters.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1");

	public static readonly HttpStatusCodeInfo Unauthorized = new(
		401, "Unauthorized",
		"Authentication failed or user does not have permissions.",
		"https://datatracker.ietf.org/doc/html/rfc7235#section-3.1");

	public static readonly HttpStatusCodeInfo Forbidden = new(
		403, "Forbidden",
		"Access is forbidden to the requested resource.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3");

	public static readonly HttpStatusCodeInfo NotFound = new(
		404, "NotFound",
		"The requested resource could not be found.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4");
	public static readonly HttpStatusCodeInfo InValidOperation = new(
		406, "InValidOperation",
		"The requested operation is invalid.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4");

	public static readonly HttpStatusCodeInfo Conflict = new(
		409, "Conflict",
		"Request could not be completed due to a conflict.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8");

	public static readonly HttpStatusCodeInfo ConflictDb = new(
		-2146232060, "Conflict",
		"Request could not be completed due to a conflict.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8");

	public static readonly HttpStatusCodeInfo UnprocessableEntity = new(
		422, "UnprocessableEntity",
		"The request was well-formed but was unable to be followed due to semantic errors.",
		"https://datatracker.ietf.org/doc/html/rfc4918#section-11.2");

	public static readonly HttpStatusCodeInfo TooManyRequests = new(
		429, "TooManyRequests",
		"The user has sent too many requests in a given amount of time.",
		"https://datatracker.ietf.org/doc/html/rfc6585#section-4");

	public static readonly HttpStatusCodeInfo InternalServerError = new(
		500, "InternalServerError",
		"A generic error message returned when no more specific message is suitable.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");

	public static readonly HttpStatusCodeInfo NotImplemented = new(
		501, "NotImplemented",
		"The server does not support the functionality required to fulfill the request.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2");

	public static readonly HttpStatusCodeInfo BadGateway = new(
		502, "BadGateway",
		"The server received an invalid response from the upstream server.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3");

	public static readonly HttpStatusCodeInfo ServiceUnavailable = new(
		503, "ServiceUnavailable",
		"The server is currently unavailable (because it is overloaded or down for maintenance).",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4");

	public static readonly HttpStatusCodeInfo GatewayTimeout = new(
		504, "GatewayTimeout",
		"The server did not receive a timely response from the upstream server.",
		"https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.5");


	private HttpStatusCodeInfo(int code, string name, string description, string typeUri)
	{
		Code = code;
		Name = name;
		Description = description;
		TypeUri = typeUri;
		_byCode[code] = this;
	}

	public int Code { get; }
	public string Name { get; }
	public string Description { get; }
	public string TypeUri { get; }

	public static HttpStatusCodeInfo FromCode(int code)
	{
		return _byCode.GetValueOrDefault(code, InternalServerError);
	}

	public override string ToString()
	{
		return $"{Code} {Name} ::::: {Description}";
	}
}