using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Repetify.Api.Config;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.AuthPlatform;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.Crosscutting;

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
public class ExternalLoginController(IJwtService jwtService, IGoogleOauthService googleOauthService, IMicrosoftOauthService microsoftOauthService, IUserAppService userAppService, IOptionsSnapshot<FrontendConfig> frontendConfig) : Controller
{

	private readonly IJwtService _jwtService = jwtService;

	private readonly IGoogleOauthService _googleOauthService = googleOauthService;

	private readonly IMicrosoftOauthService _MicrosoftOauthService = microsoftOauthService;

	private readonly IUserAppService _userAppService = userAppService;

	private readonly IOptionsSnapshot<FrontendConfig> _frontendConfig = frontendConfig;

	/// <summary>
	/// Initiates the Google sign-in process by redirecting to the Google OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Google OAuth authorization URL.</returns>
	[HttpGet("initiateGoogleSignin")]
	public IActionResult InitiateGoogleSignin([FromQuery] Uri? returnUrl)
	{
		if (returnUrl is null)
		{
			returnUrl = _frontendConfig.Value.FrontendBaseUrl;
		}

		var url = _googleOauthService.GetOauthCodeUrl(returnUrl);
		return Redirect(url.AbsoluteUri);
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
		var tokenResponse = await _googleOauthService.GetToken(code).ConfigureAwait(false);
		var payload = await _googleOauthService.GetUserInfo(tokenResponse.IdToken).ConfigureAwait(false);
		await CheckAndAddNewUserAsync(payload.Email, payload.Email).ConfigureAwait(false);
		var token = _jwtService.GenerateJwtToken(payload.FamilyName, payload.GivenName, payload.Email);

		this.Response.Cookies.Append("AuthToken", token, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
		return Redirect(_frontendConfig.Value.FrontendBaseUrl + (!string.IsNullOrWhiteSpace(state) ? state : string.Empty));
	}

	/// <summary>
	/// Initiates the Microsoft sign-in process by redirecting to the Microsoft OAuth authorization URL.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>Redirects to the Microsoft OAuth authorization URL.</returns>
	[HttpGet("initiateMicrosoftSignin")]
	public IActionResult InitiateMicrosoftSignin([FromQuery] Uri? returnUrl)
	{
		if (returnUrl is null)
		{
			returnUrl = _frontendConfig.Value.FrontendBaseUrl;
		}

		var url = _MicrosoftOauthService.GetOauthCodeUrl(returnUrl);
		return Redirect(url.AbsoluteUri);
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
		var tokenResponse = await _MicrosoftOauthService.GetToken(code).ConfigureAwait(false);
		var userInfo = await _MicrosoftOauthService.GetUserInfo(tokenResponse.AccessToken).ConfigureAwait(false);
		await CheckAndAddNewUserAsync(userInfo.Mail, userInfo.Mail).ConfigureAwait(false);
		var token = _jwtService.GenerateJwtToken(userInfo.Surname, userInfo.GivenName, userInfo.Mail);

		this.Response.Cookies.Append("AuthToken", token, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(30) });
		return Redirect(_frontendConfig.Value.FrontendBaseUrl + (!string.IsNullOrWhiteSpace(state) ? state : string.Empty));
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

	private async Task CheckAndAddNewUserAsync(string username, string email)
	{
		var userResult = await _userAppService.GetUserByEmailAsync(email).ConfigureAwait(false);

		if (!userResult.IsSuccess)
		{
			var newUser = new AddOrUpdateUserDto { Username = username, Email = email };
			await _userAppService.AddUserAsync(newUser).ConfigureAwait(false);
		}
	}
}