using Microsoft.Extensions.Options;

using Repetify.Application.Config;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
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
	private readonly IGoogleOAuthService _googleOAuthService;
	private readonly IMicrosoftOAuthService _microsoftOAuthService;
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
	public UserAppService(IGoogleOAuthService googleOAuthService, IMicrosoftOAuthService microsoftOAuthService, IJwtService jwtService, IUserRepository repository, IOptionsSnapshot<FrontendConfig> frontendConfig)
	{
		_googleOAuthService = googleOAuthService ?? throw new ArgumentNullException(nameof(googleOAuthService));
		_microsoftOAuthService = microsoftOAuthService ?? throw new ArgumentNullException(nameof(microsoftOAuthService));
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

		var redirectUri = provider switch
		{
			IdentityProvider.Google => _googleOAuthService.GetOAuthCodeUrl(returnUrl),
			IdentityProvider.Microsoft => _microsoftOAuthService.GetOAuthCodeUrl(returnUrl),
			_ => null
		};

		if (redirectUri is null)
		{
			return ResultFactory.InvalidArgument<Uri>("Identity provider not supported.");
		}

		return ResultFactory.Success(redirectUri);
	}

	/// <inheritdoc/>
	public async Task<Result<FinishedOAuthResponseDto>> FinishOAuthFlowAsync(IdentityProvider provider, string code, Uri? returnUrl = null)
	{
		try
		{
			string token;

			switch (provider)
			{
				case IdentityProvider.Google:
					var googleTokenResponse = await _googleOAuthService.ExchangeCodeForTokenAsync(code).ConfigureAwait(false);
					var googleUserInfo = await _googleOAuthService.GetUserInfoAsync(googleTokenResponse.IdToken).ConfigureAwait(false);
					await CheckAndAddNewUserAsync(googleUserInfo.Email, googleUserInfo.Email).ConfigureAwait(false);
					token = _jwtService.GenerateJwtToken(googleUserInfo.FamilyName, googleUserInfo.GivenName, googleUserInfo.Email);
					break;
				case IdentityProvider.Microsoft:
					var msTokenResponse = await _microsoftOAuthService.ExchangeCodeForTokenAsync(code).ConfigureAwait(false);
					var msUserInfo = await _microsoftOAuthService.GetUserInfoAsync(msTokenResponse.AccessToken).ConfigureAwait(false);
					await CheckAndAddNewUserAsync(msUserInfo.Mail, msUserInfo.Mail).ConfigureAwait(false);
					token = _jwtService.GenerateJwtToken(msUserInfo.Surname, msUserInfo.GivenName, msUserInfo.Mail);
					break;
				default:
					return ResultFactory.InvalidArgument<FinishedOAuthResponseDto>("This identity provider is not supported.");
			}

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
