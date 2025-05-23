using Repetify.AuthPlatform.Entities.Microsoft;

namespace Repetify.AuthPlatform.Abstractions.IdentityProviders;

/// <summary>
/// Interface for Microsoft OAuth service, extending the generic OAuth service interface.
/// </summary>
public interface IMicrosoftOauthService : IOauthService
{
	/// <summary>
	/// Retrieves user information from Microsoft Graph API using the provided token.
	/// </summary>
	/// <param name="token">The OAuth token used to authenticate the request.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the user information.</returns>
	Task<GraphUserResponse> GetUserInfo(string token);
}
