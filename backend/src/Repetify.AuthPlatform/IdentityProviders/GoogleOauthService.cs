using Google.Apis.Auth;

using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Config.Google;
using Repetify.AuthPlatform.Exceptions;

using Microsoft.Extensions.Options;
using Repetify.Crosscutting.OAuth;
using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Extensions.Mappers;

namespace Repetify.AuthPlatform.IdentityProviders;

/// <summary>
/// Service for handling Google OAuth authentication and user information retrieval.
/// Inherits common OAuth logic from <see cref="OAuthService"/> and implements <see cref="IGoogleOAuthService"/>.
/// </summary>
public sealed class GoogleOAuthService : OAuthService, IOAuthService
{

	/// <summary>
	/// Initializes a new instance of the <see cref="GoogleOAuthService"/> class with the specified Google OAuth configuration and HTTP client factory.
	/// </summary>
	/// <param name="oauthConfig">The Google OAuth configuration options.</param>
	/// <param name="httpClientFactory">The HTTP client factory used for making HTTP requests.</param>
	public GoogleOAuthService(IOptionsMonitor<GoogleOAuthConfig> oauthConfig, IHttpClientFactory httpClientFactory) : base(oauthConfig, httpClientFactory)
	{
	}

	public IdentityProvider Provider => IdentityProvider.Google;
///  <inheritdoc/>
	public async Task<UserInfo> GetUserInfoAsync(OAuthCodeExchangeResponse codeExchangeResponse)
	{
		ArgumentNullException.ThrowIfNull(codeExchangeResponse);

		try
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(codeExchangeResponse.IdToken).ConfigureAwait(false);
			return payload.ToUserInfo();
		}
		catch (Exception ex)
		{
			throw new InvalidTokenException("Error when validating and retrieving information from Google's token.", ex);
		}
	}
}
