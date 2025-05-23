using Moq;

using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Domain.Exceptions;
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
			.ReturnsAsync(false);
		_userRepositoryMock.Setup(repo => repo.UsernameAlreadyExistsAsync(user.Id, user.Username))
			.ReturnsAsync(false);

		// Act & Assert  
		await _userValidator.EnsureIsValid(user);
	}

	[Fact]
	public async Task EnsureIsValid_ShouldThrowEntityExistsException_WhenEmailAlreadyExists()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "ValidUsername", "duplicateemail@example.com");
		_userRepositoryMock.Setup(repo => repo.EmailAlreadyExistsAsync(user.Id, user.Email))
			.ReturnsAsync(true);

		// Act & Assert  
		await Assert.ThrowsAsync<EntityExistsException>(() => _userValidator.EnsureIsValid(user));
	}

	[Fact]
	public async Task EnsureIsValid_ShouldThrowEntityExistsException_WhenUsernameAlreadyExists()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "DuplicateUsername", "validemail@example.com");
		_userRepositoryMock.Setup(repo => repo.UsernameAlreadyExistsAsync(user.Id, user.Username))
			.ReturnsAsync(true);

		// Act & Assert  
		await Assert.ThrowsAsync<EntityExistsException>(() => _userValidator.EnsureIsValid(user));
	}
}
