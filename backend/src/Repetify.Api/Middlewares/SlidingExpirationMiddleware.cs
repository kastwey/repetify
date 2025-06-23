using Microsoft.AspNetCore.Authentication;
using Repetify.Api.Constants;
using Repetify.AuthPlatform.Abstractions;
using Repetify.Crosscutting.Abstractions;
using Repetify.Domain.Abstractions;
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

	private readonly IClock _clock;
	/// <summary>
	/// Initializes a new instance of the <see cref="SlidingExpirationMiddleware"/> class.
	/// </summary>
	/// <param name="next">The next middleware in the pipeline.</param>
	/// <param name="jwtService">The JWT service to handle token operations.</param>
	/// <param name="clock">The clock to calculate the current date.</param>
	public SlidingExpirationMiddleware(RequestDelegate next, IJwtService jwtService, IClock clock)
	{
		_next = next;
		_jwtService = jwtService;
		_clock = clock;
	}

	/// <summary>
	/// Invokes the middleware to check and renew the JWT token if necessary.
	/// </summary>
	/// <param name="context">The HTTP context.</param>
	/// <returns>A task that represents the completion of request processing.</returns>
	public async Task Invoke(HttpContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		var cookieValue = context.Request.Cookies[AuthConstants.AuthenticationCookieName];
		if (cookieValue is not null)
		{
			if (ShouldRenew(cookieValue, out var newExpiration, out var newToken))
			{
				var cookieOptions = new CookieOptions { HttpOnly = true, Secure = true, Expires = newExpiration };
				context.Response.Cookies.Append(AuthConstants.AuthenticationCookieName, newToken!, cookieOptions);
			}
		}

		await _next(context).ConfigureAwait(false);
	}

	/// <summary>
	/// Determines whether the JWT token should be renewed.
	/// </summary>
	/// <param name="token">The value of the JWT token from the cookie.</param>
	/// <param name="newExpiration">The new expiration time for the token.</param>
	/// <param name="newToken">The new JWT token if renewal is needed.</param>
	/// <returns><c>true</c> if the token should be renewed; otherwise, <c>false</c>.</returns>
	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "If there is an exception, we just ignore the token and don't renew it, but we won't throw any exception.")]
	private bool ShouldRenew(string token, out DateTimeOffset? newExpiration, out string? newToken)
	{
		newExpiration = default;
		newToken = null;

		try
		{
			var jwtToken = _jwtService.ParseToken(token);
			var expiration = jwtToken.ValidTo;
			if (expiration - _clock.UtcNow< TimeSpan.FromMinutes(5))
			{
				newExpiration = _clock.OffsetUtcNow.AddMinutes(30);
				var familyName = jwtToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
				var givenName = jwtToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
				var email = jwtToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
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
