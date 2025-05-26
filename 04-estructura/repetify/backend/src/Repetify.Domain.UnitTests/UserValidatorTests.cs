using Moq;

using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Domain.Services;

namespace Repetify.Domain.Tests.Services;

public class UserValidatorTests
{
	private readonly Mock<IUserRepository> _userRepositoryMock;
	private readonly UserValidator _userValidator;

	public UserValidatorTests()
	{
		_userRepositoryMock = new Mock<IUserRepository>();
		_userValidator = new UserValidator(_userRepositoryMock.Object);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldNotThrow_WhenUserIsValid()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "ValidUsername", "validemail@example.com");
		_userRepositoryMock.Setup(repo => repo.EmailAlreadyExistsAsync(user.Id, user.Email))
			.ReturnsAsync(ResultFactory.Success(false));
		_userRepositoryMock.Setup(repo => repo.UsernameAlreadyExistsAsync(user.Id, user.Username))
			.ReturnsAsync(ResultFactory.Success(false));

		// Act
		var validatorResult = await _userValidator.EnsureIsValid(user);

		// assert
		Assert.True(validatorResult.IsSuccess);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldReturnConflictResult_WhenEmailAlreadyExists()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "testUser", "test@test.net");
		_userRepositoryMock.Setup(repo => repo.EmailAlreadyExistsAsync(user.Id, user.Email))
			.ReturnsAsync(ResultFactory.Success<bool>(true));
		_userRepositoryMock.Setup(repo => repo.UsernameAlreadyExistsAsync(user.Id, user.Username))
			.ReturnsAsync(ResultFactory.Success<bool>(false));

		// Act
		var validatorResult = await _userValidator.EnsureIsValid(user);

		// Assert
		Assert.Equal(ResultStatus.Conflict, validatorResult.Status);
		Assert.Equal($"A user with the email {user.Email} already exists.", validatorResult.ErrorMessage);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldReturnConflictResult_WhenUsernameAlreadyExists()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "DuplicateUsername", "validemail@example.com");

		_userRepositoryMock.Setup(repo => repo.UsernameAlreadyExistsAsync(user.Id, user.Username))
			.ReturnsAsync(ResultFactory.Success<bool>(true));
		_userRepositoryMock.Setup(repo => repo.EmailAlreadyExistsAsync(user.Id, user.Email))
			.ReturnsAsync(ResultFactory.Success<bool>(false));


		// Act
		var validatorResult = await _userValidator.EnsureIsValid(user);

		// Assert
		Assert.Equal(ResultStatus.Conflict, validatorResult.Status);
	}
}
