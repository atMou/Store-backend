using LanguageExt.Common;

namespace Payment.Infrastructure.Errors;

public record BadGatewayError : Error
{
    private BadGatewayError(string message)
    {
        Message = message;
    }

    public new static BadGatewayError New(string message) => new(message);
    public override string Message { get; }
    public override int Code => 502;
    public override bool IsExceptional => false;
    public override bool IsExpected => true;

    public override ErrorException ToErrorException()
    {
        return ErrorException.New(Code, Message);
    }
    public override bool IsEmpty => false;
}
