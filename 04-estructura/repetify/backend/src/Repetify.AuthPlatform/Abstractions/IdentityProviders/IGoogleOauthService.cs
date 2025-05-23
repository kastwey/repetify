using Google.Apis.Auth;

namespace Repetify.AuthPlatform.Abstractions.IdentityProviders;

/// <summary>
/// Interface for Google OAuth service, extending the generic OAuth service interface.
/// </summary>
public interface IGoogleOauthService : IOauthService
{
	/// <summary>
	/// Retrieves user information from Google using the provided OAuth token.
	/// </summary>
	/// <param name="token">The OAuth token.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the user information payload.</returns>
	Task<GoogleJsonWebSignature.Payload> GetUserInfo(string token);
}
