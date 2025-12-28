using Shared.Domain.Errors;

namespace Db.Errors;

using LanguageExt.Common;

public record ConflictError : Error
{

	private ConflictError(string message)
	{
		Message = message;
	}

	public new static ConflictError New(string message) => new(message);
	public override string Message { get; }


	public override int Code => (int)ErrorCode.ConflictError;

	public override bool IsExceptional => false;
	public override bool IsExpected => true;


	public override ErrorException ToErrorException() => ErrorException.New(Code, Message);

	public override bool IsEmpty => false;


}
