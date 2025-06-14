using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
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

public abstract class OAuthService : IOAuthService
{

	private readonly OAuthConfig _oauthConfig;

	private readonly HttpClient _httpClient;

	protected OAuthService(IOptionsSnapshot<OAuthConfig> oauthConfig, IHttpClientFactory httpClientFactory)
	{
		ArgumentNullException.ThrowIfNull(oauthConfig);
		ArgumentNullException.ThrowIfNull(httpClientFactory);

		_oauthConfig = oauthConfig.Value;
		_httpClient = httpClientFactory.CreateClient();
	}

	public Uri GetOAuthCodeUrl(Uri? returnUrl = null)
	{
		var dict = new Dictionary<string, string>
		{
			["client_id"] = _oauthConfig.ClientId,
			["redirect_uri"] = _oauthConfig.RedirectUri.AbsoluteUri,
			["response_type"] = "code",
			["scope"] = string.Join(' ', _oauthConfig.Scopes),
			["access_type"] = "online",
		};

		if (returnUrl is not null)
		{
			dict.Add("state", returnUrl.AbsoluteUri);
		}

		return new(_oauthConfig.OAuthCodeUrl + "?" +
			string.Join(
				'&',
				dict.Select(d => $"{d.Key}={WebUtility.UrlEncode(d.Value)}")));
	}

	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't need to catcha specific exception because we're only trying to retrieve the response content from an HTTP response. If it's not possible, the reason doesn't matter.")]
	public async Task<OAuthCodeExchangeResponse> ExchangeCodeForTokenAsync(string code)
	{
		var form = new Dictionary<string, string>
		{
			["client_id"] = _oauthConfig.ClientId,
			["client_secret"] = _oauthConfig.ClientSecret,
			["redirect_uri"] = _oauthConfig.RedirectUri.AbsoluteUri,
			["code"] = code,
			["grant_type"] = "authorization_code"
		};

		using var content = new FormUrlEncodedContent(form);

		HttpResponseMessage response;
		string responseBody;

		try
		{
			response = await _httpClient.PostAsync(_oauthConfig.OAuthTokenUrl, content).ConfigureAwait(false);
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