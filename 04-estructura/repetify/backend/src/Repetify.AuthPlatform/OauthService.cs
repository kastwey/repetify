using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Exceptions;
using Microsoft.Extensions.Options;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace Repetify.AuthPlatform;

public abstract class OauthService : IOauthService
{

	private readonly IOptionsSnapshot<OauthConfig> _oauthConfig;

	private readonly HttpClient _httpClient;

	protected OauthService(IOptionsSnapshot<OauthConfig> oauthConfig, IHttpClientFactory httpClientFactory)
	{
		_oauthConfig = oauthConfig;
		_httpClient = httpClientFactory.CreateClient();
	}

	public Uri GetOauthCodeUrl(Uri ? returnUrl = null)
	{
		var dict = new Dictionary<string, string>
		{
			["client_id"] = _oauthConfig.Value.ClientId,
			["redirect_uri"] = _oauthConfig.Value.RedirectUri.AbsoluteUri,
			["response_type"] = "code",
			["scope"] = string.Join(' ', _oauthConfig.Value.Scopes),
			["access_type"] = "online"
		};

		if (returnUrl is not null)
		{
			dict.Add("state", returnUrl.AbsoluteUri);
		}

		return new(GetUrlFromDictionary(_oauthConfig.Value.OauthCodeUrl, dict));
	}

	private static string GetUrlFromDictionary(Uri url, Dictionary<string, string> queryStringParams)
	{
		return $"{url}?{string.Join('&', queryStringParams.Select(d => $"{d.Key}={WebUtility.UrlEncode(d.Value)}"))}";
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't need to catch a specific exception because we're only trying to retrieve the response content from an HTTP response. If it's not possible, the reason doesn't matter.")]
	public async Task<OauthCodeExchangeResponse> GetToken(string code)
	{
		using var content = new FormUrlEncodedContent(new Dictionary<string, string>()
		{
			["client_id"] = _oauthConfig.Value.ClientId,
			["client_secret"] = _oauthConfig.Value.ClientSecret,
			["redirect_uri"] = _oauthConfig.Value.RedirectUri.AbsoluteUri,
			["code"] = code,
			["grant_type"] = "authorization_code"
		});

		try
		{
			var response = await _httpClient.PostAsync(_oauthConfig.Value.OauthTokenUrl, content).ConfigureAwait(false);
			string? stringContent = null;
			try
			{
				stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			catch
			{
			}

			if (!response.IsSuccessStatusCode)
			{
				throw new GetTokenException($"Error when retrieving the token. Error code: {response.StatusCode}. Error message: {stringContent ?? "Unknown"}");
			}

			if (string.IsNullOrWhiteSpace(stringContent))
			{
				throw new GetTokenException($"Error when retrieving the token. No response from server. Response code: {response.StatusCode}.");
			}

			return JsonSerializer.Deserialize<OauthCodeExchangeResponse>(stringContent!)!;
		}
		catch (Exception ex)
		{
			throw new GetTokenException("Error when retrieving the token.", ex);
		}
	}

	protected static TPayload? GetPayload<TPayload>(string token) where TPayload : class
	{
		try
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
			if (jsonToken is null)
			{
				throw new InvalidTokenException("Invalid token.");
			}
			
			return JsonSerializer.Deserialize<TPayload>(jsonToken.Payload.SerializeToJson());
		}
		catch (Exception ex)
		{
			throw new InvalidTokenException("Error when validating and retrieving information from Google's token.", ex);
		}
	}
}