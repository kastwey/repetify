namespace Repetify.AuthPlatform.Entities.Microsoft;

public class GraphUserResponse
{
	public required string UserPrincipalName { get; set; }

	public required string Id { get; set; }

	public required string DisplayName { get; set; }

	public required string Surname { get; set; }

	public required string GivenName { get; set; }

	public required string PreferredLanguage { get; set; }

	public required string Mail { get; set; }
}
