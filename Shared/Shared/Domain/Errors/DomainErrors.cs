namespace Shared.Domain.Errors;

public static class Errors
{
	public static class Domain
	{
		public static class String
		{
			public static ValidationError IsNullOrEmpty(string identifier) => ValidationError.New($"${identifier} is required");
			public static ValidationError IsNullOrWhiteSpace(string identifier) => ValidationError.New($"${identifier} is required");
			public static ValidationError MaxLength(string repr, int maxLength, string identifier) => ValidationError.New($"{identifier} max length is '{maxLength}' characters, but got '{repr.Length}'.");

			public static ValidationError MinLength(string repr, int minLength, string identifier) => ValidationError.New($"{identifier} min length is '{minLength}' characters, but got '{repr.Length}'.");
		}
		public static class Enum
		{
			public static ValidationError Invalid<T>(string repr) => ValidationError.New($"Value '{repr}' could not be parsed into one of the following valid values: '{System.Enum.GetValues(typeof(T))}'.");

			public static Error ParseFailure<TEnum>(string repr) where TEnum : System.Enum
			{
				throw new NotImplementedException();
			}
		}

		public static class Amenity
		{

			public static class State
			{
				public static ValidationError Invalid(int value) => ValidationError.New($"Value '{value}' of amenity is invalid.");
				public static ValidationError Invalid(string value) => ValidationError.New($"Value '{value}' of amenity is invalid.");
				public static ValidationError AlreadyExists(string repr) => ValidationError.New($"Amenity with state '{repr}' already exists.");
			}

			public static class Name
			{
				public static ValidationError Invalid(string repr) => ValidationError.New($"Amenity with value '{repr}' was not found");

				public static ValidationError Invalid(int value) => ValidationError.New($"Amenity with value '{value}' was not found");

				public static ValidationError AlreadyExists(string repr) => ValidationError.New($"Amenity with amenityName '{repr}' already exists.");

			}


			public static class Percentage
			{
				public static ValidationError Invalid(string message)
				{
					return ValidationError.New(message);
				}
			}

			public static class Cost
			{
				public static ValidationError Invalid(string message)
				{
					return ValidationError.New(message);
				}
			}
		}

		public static class Money
		{
			public static class Cent
			{
				public static ValidationError Invalid(int cent) => ValidationError.New($"Value over 100 is invalid, got '{cent}'.");
			}
		}
		public static class Currency
		{
			public static ValidationError Invalid(string code, string currencyName) =>
				throw new NotImplementedException();

			public static ValidationError AlreadyExists(string repr, string currencyName) =>
				throw new NotImplementedException();
		}

		public static class Users
		{
			public static class Email
			{
				public static ValidationError Invalid(string repr) =>
					ValidationError.New($"Email: '{repr}' is invalid");
			}
		}

		public static class Date
		{
			public static ValidationError ShouldNotBeInPast(string message) => ValidationError.New(message);

			public static ValidationError AtLeastOneDayDiff(string message) => ValidationError.New(message);

			//public static BadRequestError IsOverlappingStart(DateRange dateRange, DateRange check) =>     BadRequestError.New($"Start date {check.FromDate.ToShortDateString()} is overlapping with range : from {dateRange.FromDate.ToShortDateString()} to {dateRange.ToDate.ToShortDateString()}");

			//public static BadRequestError IsOverlappingEnd(DateRange dateRange, DateRange check) => BadRequestError.New($"End date {check.ToDate.ToShortDateString()} is overlapping with range : from {dateRange.FromDate.ToShortDateString()} to {dateRange.ToDate.ToShortDateString()}");
		}

		public static class Bookings
		{
			public static class Status
			{
				public static ValidationError Invalid(string message) => ValidationError.New(message);

				public static ValidationError InvalidVariant(string message) => ValidationError.New(message);

				public static ValidationError InvalidStatusChange(string message) => ValidationError.New(message);
			}

			public static class Permission
			{
				public static ValidationError Invalid(string message)
				{
					return ValidationError.New(message);
				}
			}

			public static class Role
			{
				public static ValidationError Invalid(string message)
				{
					return ValidationError.New(message);
				}
			}
		}
	}
}
