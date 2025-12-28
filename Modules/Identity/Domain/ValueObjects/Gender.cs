
using Shared.Domain.Errors;

namespace Identity.Domain.ValueObjects;

public record Gender : DomainType<Gender, string>
{
	private Gender(byte code, string value)
	{
		Code = code;
		Value = value;
		_all.Add(this);
	}

	public byte Code { get; }
	public string Value { get; }

	private static List<Gender> _all = new();
	public static Gender None => new(0, "None");
	public static Gender Male => new(1, "Male");
	public static Gender Female => new(2, "Female");
	public static Gender Others => new(3, "Others");
	public static Fin<Gender> From(string repr)
	{
		return repr switch
		{
			"Male" => Fin<Gender>.Succ(Male),
			"Female" => Fin<Gender>.Succ(Female),
			"Other" => Fin<Gender>.Succ(Others),
			_ => Fin<Gender>.Succ(None)
		};
	}

	public string To()
	{
		return Value;
	}

	public static Fin<Gender> FromCode(byte code)
	{
		return _all.FirstOrDefault(g => g.Code == code)
			is { } gender
			? Fin<Gender>.Succ(gender)
			: Fin<Gender>.Fail(InvalidOperationError.New($"Invalid gender code: {code}"));
	}

	public static Gender FromUnsafe(string repr)
	{
		return Optional(_all.FirstOrDefault(g => g.Value == repr)).IfNone(() => None);

	}
	public static Gender FromNullable(string? repr)
	{
		return repr is null ? None : Optional(_all.FirstOrDefault(g => g.Value == repr)).IfNone(() => None);
	}

	public static Gender FromUnsafe(byte code)
	{
		return Optional(_all.FirstOrDefault(g => g.Code == code)).IfNone(() => None);

	}

}
