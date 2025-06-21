using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Config;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Repetify.AuthPlatform.Exceptions;

namespace Repetify.AuthPlatform;

/// <summary>
/// Service for generating and parsing JWT tokens.
/// </summary>
public class JwtService : IJwtService
{

	private readonly IOptionsMonitor<JwtConfig> _jwtConfig;

	/// <summary>
	/// Initializes a new instance of the <see cref="JwtService"/> class with the specified JWT configuration.
	/// </summary>
	/// <param name="jwtConfig">The JWT configuration options monitor.</param>
	public JwtService(IOptionsMonitor<JwtConfig> jwtConfig)
	{
		_jwtConfig = jwtConfig;
	}

	/// <inheritdoc/>
	public string GenerateJwtToken(string familyName, string firstName, string emailAddress)
	{
		var claims = new[]
		{
	new Claim(JwtRegisteredClaimNames.Sub, emailAddress),
	new Claim(JwtRegisteredClaimNames.Email, emailAddress),
	new Claim(JwtRegisteredClaimNames.FamilyName, familyName),
	new Claim(JwtRegisteredClaimNames.GivenName, firstName),
	new Claim(JwtRegisteredClaimNames.Name, firstName + (familyName is not null ? $" {familyName}" : "")),
	};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.CurrentValue.SigningKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _jwtConfig.CurrentValue.Issuer,
			audience: _jwtConfig.CurrentValue.Audience,
			claims: claims,
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	/// <inheritdoc/>
	public JwtSecurityToken ParseToken(string token)
	{
		var handler = new JwtSecurityTokenHandler();
		var securityToken = handler.ReadToken(token);
		if (securityToken is not JwtSecurityToken jwtToken)
		{
			throw new InvalidTokenException("Unable to deserialize the JWT token.");
		}

		return jwtToken;
	}
}
