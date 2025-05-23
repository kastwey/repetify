using Repetify.Application.Dtos;
using Repetify.Crosscutting;

namespace Repetify.Application.Abstractions.Services;

/// <summary>
/// Defines operations for managing user data within the system.
/// </summary>
public interface IUserAppService
{
	/// <summary>
	/// Retrieves a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user to retrieve.</param>
	/// <returns>A <see cref="Result{T}"/> containing the user data if found, or an appropriate error status.</returns>
	Task<Result<UserDto>> GetUserByEmailAsync(string email);

	/// <summary>
	/// Adds a new user to the system.
	/// </summary>
	/// <param name="user">The user data to add.</param>
	/// <returns>A <see cref="Result{T}"/> containing the ID of the newly created user, or an appropriate error status.</returns>
	Task<Result<Guid>> AddUserAsync(AddOrEditUserDto user);

	/// <summary>
	/// Edits an existing user's data.
	/// </summary>
	/// <param name="user">The updated user data.</param>
	/// <param name="userId">The user identifier corresponding to the user to be updated</param>
	/// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
	Task<Result> UpdateUserAsync(AddOrEditUserDto user, Guid userId);

	/// <summary>
	/// Deletes a user from the system.
	/// </summary>
	/// <param name="userId">The ID of the user to delete.</param>
	/// <returns>A <see cref="Result"/> indicating whether the deletion was successful, or an appropriate error status.</returns>
	Task<Result> DeleteUserAsync(Guid userId);
}
