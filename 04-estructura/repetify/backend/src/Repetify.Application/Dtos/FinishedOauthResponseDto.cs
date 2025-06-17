using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Application.Dtos;

/// <summary>
/// Represents the response returned after a successful OAuth authentication process.
/// Contains the generated JWT token and the URL to return to.
/// </summary>
public sealed class FinishedOAuthResponseDto
{
	/// <summary>
	/// Gets the JWT token generated for the authenticated user.
	/// </summary>
	public required string JwtToken { get; init; }

	/// <summary>
	/// Gets the URL to which the user should be redirected after authentication.
	/// </summary>
	public required Uri ReturnUrl { get; init; }
}
