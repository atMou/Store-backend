using Shared.Domain.Errors;

namespace Shared.Errors;

using LanguageExt.Common;

public record UnprocessableEntityError : Error
{

	private UnprocessableEntityError(string message)
	{
		Message = message;
	}

	public new static UnprocessableEntityError New(string message) => new(message);
	public override string Message { get; }


	public override int Code => (int)ErrorCode.UnprocessableEntity;

	public override bool IsExceptional => false;
	public override bool IsExpected => true;


	public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

	public override bool IsEmpty => false;


}
