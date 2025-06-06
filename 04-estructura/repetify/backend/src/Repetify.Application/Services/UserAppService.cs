using Microsoft.Extensions.Options;

using Repetify.Api.Config;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappers;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.Crosscutting;
using Repetify.Crosscutting.Enums;
using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting.Extensions;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Application.Services;

public class UserAppService : IUserAppService
{
	private readonly IGoogleOauthService _googleOauthService;
	private readonly IMicrosoftOauthService _microsoftOauthService;
	private readonly IJwtService _jwtService;
	private readonly IUserRepository _userRepository;
	private readonly IUserValidator _userValidator;
	private readonly FrontendConfig _frontendConfig;

	public UserAppService(IGoogleOauthService googleOauthService, IMicrosoftOauthService microsoftOauthService, IJwtService jwtService, IUserRepository repository, IUserValidator validator, IOptionsSnapshot<FrontendConfig> frontendConfig)
	{
		_googleOauthService = googleOauthService ?? throw new ArgumentNullException(nameof(googleOauthService));
		_microsoftOauthService = microsoftOauthService ?? throw new ArgumentNullException(nameof(microsoftOauthService));
		_jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
		_userRepository = repository ?? throw new ArgumentNullException(nameof(repository));
		_userValidator = validator ?? throw new ArgumentNullException(nameof(validator));
		_frontendConfig = frontendConfig?.Value ?? throw new ArgumentNullException(nameof(frontendConfig));
	}

	public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
	{
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

	public Result<Uri> GetUriToInitiateOauthSignin(IdentityProvider provider, Uri? returnUrl = null)
	{
		if (returnUrl is null)
		{
			returnUrl = _frontendConfig.FrontendBaseUrl;
		}

		var redirectUri = provider switch
		{
			IdentityProvider.Google => _googleOauthService.GetOauthCodeUrl(returnUrl),
			IdentityProvider.Microsoft => _microsoftOauthService.GetOauthCodeUrl(returnUrl),
			_ => null
		};

		if (redirectUri is null)
		{
			return ResultFactory.InvalidArgument<Uri>("Identity provider not supported.");
		}

		return ResultFactory.Success(redirectUri);
	}

	public async Task<Result<FinishedOauthResponseDto>> FinishOauthFlow(IdentityProvider provider, string code, Uri? returnUrl = null)
	{
		try
		{
			string? token = null;

			switch (provider)
			{
				case IdentityProvider.Google:
					var tokenResponse = await _googleOauthService.ExchangeCodeForToken(code).ConfigureAwait(false);
					var payload = await _googleOauthService.GetUserInfo(tokenResponse.IdToken).ConfigureAwait(false);
					await CheckAndAddNewUserAsync(payload.Email, payload.Email).ConfigureAwait(false);
					token = _jwtService.GenerateJwtToken(payload.FamilyName, payload.GivenName, payload.Email);
					break;
				case IdentityProvider.Microsoft:
					var msTokenResponse = await _microsoftOauthService.ExchangeCodeForToken(code).ConfigureAwait(false);
					var userInfo = await _microsoftOauthService.GetUserInfo(msTokenResponse.AccessToken).ConfigureAwait(false);
					await CheckAndAddNewUserAsync(userInfo.Mail, userInfo.Mail).ConfigureAwait(false);
					token = _jwtService.GenerateJwtToken(userInfo.Surname, userInfo.GivenName, userInfo.Mail);
					break;
				default:
					return ResultFactory.InvalidArgument<FinishedOauthResponseDto>("This identity provider is not supported.");
			}

			return ResultFactory.Success(new FinishedOauthResponseDto { JwtToken = token, ReturnUrl = returnUrl ?? _frontendConfig.FrontendBaseUrl });
		}
		catch (ResultFailureException ex)
		{
			return ResultFactory.PropagateFailure<FinishedOauthResponseDto>(ex.Result);
		}
	}

	private async Task CheckAndAddNewUserAsync(string username, string email)
	{
		var userResult = await GetUserByEmailAsync(email).ConfigureAwait(false);
		if (!userResult.IsSuccess)
		{
			await _userRepository.AddUserAsync(new User(null, email, username)).EnsureSuccessAsync().ConfigureAwait(false);
			await _userRepository.SaveChangesAsync().ConfigureAwait(false);
		}
	}

}