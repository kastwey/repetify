using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Entities.Microsoft;

/// <summary>
/// Represents a response from Microsoft Graph API containing user information.
/// </summary>
public class GraphUserResponse
{
	/// <summary>
	/// Gets the user's principal name (UPN).
	/// </summary>
	public required string UserPrincipalName { get; init; }

	/// <summary>
	/// Gets the unique identifier of the user.
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// Gets the display name of the user.
	/// </summary>
	public required string DisplayName { get; init; }

	/// <summary>
	/// Gets the surname (last name) of the user.
	/// </summary>
	public required string Surname { get; init; }

	/// <summary>
	/// Gets the given name (first name) of the user.
	/// </summary>
	public required string GivenName { get; init; }

	/// <summary>
	/// Gets the preferred language of the user.
	/// </summary>
	public required string PreferredLanguage { get; init; }

	/// <summary>
	/// Gets the email address of the user.
	/// </summary>
	public required string Mail { get; init; }
}
