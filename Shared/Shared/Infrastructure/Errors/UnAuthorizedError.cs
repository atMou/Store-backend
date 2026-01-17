using Shared.Domain.Errors;

namespace Shared.Infrastructure.Errors;

public record UnAuthorizedError : Error
{

    private UnAuthorizedError(string message)
    {
        IsEmpty = false;
        Message = message;
    }

    public new static UnAuthorizedError New(string message) => new(message);
    public override string Message { get; }


    public override int Code => (int)ErrorCode.UnauthorizedError;

    public override bool IsExceptional => false;
    public override bool IsExpected => true;


    public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

    public override bool IsEmpty { get; }

}
