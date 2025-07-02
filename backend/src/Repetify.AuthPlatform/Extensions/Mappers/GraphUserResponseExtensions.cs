using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Entities.Microsoft;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Extensions.Mappers;

internal static class GraphUserResponseExtensions
{
	public static UserInfo ToUserInfo(this GraphUserResponse graphUserResponse)
	{
		ArgumentNullException.ThrowIfNull(graphUserResponse);
		return new UserInfo { GivenName = graphUserResponse.GivenName, FamilyName = graphUserResponse.Surname, Email = graphUserResponse.Mail };
	}
}