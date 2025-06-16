using Microsoft.AspNetCore.Mvc;
using Repetify.Api.Constants;
using Repetify.Api.Extensions;
using Repetify.Application.Abstractions.Services;
using Repetify.Crosscutting.OAuth;

using System.ComponentModel.DataAnnotations;

namespace Repetify.Api.Controllers.Auth;

/// <summary>
/// Controller for handling external login operations with Google and Microsoft OAuth services.
/// </summary>
/// <param name="jwtService">JWT service for generating and validating JSON Web Tokens.</param>
/// <param name="googleOauthService">Google OAuth service for handling Google sign-in operations.</param>
/// <param name="microsoftOauthService">Microsoft OAuth service for handling Microsoft sign-in operations.</param>
/// <param name="frontendConfig">Configuration settings for the frontend application.</param>
[ApiController]
[Route("auth/external")]
public class ExternalAuthController(IUserAppService userAppService) : ControllerBase
{
	private readonly IUserAppService _userAppService = userAppService;

	/// <summary>
	/// Initiates the Google sign-in process by redirecting to the Google OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Google OAuth authorization URL.</returns>
	[HttpGet("google/start")]
	public IActionResult GoogleStart([FromQuery] Uri? returnUrl)
	{
		return _userAppService.InitiateOAuthSignin(IdentityProvider.Google, returnUrl)
			.ToActionResult(redirectUrl =>Redirect(redirectUrl.AbsoluteUri));
	}

	/// <summary>
	/// Handles the Google sign-in callback, exchanges the authorization code for a token, and sets the authentication cookie.
	/// </summary>
	/// <param name="code">The authorization code received from Google.</param>
	/// <param name="state">Optional state parameter.</param>
	/// <returns>Redirects to the frontend base URL with optional state.</returns>
	[HttpGet("google/callback")]
	public async Task<IActionResult> GoogleCallback([FromQuery, Required] string code, [FromQuery] string? state)
	{
		var result = await _userAppService.FinishOAuthFlowAsync(IdentityProvider.Google, code, state is not null ? new(state) : null).ConfigureAwait(false);
		return result.ToActionResult(response =>
		{
			Response.Cookies.Append(AuthConstants.AuthenticationCookieName, response.JwtToken, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
			return Redirect(response.ReturnUrl.AbsoluteUri);
		});
	}

	/// <summary>
	/// Initiates the Microsoft sign-in process by redirecting to the Microsoft OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Microsoft OAuth authorization URL.</returns>
	[HttpGet("microsoft/start")]
	public IActionResult MicrosoftStart([FromQuery] Uri? returnUrl)
	{
		return _userAppService.InitiateOAuthSignin(IdentityProvider.Microsoft, returnUrl)
			.ToActionResult(redirectUri => Redirect(redirectUri.AbsoluteUri));
	}

	/// <summary>
	/// Handles the Microsoft sign-in callback, exchanges the authorization code for a token, and sets the authentication cookie.
	/// </summary>
	/// <param name="code">The authorization code received from Microsoft.</param>
	/// <param name="state">Optional state parameter.</param>
	/// <returns>Redirects to the frontend base URL with optional state.</returns>
	[HttpGet("microsoft/callback")]
	public async Task<IActionResult> MicrosoftCallback([FromQuery, Required] string code, [FromQuery] string? state)
	{
		var result = await _userAppService.FinishOAuthFlowAsync(IdentityProvider.Microsoft, code, state is not null ? new(state) : null).ConfigureAwait(false);
		return result.ToActionResult((oauthResponse) =>
		{
			Response.Cookies.Append(AuthConstants.AuthenticationCookieName, oauthResponse.JwtToken, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
			return Redirect(oauthResponse.ReturnUrl.AbsoluteUri);
			}); ;
	}

}