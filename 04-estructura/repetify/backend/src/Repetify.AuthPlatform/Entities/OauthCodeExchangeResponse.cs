using System.Text.Json.Serialization;

namespace Repetify.AuthPlatform.Entities;

public record class OAuthCodeExchangeResponse
{

	[JsonPropertyName("access_token")]
	public required string AccessToken { get; init; }

	[JsonPropertyName("id_token")]
	public required string IdToken { get; init; }
	[JsonPropertyName("expires_in")]
	public required int ExpiresIn { get; init; }

	[JsonPropertyName("refresh_token")]
	public string? RefreshToken { get; init; }

	[JsonPropertyName("token_type")]
	public required string TokenType { get; init; }
}
