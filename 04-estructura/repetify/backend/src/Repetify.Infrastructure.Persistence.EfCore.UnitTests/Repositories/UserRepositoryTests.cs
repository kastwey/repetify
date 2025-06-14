using Microsoft.EntityFrameworkCore;

using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Repositories;
using Repetify.Infrastructure.Persistence.EfCore.UnitTests.Helpers;

namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Repositories;

public class UserRepositoryTests
{
	[Fact]
	public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
	{
		// Arrange  
		using var context = TestsHelper.CreateInMemoryDbContext();
		var repository = new UserRepository(context);
		var user = new User(Guid.NewGuid(), "testuser", "test@example.com");
		await repository.AddUserAsync(user);
		await repository.SaveChangesAsync();

		// Act  
		var result = await repository.GetUserByEmailAsync("test@example.com");

		// Assert  
		Assert.NotNull(result);
		Assert.True(result.IsSuccess);
		Assert.Equal(user.Email, result.Value!.Email);
	}

	[Fact]
	public async Task AddUserAsync_ShouldAddUserToDatabase()
	{
		// Arrange  
		using var context = TestsHelper.CreateInMemoryDbContext();
		var repository = new UserRepository(context);
		var user = new User(Guid.NewGuid(), "testuser", "test@example.com");

		// Act  
		await repository.AddUserAsync(user);
		await repository.SaveChangesAsync();

		// Assert  
		var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
		Assert.NotNull(addedUser);
		Assert.Equal(user.Email, addedUser!.Email);
	}

	[Fact]
	public async Task UpdateUser_ShouldUpdateUserInDatabase()
	{
		// Arrange  
		using var context = TestsHelper.CreateInMemoryDbContext();
		var repository = new UserRepository(context);
		var user = new User(Guid.NewGuid(), "testuser", "test@example.com");
		await repository.AddUserAsync(user);
		await repository.SaveChangesAsync();

		// Act  
		user = new User(user.Id, "updateduser", "test@example.com");
		await repository.UpdateUserAsync(user);
		await repository.SaveChangesAsync();

		// Assert  
		var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
		Assert.NotNull(updatedUser);
		Assert.Equal("updateduser", updatedUser!.Username);
	}

	[Fact]
	public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase_WhenUserExists()
	{
		// Arrange  
		using var context = TestsHelper.CreateInMemoryDbContext();
		var repository = new UserRepository(context);
		var user = new User(Guid.NewGuid(), "testuser", "test@example.com");
		await repository.AddUserAsync(user);
		await repository.SaveChangesAsync();

		// Act  
		var result = await repository.DeleteUserAsync(user.Id);
		await repository.SaveChangesAsync();

		// Assert  
		Assert.True(result.IsSuccess);
		var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
		Assert.Null(deletedUser);
	}

	[Fact]
	public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
	{
		// Arrange  
		using var context = TestsHelper.CreateInMemoryDbContext();
		var repository = new UserRepository(context);

		// Act  
		var result = await repository.DeleteUserAsync(Guid.NewGuid());

		// Assert  
		Assert.False(result.IsSuccess);
	}
}
