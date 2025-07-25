﻿using Repetify.AuthPlatform.Entities;
using Repetify.Crosscutting.OAuth;

namespace Repetify.AuthPlatform.Abstractions;

/// <summary>
/// Interface for OAuth service to handle OAuth operations.
/// </summary>
public interface IOAuthService
{
	public IdentityProvider Provider { get; }

	/// <summary>
	/// Generates the URL for the OAuth authorization code request.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>URL for the OAuth authorization code request.</returns>
	Uri GetOAuthCodeUrl(Uri? returnUrl = null);

	/// <summary>
	/// Exchanges the authorization code for an access token.
	/// </summary>
	/// <param name="code">The authorization code received from the OAuth provider.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the OAuth token response.</returns>
	Task<OAuthCodeExchangeResponse> ExchangeCodeForTokenAsync(string code);
	Task<UserInfo> GetUserInfoAsync(OAuthCodeExchangeResponse codeExchangeResponse);
}
