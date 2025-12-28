using System.Security.Cryptography;
using System.Text;

namespace Shared.Domain.Validations;
public static class Helpers
{
	public static Fin<Option<A>> ValidateNullable<A, B>(B? repr) where A : DomainType<A, B> where B : struct =>
		repr.HasValue
			? A.From(repr.Value).Map(Some)
			: FinSucc(Option<A>.None);


	public static Fin<A?> ValidateIfNotNull<A, B>(this A? repr, Func<A, Fin<A?>> fn) where B : class =>
		repr is not null
			? fn(repr)
			: FinSucc<A?>(default);

	public static Fin<Option<A>> ValidateNullable<A, B>(B? repr) where A : DomainType<A, B> where B : class =>
		repr is not null
			? A.From(repr).Map(Some)
			: FinSucc(Option<A>.None);
	public static Option<A> FromUnsafe<A, B>(B? repr) where A : DomainType<A, B> where B : struct =>
		Optional(repr)
			.Map(A.FromUnsafe);

	public static Option<A> FromUnsafe<A, B>(B? repr) where A : DomainType<A, B> where B : class =>
		Optional(repr)
			.Map(A.FromUnsafe);

	public static B? NullIfNone<A, B>(Option<A> repr) where A : DomainType<A, B> where B : struct =>
		repr.Match(a => a.To(), () => (B?)null);

	public static A? NullIfNone<A>(this Option<A?> repr) =>
		repr.ValueUnsafe();



	public static Option<A> NoneIfNull<A>(A? repr) =>
		Optional(repr);
	//public static B? NullIfNone<A, B>( Option<A> repr) where A : DomainType<A, B> where B : class =>
	//    repr.Match(a => a.To(), () => (B?)null);

	public static Func<string, Validation<Error, Unit>> MaxLength(int maxLength, string identifier) =>
		repr =>
			 repr.Length > maxLength
				? Fail<Error, Unit>(Errors.Errors.Domain.String.MaxLength(repr, maxLength, identifier))
				: Success<Error, Unit>(unit);

	public static Func<string, Validation<Error, Unit>> MinLength(int minLength, string identifier) =>
		repr =>
			 repr.Length < minLength ? Fail<Error, Unit>(Errors.Errors.Domain.String.MinLength(repr, minLength, identifier))
				: Success<Error, Unit>(unit);

	public static Validation<Error, TEnum> ParseEnum<TEnum>(string repr) where TEnum : Enum =>
		Enum.TryParse(typeof(TEnum), repr, true, out var result)
			? Success<Error, TEnum>((TEnum)result)
			: Fail<Error, TEnum>(Errors.Errors.Domain.Enum.ParseFailure<TEnum>(repr));

	public static Validation<Error, Unit> IsNullOrEmpty(string repr, string identifier) =>
		string.IsNullOrEmpty(repr)
			? Fail<Error, Unit>(Errors.Errors.Domain.String.IsNullOrEmpty(identifier))
			: Success<Error, Unit>(unit);
	public static Validation<Error, Unit> IsNullOrWhiteSpace(string repr, string identifier) =>
		string.IsNullOrWhiteSpace(repr)
			? Fail<Error, Unit>(Errors.Errors.Domain.String.IsNullOrWhiteSpace(identifier))
			: Success<Error, Unit>(unit);

	public static Validation<Error, Unit> NotInPast(this DateTime dateTime, DateTime utcNow, string propName, string message) =>
		dateTime < utcNow ? Fail<Error, Unit>(Errors.Errors.Domain.Date.ShouldNotBeInPast(message)) : Success<Error, Unit>(unit);
	public static Validation<Error, Unit> AtLeastOneDayDiff(this DateTime from, DateTime to, string message, string propName) =>
		(to - from).Days >= 1 ? unit : Fail<Error, Unit>(Errors.Errors.Domain.Date.AtLeastOneDayDiff(message));

	public static Validation<Error, Unit> MaxLength200(string repr, string identifier) => MaxLength(200, identifier)(repr);
	public static Validation<Error, Unit> MaxLength500(string repr, string identifier) => MaxLength(500, identifier)(repr);
	public static Validation<Error, Unit> MaxLength50(string repr, string identifier) => MaxLength(50, identifier)(repr);
	public static Validation<Error, Unit> MaxLength20(string repr, string identifier) => MaxLength(20, identifier)(repr);
	public static Validation<Error, Unit> MinLength10(string repr, string identifier) => MinLength(10, identifier)(repr);
	public static Validation<Error, Unit> MinLength8(string repr, string identifier) => MinLength(8, identifier)(repr);
	public static Validation<Error, Unit> MinLength50(string repr, string identifier) => MinLength(50, identifier)(repr);

	public static bool IsNotNull<A>(this A a)
	{
		return !a.IsNull();
	}

	public static string GenerateRefreshToken()
	{
		var bytes = new byte[64];
		RandomNumberGenerator.Fill(bytes);

		return Convert.ToBase64String(bytes)
			.Replace("+", "-")
			.Replace("/", "_")
			.Replace("=", "");
	}

	public static string Hash(string input, string salt)
	{
		using var sha = SHA256.Create();
		var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input + salt));
		return Convert.ToHexString(bytes);
	}

	public static string GenerateCode(int length)
	{
		var iterable = Range('A', 'Z').Concat(Range('0', '9')).ToArray();
		var random = new Random();
		random.Shuffle(iterable);
		return new string(iterable.Take(length).ToArray());
	}
	public static string GenerateCodeNumber(int length)
	{
		var code = RandomNumberGenerator.GetInt32(1, 1_000_000);
		return code.ToString("D6");
	}



}

