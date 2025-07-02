using Repetify.Crosscutting;
using Repetify.Crosscutting.OAuth;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.AuthPlatform.Abstractions;

public interface IOAuthServiceResolver
{
	Result<IOAuthService> GetOAuthService(IdentityProvider provider);
}
