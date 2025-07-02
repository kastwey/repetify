using System.Numerics;
using System.Runtime.CompilerServices;

namespace Repetify.Crosscutting;

/// <summary>
/// Provides static methods for validating method arguments and returning standardized <see cref="Result"/> objects.
/// </summary>
public static class ResultValidator
{
	/// <summary>
	/// Validates that the specified object is not null.
	/// </summary>
	/// <returns><c>true</c> if the value is not null; otherwise, <c>false</c>.</returns>
	public static bool ValidateNotNull(object? obj, out Result result, [CallerArgumentExpression("obj")] string paramName = "")
	{
		if (obj is null)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' cannot be null.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that the specified string is not null or white space.
	/// </summary>
	/// <returns><c>true</c> if the string is not null or white space; otherwise, <c>false</c>.</returns>
	public static bool ValidateNotNullOrWhiteSpace(string? str, out Result result, [CallerArgumentExpression("str")] string paramName = "")
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' cannot be null or white space.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is greater than or equal to zero (i.e., not negative).
	/// </summary>
	public static bool ValidateNotNegative<T>(T value, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value < T.Zero)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be zero or greater.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is strictly greater than zero (i.e., positive).
	/// </summary>
	public static bool ValidatePositive<T>(T value, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value <= T.Zero)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be greater than zero.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is greater than the specified minimum (exclusive).
	/// </summary>
	public static bool ValidateGreaterThan<T>(T value, T min, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value <= min)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be greater than {min}.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is greater than or equal to the specified minimum (inclusive).
	/// </summary>
	public static bool ValidateGreaterThanOrEqualTo<T>(T value, T min, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value < min)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be greater than or equal to {min}.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is less than the specified maximum (exclusive).
	/// </summary>
	public static bool ValidateLessThan<T>(T value, T max, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value >= max)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be less than {max}.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is less than or equal to the specified maximum (inclusive).
	/// </summary>
	public static bool ValidateLessThanOrEqualTo<T>(T value, T max, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value > max)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be less than or equal to {max}.");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}

	/// <summary>
	/// Validates that a numeric value is within the specified range (inclusive).
	/// </summary>
	public static bool ValidateInRange<T>(T value, T min, T max, out Result result, [CallerArgumentExpression("value")] string paramName = "")
		where T : INumber<T>
	{
		if (value < min || value > max)
		{
			result = ResultFactory.InvalidArgument($"The parameter '{paramName}' must be between {min} and {max} (inclusive).");
			return false;
		}

		result = ResultFactory.Success();
		return true;
	}
}
