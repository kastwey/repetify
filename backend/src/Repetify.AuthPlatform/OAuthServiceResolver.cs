using Repetify.AuthPlatform.Abstractions;
using Repetify.Crosscutting;
using Repetify.Crosscutting.OAuth;

namespace Repetify.AuthPlatform;

public class OAuthServiceResolver : IOAuthServiceResolver
{
	private readonly Dictionary<IdentityProvider, IOAuthService> _services;

	public OAuthServiceResolver(IEnumerable<IOAuthService> services)
	{
		_services = services.ToDictionary(s => s.Provider, s => s);
	}

	public Result<IOAuthService> GetOAuthService(IdentityProvider provider)
	{
		if (_services.TryGetValue(provider, out var authService))
		{
			return ResultFactory.Success(authService);
		}

		return ResultFactory.InvalidArgument<IOAuthService>($"The provider {provider} is not supported.");
	}
}
