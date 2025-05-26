using Moq;

using Repetify.Application.Abstractions.Services;
using Repetify.Application.Dtos;
using Repetify.Application.Services;
using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

namespace Repetify.Application.UnitTests.Services;

public class UserAppServiceTests
{
	private readonly Mock<IUserRepository> _userRepositoryMock;
	private readonly Mock<IUserValidator> _userValidatorMock;
	private readonly UserAppService _userAppService;

	public UserAppServiceTests()
	{
		_userRepositoryMock = new Mock<IUserRepository>();
		_userValidatorMock = new Mock<IUserValidator>();
		_userAppService = new UserAppService(_userRepositoryMock.Object, _userValidatorMock.Object);
	}

	[Fact]
	public async Task AddUserAsync_ShouldReturnSuccess_WhenUserIsValid()
	{
		// Arrange  
		var userDto = new AddOrUpdateUserDto { Username = "testuser", Email = "test@example.com" };
		var user = new User(Guid.NewGuid(), userDto.Username, userDto.Email);
		_userValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<User>())).ReturnsAsync(ResultFactory.Success());
		_userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>())).ReturnsAsync(ResultFactory.Success());
		_userRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		// Act  
		var result = await _userAppService.AddUserAsync(userDto);

		// Assert  
		Assert.Equal(ResultStatus.Success, result.Status);
		Assert.NotEqual(Guid.Empty, result.Value);
	}

	[Fact]
	public async Task AddUserAsync_ShouldReturnConflict_WhenUserAlreadyExists()
	{
		// Arrange  
		var userDto = new AddOrUpdateUserDto { Username = "testuser", Email = "test@example.com" };
		_userValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<User>())).ReturnsAsync(ResultFactory.Conflict("User already exists."));

		// Act  
		var result = await _userAppService.AddUserAsync(userDto);

		// Assert  
		Assert.Equal(ResultStatus.Conflict, result.Status);
		Assert.Equal("User already exists.", result.ErrorMessage);
	}

	[Fact]
	public async Task DeleteUserAsync_ShouldReturnSuccess_WhenUserIsDeleted()
	{
		// Arrange  
		var userId = Guid.NewGuid();
		_userRepositoryMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(ResultFactory.Success());
		_userRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		// Act  
		var result = await _userAppService.DeleteUserAsync(userId);

		// Assert  
		Assert.Equal(ResultStatus.Success, result.Status);
		Assert.True(result.IsSuccess);
	}

	[Fact]
	public async Task DeleteUserAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
	{
		// Arrange  
		var userId = Guid.NewGuid();
		_userRepositoryMock.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(ResultFactory.NotFound("Unable to find the user to delete."));

		// Act  
		var result = await _userAppService.DeleteUserAsync(userId);

		// Assert  
		Assert.Equal(ResultStatus.NotFound, result.Status);
		Assert.Equal("Unable to find the user to delete.", result.ErrorMessage);
	}

	[Fact]
	public async Task UpdateUserAsync_ShouldReturnSuccess_WhenUserIsUpdated()
	{
		// Arrange  
		var userId = Guid.NewGuid();
		var userDto = new AddOrUpdateUserDto { Username = "updateduser", Email = "updated@example.com" };
		_userValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<User>())).ReturnsAsync(ResultFactory.Success());
		_userRepositoryMock.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(ResultFactory.Success());
		_userRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

		// Act  
		var result = await _userAppService.UpdateUserAsync(userDto, userId);

		// Assert  
		Assert.Equal(ResultStatus.Success, result.Status);
	}

	[Fact]
	public async Task UpdateUserAsync_ShouldReturnConflict_WhenUserAlreadyExists()
	{
		// Arrange  
		var userId = Guid.NewGuid();
		var userDto = new AddOrUpdateUserDto { Username = "updateduser", Email = "updated@example.com" };
		_userValidatorMock.Setup(v => v.EnsureIsValid(It.IsAny<User>())).ReturnsAsync(ResultFactory.Conflict("User already exists."));

		// Act  
		var result = await _userAppService.UpdateUserAsync(userDto, userId);

		// Assert  
		Assert.Equal(ResultStatus.Conflict, result.Status);
		Assert.Equal("User already exists.", result.ErrorMessage);
	}

	[Fact]
	public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
	{
		// Arrange  
		var email = "test@example.com";
		var user = new User(Guid.NewGuid(), "testuser", email);
		_userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(ResultFactory.Success(user));

		// Act  
		var result = await _userAppService.GetUserByEmailAsync(email);

		// Assert  
		Assert.Equal(ResultStatus.Success, result.Status);
		Assert.NotNull(result.Value);
		Assert.Equal(email, result.Value?.Email);
	}

	[Fact]
	public async Task GetUserByEmailAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
	{
		// Arrange  
		var email = "nonexistent@example.com";
		_userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(ResultFactory.NotFound<User>("User not found."));

		// Act  
		var result = await _userAppService.GetUserByEmailAsync(email);

		// Assert  
		Assert.Equal(ResultStatus.NotFound, result.Status);
		Assert.Equal("User not found.", result.ErrorMessage);
	}
}
