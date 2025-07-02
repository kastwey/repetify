using Microsoft.Extensions.Options;

using Moq;

using Repetify.Application.Config;
using Repetify.Application.Services;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Entities;
using Repetify.AuthPlatform.Entities.Microsoft;
using Repetify.Crosscutting;
using Repetify.Crosscutting.Exceptions;
using Repetify.Crosscutting.OAuth;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;

namespace Repetify.Application.UnitTests.Services;

public class UserAppServiceTests
{
	private readonly Mock<IOAuthService> _oauthService = new();
	private readonly Mock<IJwtService> _jwtService = new();
	private readonly Mock<IUserRepository> _userRepository = new();
	private readonly Mock<IOAuthServiceResolver> _oauthServiceResolver = new();
	private readonly IOptionsSnapshot<FrontendConfig> _frontendConfig;
	private readonly FrontendConfig _frontendConfigValue;
	private readonly UserAppService _service;

	public UserAppServiceTests()
	{
		_frontendConfigValue = new FrontendConfig { FrontendBaseUrl = new("https://localhost:8080") };
		var frontendConfigMock = new Mock<IOptionsSnapshot<FrontendConfig>>();
		frontendConfigMock.Setup(x => x.Value).Returns(_frontendConfigValue);
		_frontendConfig = frontendConfigMock.Object;
		_oauthServiceResolver.Setup(m => m.GetOAuthService(It.IsAny<IdentityProvider>())).Returns(ResultFactory.Success(_oauthService.Object));
		_service = new UserAppService(
			_oauthServiceResolver.Object,
			_jwtService.Object,
			_userRepository.Object,
			_frontendConfig
		);
	}

	[Fact]
	public async Task GetUserByEmailAsync_ReturnsUserDto_When_UserExists()
	{
		var user = new User(Guid.NewGuid(), "testuser", "test@example.com");
		_userRepository.Setup(r => r.GetUserByEmailAsync("test@example.com"))
			.ReturnsAsync(ResultFactory.Success(user));

		var result = await _service.GetUserByEmailAsync("test@example.com");

		Assert.True(result.IsSuccess);
		Assert.Equal(user.Email, result.Value.Email);
		Assert.Equal(user.Username, result.Value.Username);
	}

	[Fact]
	public async Task GetUserByEmailAsync_PropagatesFailure_When_RepositoryThrowsException()
	{
		var failure = ResultFactory.NotFound<User>("not found");
		_userRepository.Setup(r => r.GetUserByEmailAsync("fail@example.com"))
			.ThrowsAsync(new ResultFailureException(failure));

		var result = await _service.GetUserByEmailAsync("fail@example.com");

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.NotFound, result.Status);
	}

	[Fact]
	public async Task FinishOauthFlow_ReturnsJwtAndUrl_When_OAuthFlowIsSuccessful()
	{
		var code = "code";
		var email = "user@google.com";
		var userInfo = new UserInfo
		{
			Email = email,
			FamilyName = "Family",
			GivenName = "Given"
		};
		_oauthService.Setup(s => s.ExchangeCodeForTokenAsync(code))
			.ReturnsAsync(new OAuthCodeExchangeResponse { IdToken = "myIdToken", AccessToken = "accessToken", RefreshToken = "refreshToken", TokenType = "bearer", ExpiresIn = 1800, });
		_oauthService.Setup(s => s.GetUserInfoAsync(It.IsAny<OAuthCodeExchangeResponse>())).ReturnsAsync(userInfo);
		_userRepository.Setup(r => r.GetUserByEmailAsync(email))
			.ReturnsAsync(ResultFactory.NotFound<User>("not found"));
		_userRepository.Setup(r => r.AddUserAsync(It.IsAny<User>())).ReturnsAsync(ResultFactory.Success());
		_userRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
		_jwtService.Setup(j => j.GenerateJwtToken("Family", "Given", email)).Returns("jwt");

		var result = await _service.FinishOAuthFlowAsync(IdentityProvider.Google, code);

		Assert.True(result.IsSuccess);
		Assert.Equal("jwt", result.Value.JwtToken);
		Assert.Equal(_frontendConfigValue.FrontendBaseUrl, result.Value.ReturnUrl);
	}

	[Fact]
	public async Task FinishOauthFlow_ReturnsInvalidArgument_When_ProviderIsUnknown()
	{
		var result = await _service.FinishOAuthFlowAsync((IdentityProvider)999, "code");

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, result.Status);
	}

	[Fact]
	public async Task FinishOauthFlow_PropagatesFailure_When_OauthServiceThrowsException()
	{
		_oauthService.Setup(s => s.ExchangeCodeForTokenAsync(It.IsAny<string>()))
			.ThrowsAsync(new ResultFailureException(ResultFactory.InvalidArgument<OAuthCodeExchangeResponse>("fail")));

		var result = await _service.FinishOAuthFlowAsync(IdentityProvider.Google, "badcode");

		Assert.False(result.IsSuccess);
		Assert.Equal(ResultStatus.InvalidArguments, result.Status);
	}
}
