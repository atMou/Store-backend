namespace Shared.Domain.Errors;

public record BadRequestError : Error
{

	private BadRequestError(string message)
	{
		Message = message;
	}

	public new static BadRequestError New(string message) => new(message);
	public override string Message { get; }

	public override int Code => (int)ErrorCode.BadRequest;

	public override bool IsExceptional => false;
	public override bool IsExpected => true;


	public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

	public override bool IsEmpty => false;



}
