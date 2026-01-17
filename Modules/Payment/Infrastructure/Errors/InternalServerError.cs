using LanguageExt.Common;

namespace Payment.Infrastructure.Errors;

public record InternalServerError : Error
{
    private InternalServerError(string message)
    {
        Message = message;
    }

    public override ErrorException ToErrorException()
    {
        return ErrorException.New(Code, Message);
    }

    public new static InternalServerError New(string message) => new(message);
    public override string Message { get; }
    public override int Code => 500;
    public override bool IsExceptional => true;
    public override bool IsExpected => false;

    public override bool IsEmpty => false;
}
