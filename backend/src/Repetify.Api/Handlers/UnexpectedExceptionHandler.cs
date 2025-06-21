using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Repetify.Api.Handlers;

/// <summary>
/// Handles unexpected exceptions by returning a standardized error response.
/// </summary>
[SuppressMessage("Class instantiation", "CA1812", Justification = "This class is instantiated in runtime.")]
internal sealed class UnexpectedExceptionHandler : IExceptionHandler
{
	/// <summary>
	/// Attempts to handle an unexpected exception by returning a standardized error response.
	/// </summary>
	/// <param name="httpContext">The HTTP context in which the exception occurred.</param>
	/// <param name="exception">The exception that was thrown.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the exception was handled.</returns>
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(httpContext);
		ArgumentNullException.ThrowIfNull(exception);

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "An unexpected error occurred.",
			Detail = "An unexpected error occurred. Please try again later."
		};

		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
		httpContext.Response.ContentType = "application/json";

		var json = JsonSerializer.Serialize(problemDetails);
		await httpContext.Response.WriteAsync(json, cancellationToken).ConfigureAwait(false);

		return true;
	}
}
