using Repetify.Crosscutting;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Abstractions.Repositories;

/// <summary>
/// Represents a repository for managing user-related data operations.
/// </summary>
public interface IUserRepository
{
	/// <summary>
	/// Retrieves a user by their email address.
	/// </summary>
	/// <param name="email">The email address of the user to retrieve.</param>
	/// <returns>A <see cref="Result{T}"/> containing the user if found, or a status indicating the result.</returns>
	Task<Result<User>> GetUserByEmailAsync(string email);

	/// <summary>
	/// Adds a new user to the repository.
	/// </summary>
	Task<Result> AddUserAsync(User user);

	/// <summary>
	/// Updates an existing user's information in the repository.
	/// </summary>
	Task<Result> UpdateUserAsync(User user);

	/// <summary>
	/// Deletes a user from the repository by their unique identifier.
	/// </summary>
	/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
	Task<Result> DeleteUserAsync(Guid userId);

	/// <summary>  
	/// Saves all changes made in the repository to the underlying data store.  
	/// </summary>  
	Task SaveChangesAsync();
}
