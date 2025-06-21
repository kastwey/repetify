using Google.Apis.Auth;

using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Config.Google;
using Repetify.AuthPlatform.Exceptions;

using Microsoft.Extensions.Options;

namespace Repetify.AuthPlatform.IdentityProviders;

/// <summary>
/// Service for handling Google OAuth authentication and user information retrieval.
/// Inherits common OAuth logic from <see cref="OAuthService"/> and implements <see cref="IGoogleOAuthService"/>.
/// </summary>
public sealed class GoogleOAuthService : OAuthService, IGoogleOAuthService
{

	/// <summary>
	/// Initializes a new instance of the <see cref="GoogleOAuthService"/> class with the specified Google OAuth configuration and HTTP client factory.
	/// </summary>
	/// <param name="oauthConfig">The Google OAuth configuration options.</param>
	/// <param name="httpClientFactory">The HTTP client factory used for making HTTP requests.</param>
	public GoogleOAuthService(IOptionsSnapshot<GoogleOAuthConfig> oauthConfig, IHttpClientFactory httpClientFactory) : base(oauthConfig, httpClientFactory)
	{
	}

///  <inheritdoc/>
	public async Task<GoogleJsonWebSignature.Payload> GetUserInfoAsync(string token)
	{
		try
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(token).ConfigureAwait(false);
			return payload;
		}
		catch (Exception ex)
		{
			throw new InvalidTokenException("Error when validating and retrieving information from Google's token.", ex);
		}
	}
}
