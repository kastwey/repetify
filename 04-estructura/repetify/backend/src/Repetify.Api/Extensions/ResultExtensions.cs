using Microsoft.AspNetCore.Mvc;

using Repetify.Crosscutting;

namespace Repetify.Api.Extensions;

/// <summary>
/// Extension methods to map Result and Result&lt;T&gt; to IActionResult, allowing a custom function to be passed in case of success.
/// </summary>
internal static class ResultExtensions
{
	/// <summary>
	/// Converts a <see cref="Result{T}"/> to an <see cref="IActionResult"/>.
	/// If the result is successful and <paramref name="successFunc"/> is provided, that function is invoked;
	/// otherwise, an OkObjectResult with the value is returned.
	/// In case of error, the default mapping according to the status is used.
	/// </summary>
	/// <typeparam name="T">Type of the value contained in the result.</typeparam>
	/// <param name="result">The Result object to map.</param>
	/// <param name="successFunc">
	/// Optional function that receives the value of type T and returns a custom IActionResult (e.g., CreatedAtAction).
	/// </param>
	/// <returns>An IActionResult mapped according to the status.</returns>
	public static IActionResult ToActionResult<T>(this Result<T> result, Func<T, IActionResult>? successFunc = null)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.Status == ResultStatus.Success)
		{
			return successFunc != null
				? successFunc(result.Value!)
				: new OkObjectResult(result.Value);
		}

		return result.Status switch
		{
			ResultStatus.NotFound => new NotFoundObjectResult(result.ErrorMessage),
			ResultStatus.Conflict => new ConflictObjectResult(result.ErrorMessage),
			ResultStatus.InvalidArguments => new BadRequestObjectResult(result.ErrorMessage),
			_ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
		};
	}

	/// <summary>
	/// Converts a <see cref="Result"/> to an <see cref="IActionResult"/>.
	/// If the result is successful and <paramref name="successFunc"/> is provided, that function is invoked;
	/// otherwise, an OkResult is returned.
	/// In case of error, the default mapping according to the status is used.
	/// </summary>
	/// <param name="result">The Result object to map.</param>
	/// <param name="successFunc">
	/// Optional function that returns a custom IActionResult in case of success.
	/// </param>
	/// <returns>An IActionResult mapped according to the status.</returns>
	public static IActionResult ToActionResult(this Result result, Func<IActionResult>? successFunc = null)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.Status == ResultStatus.Success)
		{
			return successFunc != null
				? successFunc()
				: new OkResult();
		}

		return result.Status switch
		{
			ResultStatus.NotFound => new NotFoundObjectResult(result.ErrorMessage),
			ResultStatus.Conflict => new ConflictObjectResult(result.ErrorMessage),
			ResultStatus.InvalidArguments => new BadRequestObjectResult(result.ErrorMessage),
			_ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
		};
	}
}
