using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Entities;

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

/// <summary>
/// Provides methods to map between User domain objects and UserEntity data objects.
/// </summary>
public static class UserExtensions
{
	/// <summary>
	/// Maps a User domain model to a UserEntity.
	/// </summary>
	/// <param name="userDomain">The User domain model.</param>
	/// <returns>The corresponding UserEntity.</returns>
	public static UserEntity ToDataEntity(this User userDomain)
	{
		ArgumentNullException.ThrowIfNull(userDomain);

		return new UserEntity
		{
			Id = userDomain.Id,
			Username= userDomain.Username,
			Email = userDomain.Email,
		};
	}

	/// <summary>
	/// Maps a UserEntity to a User domain model.
	/// </summary>
	/// <param name="userEntity">The UserEntity.</param>
	/// <returns>The corresponding User domain model, or null if the entity is null.</returns>
	public static User ToDomain(this UserEntity userEntity)
	{
		ArgumentNullException.ThrowIfNull(userEntity);

		return new User(
			id: userEntity.Id,
			username: userEntity.Username,
			email: userEntity.Email
		);
	}

	/// <summary>  
	/// Updates the properties of a UserEntity with the values from a User domain model.  
	/// </summary>  
	/// <param name="userEntity">The UserEntity to update.</param>  
	/// <param name="userDomain">The User domain model containing the updated values.</param>  
	public static void UpdateFromDomain(this UserEntity userEntity, User userDomain)
	{
		ArgumentNullException.ThrowIfNull(userEntity);
		ArgumentNullException.ThrowIfNull(userDomain);

		userEntity.Username = userDomain.Username;
		userEntity.Email = userDomain.Email;
	}
}
