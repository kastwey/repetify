using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Config.Microsoft;

/// <summary>
/// Configuration settings for Microsoft OAuth.
/// </summary>
public class MicrosoftOauthConfig : OauthConfig
{
	/// <summary>
	/// The configuration section name for Microsoft SSO.
	/// </summary>
	public static readonly string ConfigSection = "MicrosoftSso";
}
