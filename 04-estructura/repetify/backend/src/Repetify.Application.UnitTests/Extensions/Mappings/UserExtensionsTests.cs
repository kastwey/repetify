using Repetify.Application.Dtos;
using Repetify.Application.Extensions.Mappings;
using Repetify.Domain.Entities;

using Xunit;

namespace Repetify.Application.Tests.Extensions.Mappings;

public class UserExtensionsTests
{
	[Fact]
	public void ToDto_ShouldConvertUserToUserDto()
	{
		// Arrange  
		var user = new User(Guid.NewGuid(), "TestUser", "test@example.com");

		// Act  
		var userDto = user.ToDto();

		// Assert  
		Assert.NotNull(userDto);
		Assert.Equal(user.Id, userDto.Id);
		Assert.Equal(user.Username, userDto.Username);
		Assert.Equal(user.Email, userDto.Email);
	}

	[Fact]
	public void ToDtoList_ShouldConvertUserCollectionToUserDtoCollection()
	{
		// Arrange  
		var users = new List<User>
	   {
		   new User(Guid.NewGuid(), "User1", "user1@example.com"),
		   new User(Guid.NewGuid(), "User2", "user2@example.com")
	   };

		// Act  
		var userDtos = users.ToDtoList().ToList();

		// Assert  
		Assert.NotNull(userDtos);
		Assert.Equal(users.Count, userDtos.Count);
		for (int i = 0; i < users.Count; i++)
		{
			Assert.Equal(users[i].Id, userDtos[i].Id);
			Assert.Equal(users[i].Username, userDtos[i].Username);
			Assert.Equal(users[i].Email, userDtos[i].Email);
		}
	}

	[Fact]
	public void ToEntity_ShouldConvertUserDtoToUser()
	{
		// Arrange  
		var userDto = new UserDto(Guid.NewGuid(), "TestUser", "test@example.com");

		// Act  
		var user = userDto.ToEntity();

		// Assert  
		Assert.NotNull(user);
		Assert.Equal(userDto.Id, user.Id);
		Assert.Equal(userDto.Username, user.Username);
		Assert.Equal(userDto.Email, user.Email);
	}

	[Fact]
	public void ToEntityList_ShouldConvertUserDtoCollectionToUserCollection()
	{
		// Arrange  
		var userDtos = new List<UserDto>
	   {
		   new UserDto(Guid.NewGuid(), "User1", "user1@example.com"),
		   new UserDto(Guid.NewGuid(), "User2", "user2@example.com")
	   };

		// Act  
		var users = userDtos.ToEntityList().ToList();

		// Assert  
		Assert.NotNull(users);
		Assert.Equal(userDtos.Count, users.Count);
		for (int i = 0; i < userDtos.Count; i++)
		{
			Assert.Equal(userDtos[i].Id, users[i].Id);
			Assert.Equal(userDtos[i].Username, users[i].Username);
			Assert.Equal(userDtos[i].Email, users[i].Email);
		}
	}

	[Fact]
	public void ToEntity_WithAddOrEditUserDto_ShouldConvertToUser()
	{
		// Arrange  
		var addOrEditUserDto = new AddOrEditUserDto
		{
			Username = "TestUser",
			Email = "test@example.com"
		};
		var userId = Guid.NewGuid();

		// Act  
		var user = addOrEditUserDto.ToEntity(userId);

		// Assert  
		Assert.NotNull(user);
		Assert.Equal(userId, user.Id);
		Assert.Equal(addOrEditUserDto.Username, user.Username);
		Assert.Equal(addOrEditUserDto.Email, user.Email);
	}
}
