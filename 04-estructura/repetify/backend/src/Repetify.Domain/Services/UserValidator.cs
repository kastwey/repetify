using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Entities;

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
	public async Task<Result> EnsureIsValidAsync(User user)
	{
		Result[] results = [
			await EnsureUsernameIsUnique(user).ConfigureAwait(false),
			await EnsureEmailIsUnique(user).ConfigureAwait(false)];
		var errorMessages = results
			.Where(r => !r.IsSuccess)
			.Select(r => r.ErrorMessage);

		if (errorMessages.Any())
		{
			return ResultFactory.Conflict(string.Join(Environment.NewLine, errorMessages));
		}
		
		return ResultFactory.Success();
	}

	private async Task<Result> EnsureEmailIsUnique(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var result = await _userRepository.EmailAlreadyExistsAsync(user.Id, user.Email).ConfigureAwait(false);
		
		if (!result.IsSuccess)
		{
			return ResultFactory.PropagateFailure(result);
		}
		
		return result.Value ? ResultFactory.Conflict($"A user with the email {user.Email} already exists.")
			: ResultFactory.Success();
	}

	private async Task<Result> EnsureUsernameIsUnique(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var result = await _userRepository.UsernameAlreadyExistsAsync(user.Id, user.Username).ConfigureAwait(false);
		if(!result.IsSuccess)
		{
			return ResultFactory.PropagateFailure(result);
		}

		return result.Value ? ResultFactory.Conflict("The username is already taken.") : ResultFactory.Success();
	}
}
