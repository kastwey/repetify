using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Repetify.Api.Config;
using Repetify.Api.Extensions;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.AuthPlatform;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.Crosscutting;
using Repetify.Crosscutting.Enums;

using System.ComponentModel.DataAnnotations;

namespace Repetify.Api.Controllers;

/// <summary>
/// Controller for handling external login operations with Google and Microsoft OAuth services.
/// </summary>
/// <param name="jwtService">JWT service for generating and validating JSON Web Tokens.</param>
/// <param name="googleOauthService">Google OAuth service for handling Google sign-in operations.</param>
/// <param name="microsoftOauthService">Microsoft OAuth service for handling Microsoft sign-in operations.</param>
/// <param name="frontendConfig">Configuration settings for the frontend application.</param>
[ApiController]
[Route("api/[controller]")]
public class ExternalLoginController(IUserAppService userAppService) : Controller
{
	private readonly IUserAppService _userAppService = userAppService;

	/// <summary>
	/// Initiates the Google sign-in process by redirecting to the Google OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Google OAuth authorization URL.</returns>
	[HttpGet("initiateGoogleSignin")]
	public IActionResult InitiateGoogleSignin([FromQuery] Uri? returnUrl)
	{
		return _userAppService.GetUriToInitiateOauthSignin(IdentityProvider.Google, returnUrl)
			.ToActionResult(redirectUrl =>Redirect(redirectUrl.AbsoluteUri));
	}

	/// <summary>
	/// Handles the Google sign-in callback, exchanges the authorization code for a token, and sets the authentication cookie.
	/// </summary>
	/// <param name="code">The authorization code received from Google.</param>
	/// <param name="state">Optional state parameter.</param>
	/// <returns>Redirects to the frontend base URL with optional state.</returns>
	[HttpGet("googleSignin")]
	public async Task<IActionResult> GoogleSignin([FromQuery, Required] string code, [FromQuery] string? state)
	{
		var result = await _userAppService.FinishOauthFlow(IdentityProvider.Google, code, state is not null ? new(state) : null).ConfigureAwait(false);
		return result.ToActionResult(response =>
		{
			Response.Cookies.Append("AuthToken", response.JwtToken, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
			return Redirect(response.ReturnUrl.AbsoluteUri);
		});
	}

	/// <summary>
	/// Initiates the Microsoft sign-in process by redirecting to the Microsoft OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Microsoft OAuth authorization URL.</returns>
	[HttpGet("initiateMicrosoftSignin")]
	public IActionResult InitiateMicrosoftSignin([FromQuery] Uri? returnUrl)
	{
		return _userAppService.GetUriToInitiateOauthSignin(IdentityProvider.Microsoft, returnUrl)
			.ToActionResult(redirectUri => Redirect(redirectUri.AbsoluteUri));
	}

	/// <summary>
	/// Handles the Microsoft sign-in callback, exchanges the authorization code for a token, and sets the authentication cookie.
	/// </summary>
	/// <param name="code">The authorization code received from Microsoft.</param>
	/// <param name="state">Optional state parameter.</param>
	/// <returns>Redirects to the frontend base URL with optional state.</returns>
	[HttpGet("microsoftSignin")]
	public async Task<IActionResult> MicrosoftSignin([FromQuery, Required] string code, [FromQuery] string? state)
	{
		var result = await _userAppService.FinishOauthFlow(IdentityProvider.Microsoft, code, state is not null ? new(state) : null).ConfigureAwait(false);
		return result.ToActionResult((oauthResponse) =>
		{
			Response.Cookies.Append("AuthToken", oauthResponse.JwtToken, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
			return Redirect(oauthResponse.ReturnUrl.AbsoluteUri);
			}); ;
	}

	/// <summary>
	/// Logs out the user by deleting the authentication cookie.
	/// </summary>
	/// <returns>Returns an OK result.</returns>
	[HttpPost("logout")]
	public IActionResult Logout()
	{
		this.Response.Cookies.Delete("AuthToken");
		return Ok();
	}
}