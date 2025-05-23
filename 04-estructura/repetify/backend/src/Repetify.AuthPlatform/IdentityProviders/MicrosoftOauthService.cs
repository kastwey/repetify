using System.Net.Http.Headers;
using System.Text.Json;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config.Microsoft;
using Repetify.AuthPlatform.Entities.Microsoft;
using Repetify.AuthPlatform.Exceptions;

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Repetify.AuthPlatform.IdentityProviders;

public sealed class MicrosoftOauthService : OauthService, IMicrosoftOauthService
{

	private static JsonSerializerOptions _jsonSerializationOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private HttpClient _httpClient;

	public MicrosoftOauthService(IOptionsSnapshot<MicrosoftOauthConfig> oauthConfig, IHttpClientFactory httpClientFactory) : base(oauthConfig, httpClientFactory)
	{
		_httpClient = httpClientFactory.CreateClient();
	}

	[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't need to catch a specific exception because we're only trying to retrieve the response content from an HTTP response. If it's not possible, the reason doesn't matter.")]
	public async Task<GraphUserResponse> GetUserInfo(string token)
	{
		try
		{
			using var message = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://graph.microsoft.com/v1.0/me")
			};

			message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await _httpClient.SendAsync(message).ConfigureAwait(false);
			string responseContent = default!;
			try
			{
				responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			catch
			{
			}
			if (!response.IsSuccessStatusCode)
			{
				throw new GraphException($"Error when retrieving user information from Graph. Error code: {response.StatusCode}. Message: {responseContent ?? "unknown"}.");
			}

			return JsonSerializer.Deserialize<GraphUserResponse>(responseContent, _jsonSerializationOptions)!;
		}
		catch (Exception ex)
		{
			throw new InvalidTokenException("Error when validating and retrieving information from Microsoft's token.", ex);
		}
	}
}