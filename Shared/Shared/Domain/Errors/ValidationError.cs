namespace Shared.Domain.Errors;

public record ValidationError : Error
{
    private ValidationError(string message)
    {
        Message = message;
    }
    public new static ValidationError New(string message) => new(message);
    public override string Message { get; }


    public override int Code => (int)ErrorCode.ValidationError;

    public override bool IsExceptional => false;
    public override bool IsExpected => true;

    public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

    public override bool IsEmpty => false;

}
