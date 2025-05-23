using Google.Apis.Auth;

using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Config.Google;
using Repetify.AuthPlatform.Exceptions;

using Microsoft.Extensions.Options;

namespace Repetify.AuthPlatform.IdentityProviders;

public sealed class GoogleOauthService : OauthService, IGoogleOauthService
{

	public GoogleOauthService(IOptionsSnapshot<GoogleOauthConfig> oauthConfig, IHttpClientFactory httpClientFactory) : base(oauthConfig, httpClientFactory)
	{
	}

	public async Task<GoogleJsonWebSignature.Payload> GetUserInfo(string token)
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