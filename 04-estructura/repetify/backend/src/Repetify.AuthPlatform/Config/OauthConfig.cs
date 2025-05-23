namespace Repetify.AuthPlatform.Config;

/// <summary>
/// Configuration settings for OAuth authentication. This class can't be instantiated directly.
/// </summary>
public abstract class OauthConfig
{
	/// <summary>
	/// The URL used to obtain the OAuth authorization code. This property is immutable after initialization.
	/// </summary>
	public required Uri OauthCodeUrl { get; init; }

	/// <summary>
	/// The URL used to obtain the OAuth token. This property is immutable after initialization.
	/// </summary>
	public required Uri OauthTokenUrl { get; init; }

	/// <summary>
	/// The client ID for the OAuth application. This property is immutable after initialization.
	/// </summary>
	public required string ClientId { get; init; }

	/// <summary>
	/// The client secret for the OAuth application. This property is immutable after initialization.
	/// </summary>
	public required string ClientSecret { get; init; }

	/// <summary>
	/// The redirect URI for the OAuth application. This property is immutable after initialization.
	/// </summary>
	public required Uri RedirectUri { get; init; }

	/// <summary>
	/// The scopes for the OAuth application. This property is immutable after initialization.
	/// </summary>
	public required string[] Scopes { get; init; }
}
