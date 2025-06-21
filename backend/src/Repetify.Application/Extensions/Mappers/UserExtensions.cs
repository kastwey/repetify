using Repetify.Application.Dtos;
using Repetify.Domain.Entities;

namespace Repetify.Application.Extensions.Mappers;

/// <summary>
/// Provides extension methods for mapping User domain entities to UserDto objects and vice versa.
/// </summary>
public static class UserExtensions
{
	/// <summary>
	/// Converts a User domain entity to a UserDto.
	/// </summary>
	/// <param name="user">The User domain entity to convert.</param>
	/// <returns>A UserDto representing the User domain entity.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the user is null.</exception>
	public static UserDto ToDto(this User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		return new UserDto(
			id: user.Id,
			username: user.Username,
			email: user.Email
		);
	}

	/// <summary>
	/// Converts a collection of User entities to a collection of UserDto objects.
	/// </summary>
	/// <param name="users">The collection of User entities to convert.</param>
	/// <returns>A collection of UserDto objects representing the User entities.</returns>
	public static IEnumerable<UserDto> ToDtoList(this IEnumerable<User> users)
	{
		return users.Select(user => user.ToDto());
	}

	/// <summary>
	/// Converts a UserDto to a User domain entity.
	/// </summary>
	/// <param name="userDto">The UserDto to convert.</param>
	/// <returns>A User domain entity representing the UserDto.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the userDto is null.</exception>
	public static User ToEntity(this UserDto userDto)
	{
		ArgumentNullException.ThrowIfNull(userDto);

		return new User(
			id: userDto.Id,
			username: userDto.Username,
			email: userDto.Email
		)
		{
		};
	}

	/// <summary>  
	/// Converts an AddOrEditUserDto to a User domain entity.  
	/// </summary>  
	/// <param name="addOrEditUserDto">The AddOrEditUserDto to convert.</param>  
	/// <param name="userId">The unique identifier for the User entity.</param>  
	/// <returns>A User domain entity representing the AddOrEditUserDto.</returns>  
	/// <exception cref="ArgumentNullException">Thrown when the addOrEditUserDto is null.</exception>  
	public static User ToEntity(this AddOrUpdateUserDto addOrEditUserDto, Guid? userId = null)
	{
		ArgumentNullException.ThrowIfNull(addOrEditUserDto);

		return new User(
			id: userId,
			username: addOrEditUserDto.Username!,
			email: addOrEditUserDto.Email!
		);
	}

	/// <summary>
	/// Converts a collection of UserDto objects to a collection of User entities.
	/// </summary>
	/// <param name="userDtos">The collection of UserDto objects to convert.</param>
	/// <returns>A collection of User entities representing the UserDto objects.</returns>
	public static IEnumerable<User> ToEntityList(this IEnumerable<UserDto> userDtos)
	{
		return userDtos.Select(userDto => userDto.ToEntity());
	}
}
