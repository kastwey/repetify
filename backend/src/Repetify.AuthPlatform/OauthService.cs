using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Exceptions;
using Microsoft.Extensions.Options;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Repetify.AuthPlatform;

/// <summary>
/// Abstract base class for OAuth services, providing common logic for OAuth code URL generation and token exchange.
/// </summary>
public abstract class OAuthService 
{

	private readonly IOptionsMonitor<OAuthConfig> _oauthConfig;

	private readonly HttpClient _httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="OAuthService"/> class.
	/// </summary>
	/// <param name="oauthConfig">The OAuth configuration options.</param>
	/// <param name="httpClientFactory">The HTTP client factory.</param>
	protected OAuthService(IOptionsMonitor<OAuthConfig> oauthConfig, IHttpClientFactory httpClientFactory)
	{
		ArgumentNullException.ThrowIfNull(oauthConfig);
		ArgumentNullException.ThrowIfNull(httpClientFactory);

		_oauthConfig = oauthConfig;
		_httpClient = httpClientFactory.CreateClient();
	}

	/// <summary>
	/// Generates the URL for the OAuth authorization code request.
	/// </summary>
	/// <param name="returnUrl">Optional return URL after authorization.</param>
	/// <returns>URL for the OAuth authorization code request.</returns>
	public Uri GetOAuthCodeUrl(Uri? returnUrl = null)
	{
		var dict = new Dictionary<string, string>
		{
			["client_id"] = _oauthConfig.CurrentValue.ClientId,
			["redirect_uri"] = _oauthConfig.CurrentValue.RedirectUri.AbsoluteUri,
			["response_type"] = "code",
			["scope"] = string.Join(' ', _oauthConfig.CurrentValue.Scopes),
			["access_type"] = "online",
		};

		if (returnUrl is not null)
		{
			dict.Add("state", returnUrl.AbsoluteUri);
		}

		return new(_oauthConfig.CurrentValue.OAuthCodeUrl + "?" +
			string.Join(
				'&',
				dict.Select(d => $"{d.Key}={WebUtility.UrlEncode(d.Value)}")));
	}

	/// <summary>
	/// Exchanges the authorization code for an access token.
	/// </summary>
	/// <param name="code">The authorization code received from the OAuth provider.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the OAuth token response.</returns>
	/// <exception cref="GetTokenException">Thrown when the token request fails or the response cannot be parsed.</exception>
	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't need to catcha specific exception because we're only trying to retrieve the response content from an HTTP response. If it's not possible, the reason doesn't matter.")]
	public async Task<OAuthCodeExchangeResponse> ExchangeCodeForTokenAsync(string code)
	{
		var form = new Dictionary<string, string>
		{
			["client_id"] = _oauthConfig.CurrentValue.ClientId,
			["client_secret"] = _oauthConfig.CurrentValue.ClientSecret,
			["redirect_uri"] = _oauthConfig.CurrentValue.RedirectUri.AbsoluteUri,
			["code"] = code,
			["grant_type"] = "authorization_code"
		};

		using var content = new FormUrlEncodedContent(form);

		HttpResponseMessage response;
		string responseBody;

		try
		{
			response = await _httpClient.PostAsync(_oauthConfig.CurrentValue.OAuthTokenUrl, content).ConfigureAwait(false);
			responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			throw new GetTokenException("Error making token request.", ex);
		}

		if (!response.IsSuccessStatusCode)
		{
			throw new GetTokenException($"Error retrieving token. HTTP {(int)response.StatusCode} - {response.ReasonPhrase}. Body: {responseBody}");
		}

		if (string.IsNullOrWhiteSpace(responseBody))
		{
			throw new GetTokenException($"Empty response body from token endpoint. HTTP {(int)response.StatusCode}.");
		}

		try
		{
			return JsonSerializer.Deserialize<OAuthCodeExchangeResponse>(responseBody)!;
		}
		catch (Exception ex)
		{
			throw new GetTokenException("Error parsing token response.", ex);
		}
	}
}
