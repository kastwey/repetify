using Microsoft.Extensions.Options;

using Repetify.Application.Config;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.AuthPlatform.Abstractions;
using Repetify.Crosscutting;
using Repetify.Crosscutting.OAuth;
using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting.Extensions;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Application.Services;

/// <summary>
/// Application service for user-related operations, including OAuth sign-in and user retrieval.
/// </summary>
public class UserAppService : IUserAppService
{
	private readonly IOAuthServiceResolver _oauthServiceResolver;
	private readonly IJwtService _jwtService;
	private readonly IUserRepository _userRepository;
	private readonly FrontendConfig _frontendConfig;

	/// <summary>
	/// Initializes a new instance of the <see cref="UserAppService"/> class with the specified dependencies.
	/// </summary>
	/// <param name="googleOAuthService">The Google OAuth service implementation.</param>
	/// <param name="microsoftOAuthService">The Microsoft OAuth service implementation.</param>
	/// <param name="jwtService">The JWT service for token generation.</param>
	/// <param name="repository">The user repository for user data operations.</param>
	/// <param name="frontendConfig">The frontend configuration options.</param>
	public UserAppService(IOAuthServiceResolver oauthServiceResolver, IJwtService jwtService, IUserRepository repository, IOptionsSnapshot<FrontendConfig> frontendConfig)
	{
		_oauthServiceResolver = oauthServiceResolver;
		_jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
		_userRepository = repository ?? throw new ArgumentNullException(nameof(repository));
		_frontendConfig = frontendConfig?.Value ?? throw new ArgumentNullException(nameof(frontendConfig));
	}

	/// <inheritdoc/>
	public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
	{
		ArgumentNullException.ThrowIfNull(email);
		try
		{
			var user = await _userRepository.GetUserByEmailAsync(email).EnsureSuccessAsync().ConfigureAwait(false);
			return ResultFactory.Success<UserDto>(user.ToDto());
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<UserDto>(ex.Result);
		}
	}

	/// <inheritdoc/>
	public Result<Uri> InitiateOAuthSignin(IdentityProvider provider, Uri? returnUrl = null)
	{
		if (returnUrl is null)
		{
			returnUrl = _frontendConfig.FrontendBaseUrl;
		}

		try
		{
			var oauthService = _oauthServiceResolver.GetOAuthService(provider).EnsureSuccess();
			var redirectUri = oauthService.GetOAuthCodeUrl(returnUrl);

			if (redirectUri is null)
			{
				return ResultFactory.InvalidArgument<Uri>("Identity provider not supported.");
			}

			return ResultFactory.Success(redirectUri);
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<Uri>(ex.Result);
		}
	}

	/// <inheritdoc/>
	public async Task<Result<FinishedOAuthResponseDto>> FinishOAuthFlowAsync(IdentityProvider provider, string code, Uri? returnUrl = null)
	{
		try
		{
			var oauthService = _oauthServiceResolver.GetOAuthService(provider).EnsureSuccess();
			var tokenResponse = await oauthService.ExchangeCodeForTokenAsync(code).ConfigureAwait(false);
			var userInfo = await oauthService.GetUserInfoAsync(tokenResponse).ConfigureAwait(false);
			await CheckAndAddNewUserAsync(userInfo.Email, userInfo.Email).ConfigureAwait(false);
			var token = _jwtService.GenerateJwtToken(userInfo.FamilyName, userInfo.GivenName, userInfo.Email);
			return ResultFactory.Success(new FinishedOAuthResponseDto { JwtToken = token, ReturnUrl = returnUrl ?? _frontendConfig.FrontendBaseUrl });
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<FinishedOAuthResponseDto>(ex.Result);
		}
	}

	private async Task CheckAndAddNewUserAsync(string username, string email)
	{
		var userResult = await GetUserByEmailAsync(email).ConfigureAwait(false);
		if (userResult.Status == ResultStatus.NotFound)
		{
			await _userRepository.AddUserAsync(new User(null, email, username)).EnsureSuccessAsync().ConfigureAwait(false);
			await _userRepository.SaveChangesAsync().ConfigureAwait(false);
		}
		else
		{
			userResult.EnsureSuccess();
		}
	}
}
