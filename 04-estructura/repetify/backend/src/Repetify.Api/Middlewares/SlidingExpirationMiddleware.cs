using Repetify.AuthPlatform.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

namespace Repetify.Api.Middlewares;

/// <summary>
/// Middleware to handle sliding expiration of JWT tokens.
/// </summary>
public class SlidingExpirationMiddleware
{
	private readonly RequestDelegate _next;

	private readonly IJwtService _jwtService;

	/// <summary>
	/// Initializes a new instance of the <see cref="SlidingExpirationMiddleware"/> class.
	/// </summary>
	/// <param name="next">The next middleware in the pipeline.</param>
	/// <param name="jwtService">The JWT service to handle token operations.</param>
	public SlidingExpirationMiddleware(RequestDelegate next, IJwtService jwtService)
	{
		_next = next;
		_jwtService = jwtService;
	}

	/// <summary>
	/// Invokes the middleware to check and renew the JWT token if necessary.
	/// </summary>
	/// <param name="context">The HTTP context.</param>
	/// <returns>A task that represents the completion of request processing.</returns>
	public async Task Invoke(HttpContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var cookie = context.Request.Cookies["AuthToken"];
		if (cookie is not null)
		{
			var cookieOptions = new CookieOptions { HttpOnly = true, Secure = true };
			if (ShouldRenew(cookie, out var newExpiration, out var newToken))
			{
				cookieOptions.Expires = newExpiration;
				context.Response.Cookies.Append("AuthToken", newToken!, cookieOptions);
			}
		}

		await _next(context).ConfigureAwait(false);
	}

	/// <summary>
	/// Determines whether the JWT token should be renewed.
	/// </summary>
	/// <param name="cookieValue">The value of the JWT token from the cookie.</param>
	/// <param name="newExpiration">The new expiration time for the token.</param>
	/// <param name="newToken">The new JWT token if renewal is needed.</param>
	/// <returns><c>true</c> if the token should be renewed; otherwise, <c>false</c>.</returns>
	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "If there is an exception, we just ignore the token and don't renew it, but we won't throw any exception.")]
	private bool ShouldRenew(string cookieValue, out DateTimeOffset newExpiration, out string? newToken)
	{
		newExpiration = default;
		newToken = default;

		try
		{
			var jsonToken = _jwtService.ParseToken(cookieValue);
			var expiration = jsonToken.ValidTo;
			if (expiration - DateTime.UtcNow < TimeSpan.FromMinutes(5))
			{
				newExpiration = DateTimeOffset.UtcNow.AddMinutes(30);
				var familyName = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
				var givenName = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
				var email = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
				newToken = _jwtService.GenerateJwtToken(familyName ?? string.Empty, givenName ?? string.Empty, email ?? string.Empty);
				return true;
			}

			return false;
		}

		catch (Exception)
		{
			return false;
		}
	}
}
