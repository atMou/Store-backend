namespace Shared.Domain.Errors;

public record NotFoundError : Error
{

    private NotFoundError(string message)
    {
        Message = message;
    }

    public new static NotFoundError New(string message) => new(message);
    public override string Message { get; }


    public override int Code => (int)ErrorCode.NotFoundError;

    public override bool IsExceptional => false;
    public override bool IsExpected => true;


    public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

    public override bool IsEmpty => false;

}
