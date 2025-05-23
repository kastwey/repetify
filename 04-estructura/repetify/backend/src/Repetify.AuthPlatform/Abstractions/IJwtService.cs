using System.IdentityModel.Tokens.Jwt;

namespace Repetify.AuthPlatform.Abstractions;

/// <summary>
/// Interface for JWT service to handle token generation and parsing.
/// </summary>
public interface IJwtService
{
	/// <summary>
	/// Generates a JWT token based on the provided user information.
	/// </summary>
	/// <param name="familyName">The family name of the user.</param>
	/// <param name="firstName">The first name of the user.</param>
	/// <param name="emailAddress">The email address of the user.</param>
	/// <returns>A JWT token as a string.</returns>
	public string GenerateJwtToken(string familyName, string firstName, string emailAddress);

	/// <summary>
	/// Parses a JWT token and returns the corresponding JwtSecurityToken object.
	/// </summary>
	/// <param name="token">The JWT token to parse.</param>
	/// <returns>A JwtSecurityToken object.</returns>
	public JwtSecurityToken? ParseToken(string token);
}
