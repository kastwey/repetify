using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Config.Google;

/// <summary>
/// Configuration settings for Google OAuth.
/// </summary>
public class GoogleOauthConfig : OauthConfig
{
	/// <summary>
	/// The configuration section name for Google SSO.
	/// </summary>
	public static readonly string ConfigSection = "GoogleSso";
}
