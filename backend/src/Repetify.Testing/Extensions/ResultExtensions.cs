using Repetify.Crosscutting;

namespace Repetify.Testing.Extensions;

/// <summary>
/// Provides extension methods for asserting the success of <see cref="Result"/> and <see cref="Result{T}"/> instances in tests.
/// </summary>
public static class ResultExtensions
{
	/// <summary>
	/// Ensures that the specified <see cref="Result"/> is successful.
	/// Throws an assertion if the result is not successful.
	/// </summary>
	/// <param name="result">The result to check for success.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null.</exception>
	public static void AssertIsSuccess(this Result result)
	{
		ArgumentNullException.ThrowIfNull(result);
		Assert.True(result.IsSuccess);
	}

	/// <summary>
	/// Ensures that the specified <see cref="Result{T}"/> is successful and returns its value.
	/// Throws an assertion if the result is not successful.
	/// </summary>
	/// <typeparam name="T">The type of the value contained in the result.</typeparam>
	/// <param name="result">The result to check for success.</param>
	/// <returns>The value contained in the successful result.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null.</exception>
	public static T AssertIsSuccess<T>(this Result<T> result)
	{
		ArgumentNullException.ThrowIfNull(result);
		Assert.True(result.IsSuccess);

		return result.Value;
	}
}
