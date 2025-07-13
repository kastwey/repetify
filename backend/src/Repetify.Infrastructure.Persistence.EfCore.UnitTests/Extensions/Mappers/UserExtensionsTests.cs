using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Extensions.Mappers;

public class UserExtensionsTests
{
	[Fact]
	public void ToDataEntity_ValidUserDomain_ReturnsUserEntity()
	{
		// Arrange  
		var userDomain = new User(Guid.NewGuid(), "TestUser", "test@example.com");

		// Act  
		var userEntity = userDomain.ToDataEntity();

		// Assert  
		Assert.NotNull(userEntity);
		Assert.Equal(userDomain.Id, userEntity.Id);
		Assert.Equal(userDomain.Username, userEntity.Username);
		Assert.Equal(userDomain.Email, userEntity.Email);
	}

	[Fact]
	public void ToDataEntity_NullUserDomain_ReturnsInvalidArgumentResult()
	{
		// Arrange  
		User? userDomain = null;

		// Act & Assert  
		Assert.Throws<ArgumentNullException>(() => userDomain!.ToDataEntity());
	}

	[Fact]
	public void ToDomain_ValidUserEntity_ReturnsUserDomain()
	{
		// Arrange  
		var userEntity = new UserEntity
		{
			Id = Guid.NewGuid(),
			Username = "TestUser",
			Email = "test@example.com"
		};

		// Act  
		var userDomain = userEntity.ToDomain();

		// Assert  
		Assert.NotNull(userDomain);
		Assert.Equal(userEntity.Id, userDomain.Id);
		Assert.Equal(userEntity.Username, userDomain.Username);
		Assert.Equal(userEntity.Email, userDomain.Email);
	}

	[Fact]
	public void ToDomain_NullUserEntity_ReturnsInvalidArgumentResult()
	{
		// Arrange  
		UserEntity? userEntity = null;

		// Act & Assert  
		Assert.Throws<ArgumentNullException>(() => userEntity!.ToDomain());
	}

	[Fact]
	public void UpdateFromDomain_ValidUserDomain_UpdatesUserEntity()
	{
		// Arrange  
		var userEntity = new UserEntity
		{
			Id = Guid.NewGuid(),
			Username = "OldUser",
			Email = "old@example.com"
		};
		var userDomain = new User(userEntity.Id, "UpdatedUser", "updated@example.com");

		// Act  
		userEntity.UpdateFromDomain(userDomain);

		// Assert  
		Assert.Equal(userDomain.Username, userEntity.Username);
		Assert.Equal(userDomain.Email, userEntity.Email);
	}

	[Fact]
	public void UpdateFromDomain_NullUserEntity_ReturnsInvalidArgumentResult()
	{
		// Arrange  
		UserEntity? userEntity = null;
		var userDomain = new User(Guid.NewGuid(), "TestUser", "test@example.com");

		// Act & Assert  
		Assert.Throws<ArgumentNullException>(() => userEntity!.UpdateFromDomain(userDomain));
	}

	[Fact]
	public void UpdateFromDomain_NullUserDomain_ReturnsInvalidArgumentResult()
	{
		// Arrange  
		var userEntity = new UserEntity
		{
			Id = Guid.NewGuid(),
			Username = "TestUser",
			Email = "test@example.com"
		};
		User? userDomain = null;

		// Act & Assert  
		Assert.Throws<ArgumentNullException>(() => userEntity.UpdateFromDomain(userDomain!));
	}
}
