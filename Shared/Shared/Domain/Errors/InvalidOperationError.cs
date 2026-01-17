namespace Shared.Domain.Errors;

public record InvalidOperationError : Error
{

    private InvalidOperationError(string message)
    {
        Message = message;
    }

    public new static InvalidOperationError New(string message) => new(message);
    public override string Message { get; }

    public override int Code => (int)ErrorCode.InvalidOperation;

    public override bool IsExceptional => false;
    public override bool IsExpected => true;


    public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

    public override bool IsEmpty => false;



}
