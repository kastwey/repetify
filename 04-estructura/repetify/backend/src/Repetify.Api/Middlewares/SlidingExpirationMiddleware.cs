using Repetify.AuthPlatform.Abstractions;

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
			var cookieOptions = new CookieOptions();
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
	private bool ShouldRenew(string cookieValue, out DateTimeOffset newExpiration, out string? newToken)
	{
		// Parse the JWT token from the cookie value
		var jsonToken = _jwtService.ParseToken(cookieValue);
		if (jsonToken is null)
		{
			newExpiration = default;
			newToken = default;
			return false;
		}

		// Get the expiration time of the token
		var expiration = jsonToken.ValidTo;
		// Check if the token is close to expiration (less than 5 minutes remaining)
		if (expiration - DateTime.UtcNow < TimeSpan.FromMinutes(5))
		{
			// Set the new expiration time to 30 minutes from now
			newExpiration = DateTimeOffset.UtcNow.AddMinutes(30);
			// Extract user information from the token claims
			var familyName = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
			var givenName = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
			var email = jsonToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
			// Generate a new JWT token with the extracted user information
			newToken = _jwtService.GenerateJwtToken(familyName ?? string.Empty, givenName ?? string.Empty, email ?? string.Empty);
			return true;
		}

		// If the token does not need to be renewed, set default values
		newExpiration = default;
		newToken = default;
		return false;
	}
}
