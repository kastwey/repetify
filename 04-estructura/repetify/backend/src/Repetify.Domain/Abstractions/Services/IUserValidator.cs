using Repetify.Crosscutting;
using Repetify.Domain.Entities;

namespace Repetify.Domain.Abstractions.Services;

/// <summary>
/// Interface for validating complex rules for users.
/// These rules cannot be validated in the entity itself because they require repository dependencies.
/// </summary>
public interface IUserValidator
{
	///<summary>
	/// Ensures that the provided user is valid according to complex rules.
	/// </summary>
	/// <param name="user">The user to validate.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task<Result> EnsureIsValid(User user);
}
