using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Entities;

public sealed class UserInfo
{

	public required string GivenName { get; init; }

	public required string FamilyName { get; init; }
	
	public required string Email { get; init; }
}
