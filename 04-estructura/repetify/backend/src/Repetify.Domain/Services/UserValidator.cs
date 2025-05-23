using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;
using Repetify.Domain.Exceptions;

namespace Repetify.Domain.Services;

/// <summary>
/// Validates User entities to ensure they meet specific rules.
/// </summary>
public class UserValidator : IUserValidator
{
	private IUserRepository _userRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="UserValidator"/> class.
	/// </summary>
	/// <param name="userRepository">The user repository.</param>
	public UserValidator(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	/// <inheritdoc/>
	public async Task EnsureIsValid(User user)
	{
		await EnsureEmailIsUnique(user).ConfigureAwait(false);
		await EnsureUsernameIsUnique(user).ConfigureAwait(false);
	}

	private async Task EnsureEmailIsUnique(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (await _userRepository.EmailAlreadyExistsAsync(user.Id, user.Email).ConfigureAwait(false))
		{
			throw new EntityExistsException("User", "Email", user.Email);
		}
	}

	private async Task EnsureUsernameIsUnique(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		if (await _userRepository.UsernameAlreadyExistsAsync(user.Id, user.Username).ConfigureAwait(false))
		{
			throw new EntityExistsException("User", "Username", user.Username);
		}
	}
}
