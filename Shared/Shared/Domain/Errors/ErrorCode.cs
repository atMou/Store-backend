namespace Shared.Domain.Errors;
public enum ErrorCode
{
    ValidationError = 400,
    BadRequest = 400,
    ConflictError = 409,
    NotFoundError = 404,
    InvalidOperation = 406,
    UnauthorizedError = 401,
    ForbiddenError = 403,
    InternalServerError = 500,
    UnprocessableEntity = 422,
    TooManyRequests = 429,

}
