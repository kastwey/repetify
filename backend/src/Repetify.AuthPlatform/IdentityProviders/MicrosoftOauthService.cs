using System.Net.Http.Headers;
using System.Text.Json;
using Repetify.AuthPlatform.Config.Microsoft;
using Repetify.AuthPlatform.Entities.Microsoft;
using Repetify.AuthPlatform.Exceptions;

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Repetify.AuthPlatform.Abstractions;
using Repetify.Crosscutting.OAuth;
using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Extensions.Mappers;

namespace Repetify.AuthPlatform.IdentityProviders;

/// <summary>
/// Service for handling Microsoft OAuth authentication and user information retrieval via Microsoft Graph API.
/// Inherits from <see cref="OAuthService"/> and implements <see cref="IMicrosoftOAuthService"/>.
/// </summary>
public sealed class MicrosoftOAuthService : OAuthService, IOAuthService
{

	private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly IOptionsMonitor<MicrosoftOAuthConfig> _oauthConfig;

	private readonly HttpClient _httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="MicrosoftOAuthService"/> class.
	/// </summary>
	/// <param name="oauthConfig">The Microsoft OAuth configuration options.</param>
	/// <param name="httpClientFactory">The HTTP client factory.</param>
	public MicrosoftOAuthService(IOptionsMonitor<MicrosoftOAuthConfig> oauthConfig, IHttpClientFactory httpClientFactory) : base(oauthConfig, httpClientFactory)
	{
		_oauthConfig = oauthConfig;
		_httpClient = httpClientFactory.CreateClient();
	}

	public IdentityProvider Provider => IdentityProvider.Microsoft;

	///   <inheritdoc/>
	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't need to catch a specific exception because we're only trying to retrieve the response content from an HTTP response. If it's not possible, the reason doesn't matter.")]
	public async Task<UserInfo> GetUserInfoAsync(OAuthCodeExchangeResponse codeExchangeResponse)
	{
		ArgumentNullException.ThrowIfNull(codeExchangeResponse);

		try
		{
			using var message = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = _oauthConfig.CurrentValue.GraphUserInfoUrl
			};

			message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", codeExchangeResponse.AccessToken);
			using var response = await _httpClient.SendAsync(message).ConfigureAwait(false);
			var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				throw new GraphException($"Error when retrieving user information from Graph. Http : {(int)response.StatusCode}. Reason: {response.ReasonPhrase}. Response body: {responseBody ?? "unknown"}.");
			}

			if (string.IsNullOrWhiteSpace(responseBody))
			{
				throw new GraphException("Received empty body from Graph.");
			}

			return JsonSerializer.Deserialize<GraphUserResponse>(responseBody, _jsonSerializerOptions)!.ToUserInfo();
		}
		catch (Exception ex) when (ex is not GraphException)
		{
			throw new GraphException("Error when retrieving user information from Graph.", ex);
		}
	}
}
