using Repetify.AuthPlatform.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Repetify.AuthPlatform.Extensions.Mappers;
internal static class GooglePayloadExtensions
{
	public static UserInfo ToUserInfo(this Payload payload)
	{
		ArgumentNullException.ThrowIfNull(payload);
		return new UserInfo { GivenName = payload.GivenName, FamilyName = payload.FamilyName, Email = payload.Email };
	}
}
